using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Progress.Sitefinity.RestSdk.OData;
using System.Net;
using VFL.Renderer.Services;
using VFL.Renderer.Services.MyBillLogin;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System;
using System.Linq;
using VFL.Renderer.Models.MyBillLoginForm;
using VFL.Renderer.Config;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using VFL.Renderer.Common;

namespace VFL.Renderer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MyBillLoginFormController : ControllerBase
    {
        private const string AuthScheme = "MyBillAuth";
        private const string AccessTokenClaim = "access_tokenMyBill";

        private readonly IAuthServiceMyBill _authService;
        private readonly ApiSettings _apiSettings;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MyBillLoginFormController> _logger;

        public MyBillLoginFormController(
            IAuthServiceMyBill authService,
            IOptions<ApiSettings> apiSettings,
            IMemoryCache cache,
            ILogger<MyBillLoginFormController> logger)
        {
            _authService = authService;
            _apiSettings = apiSettings.Value;
            _cache = cache;
            _logger = logger;
        }

        #region Public Endpoints

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] MyBillLoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await ClearExistingSessionAsync();

                var response = await _authService.MyBillLoginAsync<MyBillLoginResponse>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                    return BadRequest(response);

                if (!response.data.isLoggedIn)
                    return Unauthorized(new { message = "Login failed", data = response.data });

                await SignInUserAsync(request.username, response.data.jwtToken);
                ClearUserCache(request.username);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill Login: Error during login");
                return StatusCode(500, new { message = "Login error", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoginUsingSingleAccessToken([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required", isSuccess = false });

            try
            {
                await ClearExistingSessionAsync();

                var response = await _authService.LoginUsingSingleAccessTokenAsync<MyBillLoginResponse>(token);

                // Handle server error (possibly expired token)
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                    return BadRequest(BuildTokenErrorResponse(response));

                if (response.StatusCode != HttpStatusCode.OK)
                    return BadRequest(BuildLoginFailedResponse(response));

                if (response.data?.isLoggedIn != true || string.IsNullOrEmpty(response.data.jwtToken))
                    return Unauthorized(new { message = "Invalid or expired access token", isSuccess = false, data = response.data });

                string username = ExtractUsernameFromJwt(response.data.jwtToken) ?? $"support_{Guid.NewGuid():N}";
                await SignInUserAsync(username, response.data.jwtToken, "support_access_token");

                return Ok(new
                {
                    isSuccess = true,
                    isLoggedIn = true,
                    message = "Login successful",
                    data = new { isLoggedIn = true, username }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in LoginUsingSingleAccessToken");
                return StatusCode(500, new { message = "Login error", error = ex.Message, isSuccess = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> RequestNewAccessToken()
        {
            try
            {
                // Check if user is currently authenticated
                string username = GetCurrentUsername();
                if (string.IsNullOrEmpty(username))
                {
                    _logger.LogWarning("MyBill: RequestNewAccessToken - User not authenticated");
                    return Unauthorized(new { message = "User not authenticated", isSuccess = false });
                }
                
                var response = await _authService.RequestNewAccessTokenAsync<MyBillLoginResponse>();

                if (response == null)
                {
                    _logger.LogWarning("MyBill: RequestNewAccessToken - Response is null");
                    return Unauthorized(new { message = "Failed to refresh token - no response", isSuccess = false });
                }

                if (response.StatusCode != HttpStatusCode.OK || response.data?.isLoggedIn != true || string.IsNullOrEmpty(response.data?.jwtToken))
                {
                    _logger.LogWarning("MyBill: RequestNewAccessToken - Token refresh failed. Status: {StatusCode}",
                        response.StatusCode);
                    return Unauthorized(new { message = "Failed to refresh token", isSuccess = false });
                }

                await SignInUserAsync(username, response.data.jwtToken);
                ClearUserCache(username);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error refreshing token");
                return Unauthorized(new { message = "Token refresh failed", error = ex.Message, isSuccess = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout([FromQuery] string returnUrl = null)
        {
            try
            {
                string username = GetCurrentUsername();
                if (!string.IsNullOrEmpty(username))
                    ClearUserCache(username);

                await ClearSessionAsync();

                var redirectUrl = returnUrl ?? _apiSettings.MyBillLoginPath ?? "/my-bill-login";
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error during logout");
                var redirectUrl = _apiSettings.MyBillLoginPath ?? "/my-bill-login";
                return Redirect(redirectUrl);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SupportLogin([FromQuery] string token, [FromQuery] string returnUrl = null)
        {
            string redirectUrl = returnUrl
                ?? _apiSettings.MyBillSupportLoginRedirectUrl
                ?? _apiSettings.MyBillDashboardPath
                ?? "/mybill/mybill-dashboard";

            if (string.IsNullOrEmpty(token))
                return Content(GenerateErrorHtml("Token Required", "No access token was provided. Please use a valid support access link."), "text/html");

            try
            {
                await ClearExistingSessionAsync(forceExpireCookies: true);

                var response = await _authService.LoginUsingSingleAccessTokenAsync<MyBillLoginResponse>(token);

                // Handle errors
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                    return Content(GenerateServerErrorHtml(response), "text/html");

                if (response.StatusCode != HttpStatusCode.OK)
                    return Content(GenerateLoginFailedHtml(response), "text/html");

                if (response.data?.isLoggedIn != true || string.IsNullOrEmpty(response.data.jwtToken))
                    return Content(GenerateErrorHtml("Login Failed", "Invalid or expired access token. Please request a new token from customer support."), "text/html");

                // Success - sign in and redirect
                string username = ExtractUsernameFromJwt(response.data.jwtToken) ?? $"support_{Guid.NewGuid():N}";
                ClearUserCache(username);
                await SignInUserAsync(username, response.data.jwtToken, "support_access_token", allowRefresh: true);

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in SupportLogin");
                return Content(GenerateErrorHtml("Login Error", "An unexpected error occurred during authentication."), "text/html");
            }
        }

        #endregion

        #region Private Helper Methods

        private string GetCurrentUsername()
        {
            return HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        }

        private void ClearUserCache(string username)
        {
            if (string.IsNullOrEmpty(username)) return;

            _cache.Remove($"MyBillProfileInformation_{username}");
            _cache.Remove($"MyBillAllAccountsByPrimary_{username}");
        }

        private async Task ClearExistingSessionAsync(bool forceExpireCookies = false)
        {
            // Clear cache for existing user
            string existingUsername = GetCurrentUsername();
            if (!string.IsNullOrEmpty(existingUsername))
                ClearUserCache(existingUsername);

            await ClearSessionAsync(forceExpireCookies);
        }

        private async Task ClearSessionAsync(bool forceExpireCookies = false)
        {
            // Always force-delete cookies with explicit path to ensure removal
            // regardless of how they were originally set
            var deleteOptions = new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            };

            Response.Cookies.Append("MyBillAuthCookie", "", deleteOptions);
            Response.Cookies.Append("mybillData", "", deleteOptions);

            if (forceExpireCookies)
            {
                Response.Cookies.Append(".MyBill.Session", "", deleteOptions);
            }

            // Sign out
            try
            {
                await HttpContext.SignOutAsync(AuthScheme);
            }
            catch
            {
                // Ignore if no existing session
            }

            // Clear session
            HttpContext.Session.Clear();
        }

        private async Task SignInUserAsync(string username, string jwtToken, string loginType = null, bool allowRefresh = false)
        {
            var claimsList = new System.Collections.Generic.List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(AccessTokenClaim, jwtToken)
            };

            // Add login type claim if provided
            if (!string.IsNullOrEmpty(loginType))
            {
                claimsList.Add(new Claim("login_type", loginType));
            }

            var identity = new ClaimsIdentity(claimsList, AuthScheme);
            var principal = new ClaimsPrincipal(identity);

            var cookieExpiry = DateTimeOffset.UtcNow.AddMinutes(_apiSettings.MyBillRefreshTokenExpiryMinutes);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = cookieExpiry
            };

            if (allowRefresh)
            {
                authProperties.IssuedUtc = DateTimeOffset.UtcNow;
                authProperties.AllowRefresh = true;
            }

            await HttpContext.SignInAsync(AuthScheme, principal, authProperties);
        }

        private string ExtractUsernameFromJwt(string jwtToken)
        {
            try
            {
                var parts = jwtToken.Split('.');
                if (parts.Length != 3) return null;

                var payload = parts[1];
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }
                payload = payload.Replace('-', '+').Replace('_', '/');

                var jsonBytes = Convert.FromBase64String(payload);
                var json = System.Text.Encoding.UTF8.GetString(jsonBytes);
                var claims = JsonConvert.DeserializeObject<JObject>(json);

                return claims["email"]?.ToString()
                    ?? claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]?.ToString()
                    ?? claims["sub"]?.ToString();
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Response Builders

        private static object BuildTokenErrorResponse(ApiResponse<MyBillLoginResponse> response)
        {
            bool isExpired = !string.IsNullOrEmpty(response.detail) && response.detail.Contains("expired");
            string message = isExpired
                ? "This access token has expired. Please request a new token from support."
                : response.title ?? "An error occurred during authentication";

            return new { message, isSuccess = false, isExpired, statusCode = 500 };
        }

        private static object BuildLoginFailedResponse(ApiResponse<MyBillLoginResponse> response)
        {
            string message = "Login failed";
            if (!string.IsNullOrEmpty(response.detail))
                message = response.detail.Contains("expired") ? "This access token has expired. Please request a new token." : response.detail;
            else if (!string.IsNullOrEmpty(response.title))
                message = response.title;

            return new { message, isSuccess = false, statusCode = (int)response.StatusCode };
        }

        #endregion

        #region HTML Generators

        private string GenerateServerErrorHtml(ApiResponse<MyBillLoginResponse> response)
        {
            if (!string.IsNullOrEmpty(response.detail) && response.detail.Contains("expired"))
            {
                return GenerateErrorHtml(
                    "Access Token Expired",
                    "This access token has expired and can no longer be used.<br><br>Please request a new access token from customer support.");
            }

            return GenerateErrorHtml(
                "Server Error",
                response.title ?? "An internal server error occurred. Please try again or contact support.");
        }

        private string GenerateLoginFailedHtml(ApiResponse<MyBillLoginResponse> response)
        {
            string message = "Authentication failed. Please check your access token and try again.";
            if (!string.IsNullOrEmpty(response.detail) && response.detail.Contains("expired"))
                message = "This access token has expired. Please request a new token from support.";

            return GenerateErrorHtml("Login Failed", message);
        }

        private static string GenerateErrorHtml(string title, string message)
        {
            var htmlBuilder = new System.Text.StringBuilder();
            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.AppendLine("<head>");
            htmlBuilder.AppendLine($"    <title>{System.Web.HttpUtility.HtmlEncode(title)} - MyBill Support</title>");
            htmlBuilder.AppendLine("    <meta charset='utf-8'>");
            htmlBuilder.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1'>");
            htmlBuilder.AppendLine("    <style>");
            htmlBuilder.AppendLine("        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; display: flex; justify-content: center; align-items: center; min-height: 100vh; margin: 0; background: #f5f5f5; }");
            htmlBuilder.AppendLine("        .error-container { text-align: center; padding: 50px 40px; background: white; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.1); max-width: 500px; margin: 20px; }");
            htmlBuilder.AppendLine("        .error-icon { width: 80px; height: 80px; background: #fee2e2; border-radius: 50%; display: flex; align-items: center; justify-content: center; margin: 0 auto 24px; font-size: 40px; color: #dc3545; }");
            htmlBuilder.AppendLine("        h1 { color: #1a1a1a; margin: 0 0 16px; font-size: 24px; font-weight: 600; }");
            htmlBuilder.AppendLine("        p { color: #4a5568; margin: 0 0 32px; line-height: 1.6; font-size: 15px; }");
            htmlBuilder.AppendLine("        .btn { display: inline-block; padding: 14px 32px; background: #e60000; color: white; text-decoration: none; border-radius: 8px; font-weight: 500; font-size: 15px; }");
            htmlBuilder.AppendLine("        .btn:hover { background: #cc0000; }");
            htmlBuilder.AppendLine("        .footer { margin-top: 32px; padding-top: 24px; border-top: 1px solid #e2e8f0; font-size: 13px; color: #718096; }");
            htmlBuilder.AppendLine("    </style>");
            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");
            htmlBuilder.AppendLine("    <div class='error-container'>");
            htmlBuilder.AppendLine("        <div class='error-icon'>&#10005;</div>");
            htmlBuilder.AppendLine($"        <h1>{System.Web.HttpUtility.HtmlEncode(title)}</h1>");
            htmlBuilder.AppendLine($"        <p>{message}</p>");
            htmlBuilder.AppendLine("        <a href='/' class='btn'>Return to Home</a>");
            htmlBuilder.AppendLine("        <div class='footer'>MyBill Support Access Portal</div>");
            htmlBuilder.AppendLine("    </div>");
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");
            return htmlBuilder.ToString();
        }

        #endregion
    }
}
