/**
 * MyBill Transaction History Widget
 * Handles server-side cursor-based pagination via GraphQL API
 * 
 * Features:
 * - Server-side pagination with cursor-based navigation
 * - Status filtering (All, Success, In Progress, Failed)
 * - BAN account filter dropdown (loaded from API, with "All Accounts" option)
 * - Invoice number filter (server-side contains search)
 * - Date range filter (date from / date to)
 * - Client-side search within current page results
 * - Apply Filters / Reset buttons
 * - Automatic session handling
 */
(function () {
    'use strict';

    // Configuration
    const API_ENDPOINT = '/api/MyBillTransactionHistory/GetTransactionHistory';
    const BAN_ENDPOINT = '/api/MyBillTransactionHistory/GetBanAccounts';
    
    // State management
    let state = {
        transactions: [],           // Current page transactions
        totalCount: 0,              // Total records available
        pageSize: 20,               // Records per page
        currentPage: 1,             // Current page number (for display)
        endCursor: null,            // Cursor for next page
        hasNextPage: false,         // Whether more pages exist
        cursorHistory: [],          // Stack of cursors for "Previous" navigation
        searchTerm: '',             // Current search term (client-side)
        isLoading: false,           // Loading state
        banNumber: null,            // BAN filter (from filter bar)
        invoiceNumber: null,        // Invoice number filter
        paymentStatus: null,        // Payment status filter (from filter bar dropdown)
        dateFrom: null,             // Date from filter
        dateTo: null,               // Date to filter
        banAccounts: []             // Loaded BAN accounts for filter dropdown
    };

    // DOM Elements
    let elements = {};

    /**
     * Initialize the widget
     */
    function init() {
        const container = document.getElementById('mybillTransactionHistory');
        if (!container) {
            return;
        }

        // Check if in designer mode
        const isDesignerMode = container.dataset.designerMode === 'true';
        if (isDesignerMode) {
            return;
        }

        // Check if authenticated
        const isAuthenticated = container.dataset.authenticated === 'true';
        if (!isAuthenticated) {
            return;
        }

        // Get page size from data attribute
        const pageSize = parseInt(container.dataset.pageSize) || 20;
        state.pageSize = pageSize;

        // Cache DOM elements
        cacheElements();

        // Setup event listeners
        setupEventListeners();

        // mybill-auth-refresh.js intercepts ALL /api/MyBill* fetch calls.
        // If the JWT is expired (401), it automatically refreshes via the
        // backend refresh token (7-day cookie) and retries the request.
        // No eager refresh needed — just load data directly.
        loadBanAccounts().then(function () {
            loadTransactionHistory();
        });
    }

    /**
     * Cache DOM elements for performance
     */
    function cacheElements() {
        elements = {
            container: document.getElementById('mybillTransactionHistory'),
            loadingState: document.getElementById('txnLoadingState'),
            errorState: document.getElementById('txnErrorState'),
            emptyState: document.getElementById('txnEmptyState'),
            tableContainer: document.getElementById('txnTableContainer'),
            tableBody: document.getElementById('txnTableBody'),
            errorMessage: document.getElementById('txnErrorMessage'),
            retryBtn: document.getElementById('txnRetryBtn'),
            searchInput: document.getElementById('txnSearchInput'),
            paginationFooter: document.getElementById('txnPaginationFooter'),
            pageStart: document.getElementById('txnPageStart'),
            pageEnd: document.getElementById('txnPageEnd'),
            totalRecords: document.getElementById('txnTotalRecords'),
            currentPage: document.getElementById('txnCurrentPage'),
            prevPage: document.getElementById('txnPrevPage'),
            nextPage: document.getElementById('txnNextPage'),
            // Filter bar elements
            filterBan: document.getElementById('txnFilterBan'),
            filterInvoice: document.getElementById('txnFilterInvoice'),
            filterPaymentStatus: document.getElementById('txnFilterPaymentStatus'),
            filterDateFrom: document.getElementById('txnFilterDateFrom'),
            filterDateTo: document.getElementById('txnFilterDateTo'),
            applyFiltersBtn: document.getElementById('txnApplyFilters'),
            resetFiltersBtn: document.getElementById('txnResetFilters')
        };
    }

    /**
     * Setup event listeners
     */
    function setupEventListeners() {
        // Retry button
        if (elements.retryBtn) {
            elements.retryBtn.addEventListener('click', () => {
                resetPagination();
                loadTransactionHistory();
            });
        }

        // Search input (debounced client-side filter)
        if (elements.searchInput) {
            let searchTimeout;
            elements.searchInput.addEventListener('input', (e) => {
                clearTimeout(searchTimeout);
                searchTimeout = setTimeout(() => {
                    state.searchTerm = e.target.value.toLowerCase().trim();
                    renderTransactions();
                }, 300);
            });
        }

        // Apply Filters button
        if (elements.applyFiltersBtn) {
            elements.applyFiltersBtn.addEventListener('click', () => {
                applyFilters();
            });
        }

        // Reset Filters button
        if (elements.resetFiltersBtn) {
            elements.resetFiltersBtn.addEventListener('click', () => {
                resetFilters();
            });
        }

        // Pagination - Previous
        if (elements.prevPage) {
            elements.prevPage.querySelector('a').addEventListener('click', (e) => {
                e.preventDefault();
                if (state.currentPage > 1 && !state.isLoading) {
                    goToPreviousPage();
                }
            });
        }

        // Pagination - Next
        if (elements.nextPage) {
            elements.nextPage.querySelector('a').addEventListener('click', (e) => {
                e.preventDefault();
                if (state.hasNextPage && !state.isLoading) {
                    goToNextPage();
                }
            });
        }
    }

    /**
     * Load BAN accounts for the filter dropdown
     */
    async function loadBanAccounts() {
        try {
            const response = await fetch(BAN_ENDPOINT, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
                credentials: 'include'
            });

            if (response.status === 401) {
                return;
            }

            if (!response.ok) {
                return;
            }

            const result = await response.json();

            if (result.isSuccess && result.data && Array.isArray(result.data)) {
                state.banAccounts = result.data;
                populateBanDropdown(result.data);
            }
        } catch (err) {
            // Silently fail - BAN dropdown will just show "All Accounts"
        }
    }

    /**
     * Populate BAN dropdown with loaded accounts
     */
    function populateBanDropdown(banAccounts) {
        if (!elements.filterBan) return;

        // Keep the "All Accounts" option and add BAN options
        elements.filterBan.innerHTML = '<option value="">All Accounts</option>';

        banAccounts.forEach(ban => {
            const option = document.createElement('option');
            option.value = ban.number;
            option.textContent = ban.number;
            elements.filterBan.appendChild(option);
        });
    }

    /**
     * Apply filters from the filter bar and reload transactions
     */
    function applyFilters() {
        // Read filter values from the UI
        state.banNumber = elements.filterBan ? elements.filterBan.value || null : null;
        state.invoiceNumber = elements.filterInvoice ? elements.filterInvoice.value.trim() || null : null;
        state.paymentStatus = elements.filterPaymentStatus ? elements.filterPaymentStatus.value || null : null;
        state.dateFrom = elements.filterDateFrom ? elements.filterDateFrom.value || null : null;
        state.dateTo = elements.filterDateTo ? elements.filterDateTo.value || null : null;

        resetPagination();
        loadTransactionHistory();
    }

    /**
     * Reset all filters and reload transactions
     */
    function resetFilters() {
        // Clear filter state
        state.banNumber = null;
        state.invoiceNumber = null;
        state.paymentStatus = null;
        state.dateFrom = null;
        state.dateTo = null;
        state.searchTerm = '';

        // Clear filter UI inputs
        if (elements.filterBan) elements.filterBan.value = '';
        if (elements.filterInvoice) elements.filterInvoice.value = '';
        if (elements.filterPaymentStatus) elements.filterPaymentStatus.value = '';
        if (elements.filterDateFrom) elements.filterDateFrom.value = '';
        if (elements.filterDateTo) elements.filterDateTo.value = '';
        if (elements.searchInput) elements.searchInput.value = '';

        resetPagination();
        loadTransactionHistory();
    }

    /**
     * Reset pagination state
     */
    function resetPagination() {
        state.currentPage = 1;
        state.endCursor = null;
        state.cursorHistory = [];
        state.hasNextPage = false;
    }

    /**
     * Load transaction history from API
     */
    async function loadTransactionHistory(after = null) {
        if (state.isLoading) return;

        state.isLoading = true;
        showLoading();

        try {
            const requestBody = {
                pageSize: state.pageSize,
                after: after,
                status: state.paymentStatus || null,
                banNumber: state.banNumber || null,
                invoiceNumber: state.invoiceNumber || null,
                dateFrom: state.dateFrom || null,
                dateTo: state.dateTo || null
            };

            const response = await fetch(API_ENDPOINT, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(requestBody)
            });

            if (response.status === 401) {
                handleSessionExpired();
                return;
            }

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();

            if (!result.isSuccess) {
                throw new Error(result.message || 'Failed to load transaction history');
            }

            // Update state with response data
            state.transactions = result.data.transactions || [];
            state.totalCount = result.data.totalCount || 0;
            state.endCursor = result.data.pageInfo?.endCursor || null;
            state.hasNextPage = result.data.pageInfo?.hasNextPage || false;

            // Render transactions
            renderTransactions();
            updatePagination();

        } catch (error) {
            showError(error.message || 'Failed to load transaction history');
        } finally {
            state.isLoading = false;
        }
    }

    /**
     * Go to next page
     */
    function goToNextPage() {
        if (!state.hasNextPage || !state.endCursor) return;

        // Save current cursor to history for "Previous" navigation
        if (state.currentPage === 1) {
            state.cursorHistory.push(null);
        } else {
            state.cursorHistory.push(state.cursorHistory.length === 0 ? null : state.cursorHistory[state.cursorHistory.length - 1]);
        }

        state.currentPage++;
        loadTransactionHistory(state.endCursor);
    }

    /**
     * Go to previous page
     */
    function goToPreviousPage() {
        if (state.currentPage <= 1) return;

        state.currentPage--;
        
        // Get the cursor for the previous page from history
        const previousCursor = state.cursorHistory.pop();
        
        loadTransactionHistory(previousCursor);
    }

    /**
     * Render transactions in the table
     */
    function renderTransactions() {
        if (!elements.tableBody) return;

        // Filter by search term (client-side)
        let displayTransactions = state.transactions;
        if (state.searchTerm) {
            displayTransactions = state.transactions.filter(txn => {
                const searchable = [
                    txn.reference,
                    txn.invoiceNumber,
                    txn.accountName,
                    txn.ban,
                    txn.paymentMethod,
                    txn.status
                ].join(' ').toLowerCase();
                return searchable.includes(state.searchTerm);
            });
        }

        if (displayTransactions.length === 0) {
            if (state.transactions.length === 0) {
                showEmpty();
            } else {
                // Search returned no results but we have data
                elements.tableBody.innerHTML = `
                    <tr>
                        <td colspan="8" class="text-center text-muted py-4">
                            No transactions match your search criteria.
                        </td>
                    </tr>
                `;
                showTable();
            }
            return;
        }

        // Build table rows
        const rows = displayTransactions.map(txn => {
            const statusClass = getStatusClass(txn.status);
            const formattedDate = formatDate(txn.date);
            const formattedAmount = formatCurrency(txn.amount);

            return `
                <tr>
                    <td>${formattedDate}</td>
                    <td><code>${escapeHtml(txn.reference)}</code></td>
                    <td>${escapeHtml(txn.invoiceNumber)}</td>
                    <td>${escapeHtml(txn.accountName)} (${escapeHtml(txn.ban)})</td>
                    <td>${escapeHtml(txn.paymentMethod)}</td>
                    <td class="fw-semibold">${formattedAmount}</td>
                    <td>${escapeHtml(txn.partialPayment)}</td>
                    <td><span class="badge ${statusClass}">${escapeHtml(txn.status)}</span></td>
                </tr>
            `;
        }).join('');

        elements.tableBody.innerHTML = rows;
        showTable();
    }

    /**
     * Update pagination controls
     */
    function updatePagination() {
        if (!elements.paginationFooter) return;

        const start = state.transactions.length === 0 ? 0 : ((state.currentPage - 1) * state.pageSize) + 1;
        const end = start + state.transactions.length - 1;

        if (elements.pageStart) elements.pageStart.textContent = start;
        if (elements.pageEnd) elements.pageEnd.textContent = Math.max(0, end);
        if (elements.totalRecords) elements.totalRecords.textContent = state.totalCount;
        if (elements.currentPage) elements.currentPage.textContent = state.currentPage;

        // Update Previous button state
        if (elements.prevPage) {
            if (state.currentPage <= 1) {
                elements.prevPage.classList.add('disabled');
            } else {
                elements.prevPage.classList.remove('disabled');
            }
        }

        // Update Next button state
        if (elements.nextPage) {
            if (!state.hasNextPage) {
                elements.nextPage.classList.add('disabled');
            } else {
                elements.nextPage.classList.remove('disabled');
            }
        }
    }

    /**
     * Show loading state
     */
    function showLoading() {
        if (elements.loadingState) elements.loadingState.classList.remove('d-none');
        if (elements.errorState) elements.errorState.classList.add('d-none');
        if (elements.emptyState) elements.emptyState.classList.add('d-none');
        if (elements.tableContainer) elements.tableContainer.classList.add('d-none');
    }

    /**
     * Show table
     */
    function showTable() {
        if (elements.loadingState) elements.loadingState.classList.add('d-none');
        if (elements.errorState) elements.errorState.classList.add('d-none');
        if (elements.emptyState) elements.emptyState.classList.add('d-none');
        if (elements.tableContainer) elements.tableContainer.classList.remove('d-none');
    }

    /**
     * Show error state
     */
    function showError(message) {
        if (elements.loadingState) elements.loadingState.classList.add('d-none');
        if (elements.tableContainer) elements.tableContainer.classList.add('d-none');
        if (elements.emptyState) elements.emptyState.classList.add('d-none');
        if (elements.errorState) elements.errorState.classList.remove('d-none');
        if (elements.errorMessage) elements.errorMessage.textContent = message;
    }

    /**
     * Show empty state
     */
    function showEmpty() {
        if (elements.loadingState) elements.loadingState.classList.add('d-none');
        if (elements.tableContainer) elements.tableContainer.classList.add('d-none');
        if (elements.errorState) elements.errorState.classList.add('d-none');
        if (elements.emptyState) elements.emptyState.classList.remove('d-none');
    }

    /**
     * Handle session expired
     */
    function handleSessionExpired() {
        showError('Your session has expired. Please log in again.');
        
        // Redirect to login using configurable path from appsettings
        setTimeout(() => {
            var cfg = window.mybillConfig || {};
            var loginPath = cfg.loginPath || '/my-bill-login';
            const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
            window.location.href = `${loginPath}?returnUrl=${returnUrl}`;
        }, 2000);
    }

    /**
     * Get CSS class for status badge
     */
    function getStatusClass(status) {
        switch (status?.toLowerCase()) {
            case 'success':
                return 'bg-success';
            case 'failed':
                return 'bg-danger';
            case 'in progress':
                return 'bg-warning text-dark';
            default:
                return 'bg-secondary';
        }
    }

    /**
     * Format date for display
     */
    function formatDate(dateString) {
        if (!dateString) return '-';
        try {
            const date = new Date(dateString);
            return date.toLocaleDateString('en-GB', {
                day: '2-digit',
                month: 'short',
                year: 'numeric'
            });
        } catch {
            return dateString;
        }
    }

    /**
     * Format currency for display
     */
    function formatCurrency(amount) {
        if (amount === null || amount === undefined) return '$0.00';
        return '$' + parseFloat(amount).toFixed(2);
    }

    /**
     * Escape HTML to prevent XSS
     */
    function escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    /**
     * Public API for external BAN selection
     * Can be called from other widgets (e.g., MyBill Dashboard) when BAN changes
     */
    window.MyBillTransactionHistory = {
        /**
         * Set BAN filter and reload transactions
         */
        setBanNumber: function(banNumber) {
            state.banNumber = banNumber;
            // Also update the filter bar dropdown to reflect external selection
            if (elements.filterBan) {
                elements.filterBan.value = banNumber || '';
            }
            resetPagination();
            loadTransactionHistory();
        },

        /**
         * Refresh transactions
         */
        refresh: function() {
            resetPagination();
            loadTransactionHistory();
        }
    };

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
