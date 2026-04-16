/**
 * MyBill Authentication Token Auto-Refresh
 *
 * Configuration (set via window.mybillConfig before this script loads):
 *   window.mybillConfig.jwtExpiryMinutes  – JWT lifetime from appsettings (default 14)
 *   window.mybillConfig.loginPath         – login page path from appsettings (default /my-bill-login)
 *
 * Strategy:
 *   - The refresh token has a 7-day expiry. As long as it is valid the user
 *     can continue even after hours or days of inactivity.
 *   - A proactive timer refreshes the JWT before it expires (at 80% of its lifetime).
 *   - Any MyBill API 401 triggers an immediate refresh + retry.
 *   - Redirect to login ONLY when the refresh token itself is expired.
 */

(function () {
    'use strict';

    var cfg = window.mybillConfig || {};
    var JWT_EXPIRY_MINUTES = cfg.jwtExpiryMinutes || 14;
    var LOGIN_PATH = cfg.loginPath || '/my-bill-login';

    // Proactive refresh at 80% of JWT lifetime so it never actually expires
    var PROACTIVE_REFRESH_MS = Math.floor(JWT_EXPIRY_MINUTES * 0.8 * 60 * 1000);

    var log = Function.prototype;
    var warn = Function.prototype;
    var error = Function.prototype;

    // State
    var isRefreshing = false;
    var refreshPromise = null;
    var sessionDead = false;
    var proactiveTimer = null;

    /**
     * Request a new access token from the backend.
     * The backend uses the refresh token cookie (7-day expiry) to issue a new JWT.
     */
    async function refreshAccessToken() {
        log('MyBill: Attempting to refresh access token...');

        try {
            var response = await originalFetch('/api/MyBillLoginForm/RequestNewAccessToken', {
                method: 'GET',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                    'Cache-Control': 'no-cache'
                }
            });

            if (response.ok) {
                var data = await response.json();
                if (data && data.data && data.data.isLoggedIn && data.data.jwtToken) {
                    log('MyBill: Token refreshed successfully');
                    scheduleProactiveRefresh();
                    return true;
                }
            }

            if (response.status === 401) {
                warn('MyBill: Refresh token expired');
                sessionDead = true;
                stopProactiveRefresh();
            }

            return false;
        } catch (err) {
            error('MyBill: Token refresh error:', err);
            return false;
        }
    }

    /**
     * Ensure we have a valid token. If not, refresh it.
     */
    async function ensureValidToken() {
        if (sessionDead) return false;
        if (isRefreshing) return await refreshPromise;

        isRefreshing = true;
        refreshPromise = refreshAccessToken().finally(function () {
            isRefreshing = false;
            refreshPromise = null;
        });

        return await refreshPromise;
    }

    /**
     * Redirect to login page. Called only when refresh token is truly expired.
     */
    function redirectToLogin() {
        var currentUrl = window.location.pathname + window.location.search;
        window.location.replace(LOGIN_PATH + '?returnUrl=' + encodeURIComponent(currentUrl));
    }

    // ---- Proactive refresh timer ----

    function scheduleProactiveRefresh() {
        stopProactiveRefresh();
        proactiveTimer = setTimeout(function () {
            log('MyBill: Proactive refresh timer fired (' + JWT_EXPIRY_MINUTES + ' min JWT, refreshing at 80%)');
            ensureValidToken();
        }, PROACTIVE_REFRESH_MS);
    }

    function stopProactiveRefresh() {
        if (proactiveTimer) {
            clearTimeout(proactiveTimer);
            proactiveTimer = null;
        }
    }

    // Start the first proactive cycle
    scheduleProactiveRefresh();

    // ---- Fetch interceptor ----

    var originalFetch = window.fetch;
    window.fetch = async function () {
        var args = arguments;
        var url = args[0];

        var isMyBillApi = typeof url === 'string' && (
            url.includes('/api/MyBill') ||
            url.includes('/api/Dashboard')
        );

        if (!isMyBillApi) {
            return originalFetch.apply(this, args);
        }

        if (sessionDead) {
            redirectToLogin();
            return new Response(null, { status: 401 });
        }

        try {
            var response = await originalFetch.apply(this, args);

            if (response.status === 401) {
                log('MyBill: 401 from', url, '- refreshing token');

                var ok = await ensureValidToken();
                if (ok) {
                    log('MyBill: Retrying request after token refresh');
                    return originalFetch.apply(this, args);
                } else {
                    redirectToLogin();
                    return response;
                }
            }

            return response;
        } catch (err) {
            error('MyBill: Fetch interceptor error:', err);
            throw err;
        }
    };

    // ---- Visibility change ----

    document.addEventListener('visibilitychange', function () {
        if (document.visibilityState === 'visible' && !sessionDead) {
            log('MyBill: Page visible - refreshing token');
            ensureValidToken();
        }
    });

    // ---- Cleanup ----

    window.addEventListener('beforeunload', stopProactiveRefresh);

    log('MyBill: Auth refresh init (JWT ' + JWT_EXPIRY_MINUTES + 'min, proactive at ' + Math.round(PROACTIVE_REFRESH_MS / 60000) + 'min, login: ' + LOGIN_PATH + ')');

})();
