(function () {
'use strict';

const log = Function.prototype;
const warn = Function.prototype;
const error = Function.prototype;

document.addEventListener('DOMContentLoaded', function () {

        log('MyBill Dashboard initialized');
        
    // DOM element references
        const noBanAccountsDiv = document.getElementById('noBanAccounts');
        const dashboardMainDiv = document.getElementById('dashboardMain');
        const selectBan = document.getElementById('select-ban');
        const companyName = document.getElementById('companyName');
        const transactionTableBody = document.getElementById('transactionTableBody');
        const loader = document.getElementById('loader');
        const btnDownloadInvoice = document.getElementById('btnDownloadInvoice');
        const searchTransactions = document.getElementById('searchTransactions');
        
        // Current Invoice card elements
        const currentInvoiceNumberEl = document.getElementById('currentInvoiceNumber');
        const currentInvoiceDueDateEl = document.getElementById('currentInvoiceDueDate');
        const currentInvoiceAmountEl = document.getElementById('currentInvoiceAmount');
        const currentInvoiceStatusEl = document.getElementById('currentInvoiceStatus');
        
        // Total pending amount card elements
        const pendingInvoiceCount = document.getElementById('pendingInvoiceCount');
        const latestDueDate = document.getElementById('latestDueDate');
        const totalPendingAmount = document.getElementById('totalPendingAmount');

        // Store current invoice details and transactions
        let currentInvoiceNumber = null;
        let currentInvoiceFileName = null;
        let allTransactions = [];
        let originalTransactions = [];
        let currentFilter = 'All';
        
        // Pagination state
        let currentPage = 1;
        let recordsPerPage = 6;
        let filteredTransactions = [];


        if (!noBanAccountsDiv || !dashboardMainDiv) {
            error('MyBill Dashboard: Critical DOM elements missing!');
            return;
        }

        /**
         * Initialize dashboard by loading profile and BAN accounts
         */
        function initializeDashboard() {
            
            // Check if we're in designer/preview mode
            const isPreviewMode = document.querySelector('.alert-info') !== null;
            log('MyBill Dashboard: Preview mode detected?', isPreviewMode);

            
            if (isPreviewMode) {
                log('MyBill Dashboard: In preview mode, skipping API calls');
                hideLoader();
                return;
            }


            showLoader();
            log('MyBill Dashboard: Calling GetAllAccountsByPrimary API...');

            
            fetch('/api/MyBillDashboard/GetAllAccountsByPrimary', {
                method: 'GET',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include'
            })
            .then(response => {
                log('MyBill Dashboard: API Response status:', response.status);

                if (!response.ok) {
                    error('MyBill Dashboard: API call failed with status:', response.status);
                    return response.text().then(text => {
                        hideLoader();
                        noBanAccountsDiv.classList.remove('d-none');
                        noBanAccountsDiv.style.display = 'block';
                        dashboardMainDiv.classList.add('d-none');
                        dashboardMainDiv.style.display = 'none';
                        
                        const noBanTitleEl = document.getElementById('noBanTitle');
                        const noBanMessageEl = document.getElementById('noBanMessage');
                        
                        if (noBanTitleEl) noBanTitleEl.textContent = 'Unable to Load Dashboard';
                        if (noBanMessageEl) noBanMessageEl.innerHTML = 'We encountered an error loading your billing information. Please try again or contact support.';
                        
                        return null;
                    });
                }
                return response.json();
            })
            .then(res => {
                if (!res) {
                    return;
                }
                
                log('MyBill Dashboard: API response received');
                
                hideLoader();
                
                if (res.statusCode === 0 || (res.statusCode >= 200 && res.statusCode < 400)) {
                    const banAccounts = res.data?.BanAccounts || res.data?.banAccounts || [];
                    log('MyBill Dashboard: BAN accounts found:', banAccounts.length);
                    
                    // Store the account settings email from the API response
                    const profileEmail = res.data?.Email || res.data?.email;
                    if (profileEmail && isValidEmail(profileEmail)) {
                        accountSettingsEmail = profileEmail;
                        log('MyBill Dashboard: Account settings email captured:', accountSettingsEmail);
                    }
                    
                    if (banAccounts.length === 0) {
                        warn('MyBill Dashboard: User has no BAN accounts linked to their profile');
                        
                        noBanAccountsDiv.classList.remove('d-none');
                        noBanAccountsDiv.style.display = 'block';
                        dashboardMainDiv.classList.add('d-none');
                        dashboardMainDiv.style.display = 'none';
                        
                        const userInfoEl = document.getElementById('noBanUserInfo');
                        const primaryAccountName = res.data?.PrimaryAccountName || res.data?.primaryAccountName || 
                                                   res.data?.CompanyName || res.data?.companyName;
                        const contactName = res.data?.ContactFullName || res.data?.contactFullName;
                        const email = res.data?.Email || res.data?.email;
                        
                        if (userInfoEl && (primaryAccountName || contactName || email)) {
                            let userInfo = 'Logged in as: ';
                            if (contactName) userInfo += contactName;
                            if (primaryAccountName) userInfo += ` (${primaryAccountName})`;
                            if (email) userInfo += ` - ${email}`;
                            userInfoEl.textContent = userInfo;
                            userInfoEl.style.display = 'block';
                        }
                        
                        if (res.message) {
                            const noBanMessageEl = document.getElementById('noBanMessage');
                            if (noBanMessageEl) {
                                noBanMessageEl.innerHTML = res.message + 
                                    '<br /><br />Please contact our support team for assistance.';
                            }
                        }
                        
                        return;
                    }

                    log('MyBill Dashboard: Showing dashboard with', banAccounts.length, 'accounts');
                    noBanAccountsDiv.classList.add('d-none');
                    noBanAccountsDiv.style.display = 'none';
                    dashboardMainDiv.classList.remove('d-none');
                    dashboardMainDiv.style.display = 'block';

                    // Populate BAN dropdown
                    selectBan.innerHTML = '';
                    let selectedBan = null;
                    let selectedBanData = null;
                    let firstBan = null;
                    let firstBanData = null;

                    banAccounts.forEach((ban, index) => {
                        const option = document.createElement('option');
                        const banNumber = ban.Number || ban.number;
                        const banName = ban.Name || ban.name || ban.AccountName || ban.accountName;
                        const isSelected = ban.IsSelected || ban.isSelected;
                        
                        if (index === 0) {
                            firstBan = banNumber;
                            firstBanData = ban;
                        }
                        
                        option.value = banNumber;
                        option.textContent = banNumber;
                        option.setAttribute('data-account-name', banName || '');
                        
                        if (isSelected) {
                            option.selected = true;
                            selectedBan = banNumber;
                            selectedBanData = ban;
                        }
                        
                        selectBan.appendChild(option);
                    });

                    if (!selectedBan) {
                        selectedBan = firstBan;
                        selectedBanData = firstBanData;
                        if (selectBan.options.length > 0) {
                            selectBan.selectedIndex = 0;
                        }
                    }

                    if (selectedBanData) {
                        const accountName = selectedBanData.Name || selectedBanData.name || 
                                          selectedBanData.AccountName || selectedBanData.accountName || 'Account';
                        companyName.textContent = accountName;
                    } else {
                        companyName.textContent = 'Account';
                    }

                    if (selectedBanData) {
                        loadPaperlessEmails(selectedBanData);
                    }

                    if (selectedBan) {
                        loadAccountSummary(selectedBan);
                    } else {
                        error('MyBill Dashboard: No selected BAN to load account summary');
                    }
                } else {
                    showError('Failed to load profile information');
                }
            })
            .catch(err => {
                error('MyBill Dashboard: Network/fetch error:', err);
                hideLoader();
                noBanAccountsDiv.classList.remove('d-none');
                noBanAccountsDiv.style.display = 'block';
                dashboardMainDiv.classList.add('d-none');
                dashboardMainDiv.style.display = 'none';
                
                const noBanTitleEl = document.getElementById('noBanTitle');
                const noBanMessageEl = document.getElementById('noBanMessage');
                if (noBanTitleEl) noBanTitleEl.textContent = 'Connection Error';
                if (noBanMessageEl) noBanMessageEl.innerHTML = 'Unable to connect to the server. Please check your internet connection and try again.';
            });
        }

        /**
         * Load paperless emails for the selected BAN account
         */
        function loadPaperlessEmails(banData) {
            try {
                if (!banData) {
                    renderPaperlessEmails([]);
                    return;
                }
                
                const paperlessEmails = banData.PaperlessEmails || banData.paperlessEmails || [];
                renderPaperlessEmails(paperlessEmails);
            } catch (e) {
                renderPaperlessEmails([]);
            }
        }

        /**
         * Render paperless emails in the Manage Bills section
         */
        function renderPaperlessEmails(paperlessEmails) {
            const container = document.getElementById('paperlessEmailsContainer');
            if (!container) {
                return;
            }
            
            try {
                container.innerHTML = '';
            } catch (e) {
                error('Error clearing container:', e);
                return;
            }
            
            try {
                const emailSlots = [];
                for (let i = 0; i < 3; i++) {
                    if (paperlessEmails && paperlessEmails[i]) {
                        const emailInfo = paperlessEmails[i];
                        emailSlots.push({
                            email: emailInfo.Email || emailInfo.email || '',
                            id: emailInfo.Id || emailInfo.id || 0
                        });
                    } else {
                        emailSlots.push({ email: '', id: 0 });
                    }
                }
                
                let formHTML = '<form id="paperlessEmailForm" novalidate>';
                
                emailSlots.forEach((slot, index) => {
                    formHTML += `
                        <div class="mb-3">
                            <label for="paperlessEmail${index}" class="form-label">Paperless Email ${index + 1}</label>
                            <div class="input-group">
                                <div class="input-group-text"><i class="ri-mail-line"></i></div>
                                <input 
                                    type="email" 
                                    class="form-control paperless-email-input" 
                                    id="paperlessEmail${index}" 
                                    data-email-id="${slot.id}" 
                                    data-index="${index}" 
                                    value="${slot.email}" 
                                    placeholder="Enter email address" 
                                />
                                <div class="invalid-feedback" id="emailError${index}">Please enter a valid email address.</div>
                            </div>
                        </div>
                    `;
                });
                
                formHTML += `
                    <div class="d-grid mt-3">
                        <button type="submit" class="btn btn-primary" id="btnUpdatePaperlessEmails">
                            <i class="ri-save-line me-1"></i> Update Paperless Emails
                        </button>
                    </div>
                </form>
                `;
                
                container.innerHTML = formHTML;
                
                const form = document.getElementById('paperlessEmailForm');
                if (form) {
                    form.addEventListener('submit', handlePaperlessEmailUpdate);
                }
                
            } catch (e) {
                container.innerHTML = `
                    <div class="text-danger text-center py-3">
                        <i class="ri-error-warning-line fs-4 mb-2 d-block"></i>
                        <p class="mb-0">Error loading paperless emails</p>
                    </div>
                `;
            }
        }

        /**
         * Handle paperless email form submission
         */
        function handlePaperlessEmailUpdate(event) {
            event.preventDefault();
            
            const emailInputs = document.querySelectorAll('.paperless-email-input');
            const emails = [];
            let hasError = false;
            
            emailInputs.forEach((input, index) => {
                const email = input.value.trim();
                const originalEmailId = parseInt(input.getAttribute('data-email-id')) || 0;
                const errorDiv = document.getElementById(`emailError${index}`);
                
                input.classList.remove('is-invalid');
                
                if (email) {
                    if (!isValidEmail(email)) {
                        input.classList.add('is-invalid');
                        if (errorDiv) errorDiv.textContent = 'Please enter a valid email address.';
                        hasError = true;
                    } else {
                        emails.push({ email: email, id: originalEmailId });
                    }
                }
            });
            
            if (emails.length === 0 && !hasError) {
                showError('Please enter at least one email address.');
                return;
            }
            
            if (hasError) {
                showError('Please fix the invalid email addresses.');
                return;
            }
            
            const selectedBan = selectBan.value;
            if (!selectedBan) {
                showError('No BAN account selected.');
                return;
            }
            
            const submitButton = document.getElementById('btnUpdatePaperlessEmails');
            if (submitButton) {
                submitButton.disabled = true;
                submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> Updating...';
            }
            
            fetch(`/api/Dashboard/UpdatePaperless/${encodeURIComponent(selectedBan)}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify(emails)
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                if (submitButton) {
                    submitButton.disabled = false;
                    submitButton.innerHTML = '<i class="ri-save-line me-1"></i> Update Paperless Emails';
                }
                
                if (!data || (!data.isSuccess && data.statusCode !== 200)) {
                    showError('Failed to update paperless emails: ' + (data?.message || 'Unknown error'));
                    return;
                }
                
                showSuccess('Paperless emails updated successfully!');
                
                setTimeout(() => {
                    fetch(`/api/MyBillDashboard/GetAllAccountsByPrimary?selectedBanNumber=${encodeURIComponent(selectedBan)}`, {
                        method: 'GET',
                        credentials: 'include',
                        headers: { 'Cache-Control': 'no-cache' }
                    })
                    .then(response => response.json())
                    .then(res => {
                        if (res && res.data) {
                            const banAccounts = res.data?.BanAccounts || res.data?.banAccounts || [];
                            const selectedBanData = banAccounts.find(ban => 
                                (ban.Number || ban.number) === selectedBan
                            );
                            if (selectedBanData) {
                                loadPaperlessEmails(selectedBanData);
                            }
                        }
                    })
                    .catch(err => error('Error reloading account data:', err));
                }, 1000);
            })
            .catch(e => {
                error('Error updating paperless emails:', e);
                if (submitButton) {
                    submitButton.disabled = false;
                    submitButton.innerHTML = '<i class="ri-save-line me-1"></i> Update Paperless Emails';
                }
                showError('Failed to update paperless emails. Please try again.');
            });
        }

        /**
         * Validate email address format
         */
        function isValidEmail(email) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return emailRegex.test(email);
        }

        // Store the account settings email (from profile API)
        let accountSettingsEmail = '';

        /**
         * Get current user's email
         * Priority: Account settings email > Paperless emails
         */
        function getCurrentEmail() {
            try {
                // First, check if we have the account settings email (from profile API)
                if (accountSettingsEmail && isValidEmail(accountSettingsEmail)) {
                    log('MyBill Dashboard: Using account settings email:', accountSettingsEmail);
                    return accountSettingsEmail;
                }
                
                // Fallback to first paperless email if account settings email is not available
                const emailInputs = document.querySelectorAll('.paperless-email-input');
                if (emailInputs.length > 0) {
                    const firstEmail = emailInputs[0].value.trim();
                    if (firstEmail && isValidEmail(firstEmail)) {
                        log('MyBill Dashboard: Falling back to paperless email:', firstEmail);
                        return firstEmail;
                    }
                }
            } catch (e) {
                error('Error getting current email:', e);
            }
            return '';
        }

        /**
         * Load account summary for selected BAN
         */
        function loadAccountSummary(banNumber) {
            showLoader();

            fetch('/api/MyBillDashboard/GetAccountSummary', {
                method: 'POST',
                body: JSON.stringify(banNumber),
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include'
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(res => {
                hideLoader();

                if (res && (res.statusCode === 200 || (res.statusCode >= 200 && res.statusCode < 400)) && res.data) {
                    updateAccountSummaryUI(res.data);
                } else {
                    showError('Failed to load account summary');
                }
            })
            .catch(err => {
                hideLoader();
                showError('An error occurred while loading account summary: ' + err.message);
            });
        }

        /**
         * Update UI with account summary data
         */
        function updateAccountSummaryUI(data) {
            if (data.invoice) {
                const invoice = data.invoice;
                
                currentInvoiceNumber = invoice.number;
                currentInvoiceFileName = invoice.fileName;
                
                if (currentInvoiceNumberEl) currentInvoiceNumberEl.textContent = `Invoice: ${invoice.number || '---'}`;
                if (currentInvoiceDueDateEl) currentInvoiceDueDateEl.textContent = `Due: ${formatDate(invoice.dueDate)}`;
                
                const isPaid = invoice.paymentStatus === 'Paid' || invoice.paymentStatus === true;
                const displayAmount = isPaid ? 0 : (invoice.totalAmount || invoice.amount || 0);
                if (currentInvoiceAmountEl) currentInvoiceAmountEl.textContent = `Amount: $${formatCurrency(displayAmount)}`;
                
                if (currentInvoiceStatusEl) {
                    if (isPaid) {
                        currentInvoiceStatusEl.innerHTML = '<span class="badge bg-success">Paid</span>';
                    } else {
                        currentInvoiceStatusEl.innerHTML = '<span class="badge bg-warning text-dark">Pending</span>';
                    }
                }
                
                const invoiceEvent = new CustomEvent('mybill:invoiceLoaded', {
                    detail: {
                        invoice: { invoiceNumber: invoice.number, totalAmount: invoice.totalAmount || invoice.amount },
                        ban: selectBan.value,
                        email: getCurrentEmail()
                    }
                });
                document.dispatchEvent(invoiceEvent);
                
                if (btnDownloadInvoice) {
                    btnDownloadInvoice.disabled = !currentInvoiceFileName;
                }
                
                if (data.allInvoices && data.allInvoices.length > 0) {
                    updateBillGraph(data.allInvoices);
                }
            } else {
                if (currentInvoiceNumberEl) currentInvoiceNumberEl.textContent = 'Invoice: ---';
                if (currentInvoiceDueDateEl) currentInvoiceDueDateEl.textContent = 'Due: ---';
                if (currentInvoiceAmountEl) currentInvoiceAmountEl.textContent = 'Amount: $0.00';
                if (currentInvoiceStatusEl) currentInvoiceStatusEl.innerHTML = '';
            }

            loadTransactionHistory(selectBan.value);
        }

        /**
         * Load transaction history for selected BAN
         */
        function loadTransactionHistory(banNumber) {
            fetch('/api/MyBillDashboard/GetTransactionHistory', {
                method: 'POST',
                body: JSON.stringify({ banNumber: banNumber }),
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include'
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(res => {
                if (res && (res.statusCode === 200 || (res.statusCode >= 200 && res.statusCode < 400)) && res.data) {
                    const transactions = res.data.transactions || [];
                    renderTransactionHistory(transactions, true);
                    currentPage = 1;
                    renderPaginatedTransactions();
                } else {
                    transactionTableBody.innerHTML = '<tr><td colspan="6" class="text-center">No transactions found</td></tr>';
                    updatePaginationControls();
                }
            })
            .catch(err => {
                transactionTableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Error loading transactions: ' + err.message + '</td></tr>';
                updatePaginationControls();
            });
        }

        /**
         * Render transaction history table
         */
        function renderTransactionHistory(transactions, storeOriginal = false) {
            if (storeOriginal) {
                originalTransactions = [...transactions];
                updateTotalPendingAmount(transactions);
            }
            filteredTransactions = [...transactions];
        }

        /**
         * Calculate and update the Total Pending Amount card
         */
        function updateTotalPendingAmount(transactions) {
            const payNowHintEl = document.getElementById('payNowHint');
            
            if (!transactions || transactions.length === 0) {
                if (pendingInvoiceCount) pendingInvoiceCount.textContent = 'Pending Invoices: 0';
                if (latestDueDate) latestDueDate.textContent = 'Earliest Due: ---';
                if (totalPendingAmount) totalPendingAmount.textContent = 'Total: $0.00';
                if (payNowHintEl) payNowHintEl.style.display = 'none';
                return;
            }
            
            const pendingTransactions = transactions.filter(t => t.status === 'Pending');
            
            if (pendingTransactions.length === 0) {
                if (pendingInvoiceCount) pendingInvoiceCount.textContent = 'Pending Invoices: 0';
                if (latestDueDate) latestDueDate.textContent = 'Earliest Due: ---';
                if (totalPendingAmount) totalPendingAmount.textContent = 'Total: $0.00';
                if (payNowHintEl) payNowHintEl.style.display = 'none';
                return;
            }
            
            const total = pendingTransactions.reduce((sum, t) => sum + (parseFloat(t.amount) || 0), 0);
            
            let earliestDueDate = null;
            pendingTransactions.forEach(t => {
                if (t.dueDate) {
                    const d = new Date(t.dueDate);
                    if (!earliestDueDate || d < earliestDueDate) {
                        earliestDueDate = d;
                    }
                }
            });
            
            if (pendingInvoiceCount) pendingInvoiceCount.textContent = `Pending Invoices: ${pendingTransactions.length}`;
            if (latestDueDate) latestDueDate.textContent = earliestDueDate ? `Earliest Due: ${formatDate(earliestDueDate)}` : 'Earliest Due: ---';
            if (totalPendingAmount) totalPendingAmount.textContent = `Total: $${formatCurrency(total)}`;
            
            if (payNowHintEl) {
                payNowHintEl.style.display = total > 0 ? '' : 'none';
            }
        }

        /**
         * Render paginated transactions
         */
        function renderPaginatedTransactions() {
            if (filteredTransactions.length === 0) {
                transactionTableBody.innerHTML = '<tr><td colspan="6" class="text-center">No transactions found</td></tr>';
                updatePaginationControls();
                return;
            }

            const startIndex = (currentPage - 1) * recordsPerPage;
            const endIndex = startIndex + recordsPerPage;
            const pageTransactions = filteredTransactions.slice(startIndex, endIndex);

            transactionTableBody.innerHTML = '';

            pageTransactions.forEach((transaction) => {
                const row = document.createElement('tr');
                
                let statusDisplay;
                let actionButton;
                
                if (transaction.status === 'Pending') {
                    statusDisplay = `<button class="btn btn-sm btn-danger" 
                                           data-action="pay-invoice" 
                                           data-invoice-number="${transaction.invoiceNumber}" 
                                           data-amount="${transaction.amount}" 
                                           data-ban="${selectBan.value}" 
                                           data-email="${getCurrentEmail()}" 
                                           data-payment-type="full"
                                           title="Pay Invoice ${transaction.invoiceNumber}">
                           <i class="ri-secure-payment-line me-1"></i> Pay Now
                       </button>`;
                    
                    const hasFile = transaction.fileName && transaction.fileName !== null && transaction.fileName !== '';
                    actionButton = hasFile 
                        ? `<button class="btn btn-sm btn-primary-light invoice-download-btn" data-filename="${transaction.fileName}"><i class="ri-download-2-line"></i> Download</button>`
                        : `<span class="text-muted">Not available</span>`;
                } else if (transaction.status === 'Paid') {
                    statusDisplay = `<span class="badge bg-success">Paid</span>`;
                    const hasFile = transaction.fileName && transaction.fileName !== null && transaction.fileName !== '';
                    actionButton = hasFile 
                        ? `<button class="btn btn-sm btn-primary-light invoice-download-btn" data-filename="${transaction.fileName}"><i class="ri-download-2-line"></i> Download</button>`
                        : `<span class="text-muted">Not available</span>`;
                } else {
                    statusDisplay = `<span class="badge bg-secondary">${transaction.status}</span>`;
                    actionButton = `<span class="text-muted">Not available</span>`;
                }
                
                row.innerHTML = `
                    <td>${transaction.invoiceNumber}</td>
                    <td>${transaction.description}</td>
                    <td>$${formatCurrency(transaction.amount)}</td>
                    <td>${formatDate(transaction.dueDate)}</td>
                    <td>${statusDisplay}</td>
                    <td>${actionButton}</td>
                `;
                
                transactionTableBody.appendChild(row);
            });

            updatePaginationControls();
        }

        /**
         * Update pagination controls
         */
        function updatePaginationControls() {
            const totalRecords = filteredTransactions.length;
            const totalPages = Math.ceil(totalRecords / recordsPerPage);
            const startRecord = totalRecords > 0 ? ((currentPage - 1) * recordsPerPage) + 1 : 0;
            const endRecord = Math.min(currentPage * recordsPerPage, totalRecords);

            const pageStartEl = document.getElementById('pageStart');
            const pageEndEl = document.getElementById('pageEnd');
            const totalRecordsEl = document.getElementById('totalRecords');
            
            if (pageStartEl) pageStartEl.textContent = startRecord;
            if (pageEndEl) pageEndEl.textContent = endRecord;
            if (totalRecordsEl) totalRecordsEl.textContent = totalRecords;

            const prevButton = document.getElementById('prevPage');
            const nextButton = document.getElementById('nextPage');
            
            if (prevButton) {
                prevButton.classList.toggle('disabled', currentPage === 1 || totalRecords === 0);
            }
            if (nextButton) {
                nextButton.classList.toggle('disabled', currentPage >= totalPages || totalRecords === 0);
            }
        }

        /**
         * Filter transactions by search term and status
         */
        function filterTransactions(searchTerm, statusFilter) {
            let filtered = [...originalTransactions];
            
            if (statusFilter && statusFilter !== 'All') {
                filtered = filtered.filter(t => t.status === statusFilter);
            }
            
            if (searchTerm && searchTerm.length > 0) {
                const term = searchTerm.toLowerCase();
                filtered = filtered.filter(t => {
                    return (
                        t.invoiceNumber.toLowerCase().includes(term) ||
                        t.description.toLowerCase().includes(term) ||
                        t.status.toLowerCase().includes(term) ||
                        formatDate(t.date).toLowerCase().includes(term) ||
                        t.amount.toString().includes(term)
                    );
                });
            }
            
            filteredTransactions = filtered;
            currentPage = 1;
            renderPaginatedTransactions();
        }

        /**
         * Setup search and sort event listeners
         */
        function setupSearchAndSort() {
            if (searchTransactions) {
                searchTransactions.addEventListener('input', function() {
                    filterTransactions(this.value.trim(), currentFilter);
                });
            }
            
            const sortItems = document.querySelectorAll('.dropdown-menu .dropdown-item');
            sortItems.forEach(item => {
                item.addEventListener('click', function(e) {
                    e.preventDefault();
                    currentFilter = this.textContent.trim();
                    const searchTerm = searchTransactions ? searchTransactions.value.trim() : '';
                    filterTransactions(searchTerm, currentFilter);
                });
            });
            
            const prevButton = document.getElementById('prevPage');
            const nextButton = document.getElementById('nextPage');
            
            if (prevButton) {
                prevButton.addEventListener('click', function(e) {
                    e.preventDefault();
                    if (!this.classList.contains('disabled') && currentPage > 1) {
                        currentPage--;
                        renderPaginatedTransactions();
                    }
                });
            }
            
            if (nextButton) {
                nextButton.addEventListener('click', function(e) {
                    e.preventDefault();
                    const totalPages = Math.ceil(filteredTransactions.length / recordsPerPage);
                    if (!this.classList.contains('disabled') && currentPage < totalPages) {
                        currentPage++;
                        renderPaginatedTransactions();
                    }
                });
            }
            
            if (transactionTableBody) {
                transactionTableBody.addEventListener('click', function(e) {
                    const downloadBtn = e.target.closest('.invoice-download-btn');
                    if (downloadBtn) {
                        e.preventDefault();
                        const fileName = downloadBtn.getAttribute('data-filename');
                        if (fileName) {
                            downloadInvoice(fileName);
                        }
                    }
                });
            }
        }

        /**
         * Event listener for BAN selection change
         */
        selectBan.addEventListener('change', function() {
            const selectedBan = selectBan.value;
            const selectedOption = selectBan.options[selectBan.selectedIndex];
            const accountName = selectedOption.getAttribute('data-account-name') || 'Account';
            
            companyName.textContent = accountName;
            
            const banChangedEvent = new CustomEvent('mybill:banChanged', {
                detail: { ban: selectedBan, email: getCurrentEmail() }
            });
            document.dispatchEvent(banChangedEvent);
            
            fetch(`/api/MyBillDashboard/GetAllAccountsByPrimary?selectedBanNumber=${encodeURIComponent(selectedBan)}`, {
                method: 'GET',
                credentials: 'include'
            })
            .then(response => response.json())
            .then(res => {
                if (res && res.data) {
                    const banAccounts = res.data?.BanAccounts || res.data?.banAccounts || [];
                    const selectedBanData = banAccounts.find(ban => 
                        (ban.Number || ban.number) === selectedBan
                    );
                    
                    if (selectedBanData) {
                        const serverAccountName = selectedBanData.Name || selectedBanData.name || 
                                                 selectedBanData.AccountName || selectedBanData.accountName || accountName;
                        companyName.textContent = serverAccountName;
                        loadPaperlessEmails(selectedBanData);
                    }
                }
            })
            .catch(err => error('Error reloading account data:', err));

            loadAccountSummary(selectedBan);
        });

        /**
         * Event listener for download invoice button
         */
        if (btnDownloadInvoice) {
            btnDownloadInvoice.addEventListener('click', function() {
                if (currentInvoiceFileName) {
                    downloadInvoice(currentInvoiceFileName);
                } else {
                    showError('Invoice file not available for download');
                }
            });
        }

        /**
         * Utility: Format date to user's local time
         */
        function formatDate(dateString) {
            if (!dateString) return '---';
            try {
                const date = new Date(dateString);
                if (isNaN(date.getTime())) return '---';
                return date.toLocaleDateString(navigator.language || 'en-GB', { 
                    day: 'numeric', month: 'short', year: 'numeric'
                });
            } catch (e) {
                return '---';
            }
        }

        /**
         * Utility: Format currency
         */
        function formatCurrency(amount) {
            if (!amount && amount !== 0) return '0.00';
            return parseFloat(amount).toFixed(2);
        }

        /**
         * Utility: Show loader
         */
        function showLoader() {
            if (loader) loader.classList.remove('d-none');
        }

        /**
         * Utility: Hide loader
         */
        function hideLoader() {
            if (loader) loader.classList.add('d-none');
        }

        /**
         * Utility: Show error message
         */
        function showError(message) {
            if (typeof Swal !== 'undefined') {
                Swal.fire({ title: 'Error', text: message, icon: 'error', confirmButtonText: 'OK' });
            } else {
                alert(message);
            }
        }

        /**
         * Utility: Show success message
         */
        function showSuccess(message) {
            if (typeof Swal !== 'undefined') {
                Swal.fire({ title: 'Success', text: message, icon: 'success', confirmButtonText: 'OK' });
            } else {
                alert(message);
            }
        }

        /**
         * Function for downloading invoice
         */
        function downloadInvoice(fileName) {
            if (!fileName || fileName === null || fileName === '') {
                showError('Invoice file not available');
                return;
            }
            window.location.href = `/api/MyBillDashboard/DownloadInvoice?fileName=${encodeURIComponent(fileName)}`;
        }

        /**
         * Initialize bill details graph (ApexCharts)
         */
        let billChart = null;
        let pendingInvoiceData = null;
        
        function initializeBillGraph() {
            if (typeof ApexCharts === 'undefined') return;
            
            const graphElement = document.querySelector("#audienceReport");
            if (!graphElement) return;

            const options = {
                series: [{ name: 'Amount', data: [] }],
                chart: { type: 'bar', height: 230, toolbar: { show: false } },
                grid: { borderColor: '#f1f1f1', strokeDashArray: 3 },
                colors: ["#845adf"],
                plotOptions: { bar: { horizontal: false, columnWidth: '50%', endingShape: 'rounded' } },
                dataLabels: { enabled: false },
                stroke: { show: true, width: 2, colors: ['transparent'] },
                xaxis: { categories: [], labels: { style: { colors: "#8c9097", fontSize: '11px', fontWeight: 600 } } },
                yaxis: {
                    title: { text: 'Amount ($)', style: { color: "#8c9097" } },
                    labels: { style: { colors: "#8c9097", fontSize: '11px', fontWeight: 600 }, formatter: (v) => "$" + formatCurrency(v) }
                },
                fill: { opacity: 1 },
                tooltip: { y: { formatter: (val) => "$" + formatCurrency(val) } }
            };

            try {
                billChart = new ApexCharts(graphElement, options);
                billChart.render();
                if (pendingInvoiceData && pendingInvoiceData.length > 0) {
                    updateBillGraph(pendingInvoiceData);
                    pendingInvoiceData = null;
                }
            } catch (e) {
                error('Error rendering graph:', e);
            }
        }
        
        /**
         * Update graph with real invoice data
         */
        function updateBillGraph(invoices) {
            if (!invoices || invoices.length === 0) return;
            if (!billChart) {
                pendingInvoiceData = invoices;
                return;
            }
            
            const recentInvoices = invoices.slice(0, 6).reverse();
            const categories = [];
            const amounts = [];
            
            recentInvoices.forEach(invoice => {
                const dateStr = invoice.billingCycleStartDate || invoice.billingCycleEndDate;
                if (dateStr) {
                    const date = new Date(dateStr);
                    categories.push(date.toLocaleDateString('en-US', { month: 'short', year: 'numeric' }));
                } else {
                    categories.push('N/A');
                }
                amounts.push(invoice.amount || 0);
            });
            
            billChart.updateOptions({
                xaxis: { categories: categories, labels: { style: { colors: "#8c9097", fontSize: '11px', fontWeight: 600 } } }
            });
            billChart.updateSeries([{ name: 'Amount', data: amounts }]);
        }

        function ensureGraphInitialized() {
            if (typeof ApexCharts !== 'undefined' && document.querySelector("#audienceReport")) {
                initializeBillGraph();
            } else {
                setTimeout(ensureGraphInitialized, 500);
            }
        }
        
        ensureGraphInitialized();
        setupSearchAndSort();

        // mybill-auth-refresh.js intercepts ALL /api/MyBill* fetch calls.
        // If the JWT is expired (401), it automatically refreshes via the
        // backend refresh token (7-day cookie) and retries the request.
        // No eager refresh needed here — just load the dashboard directly.
        initializeDashboard();
    });
})();
