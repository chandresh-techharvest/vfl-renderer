/**
 * MyBill Payment JavaScript
 * Handles payment initiation from dashboard (Pay Now button)
 * Creates checkout session and redirects to checkout page
 */

(function() {
    'use strict';

    const log = Function.prototype;
    const error = Function.prototype;

    // Configuration
    let checkoutPageUrl = window.mybillCheckoutPageUrl || '/mybill/checkout';
    let currentInvoice = null;
    let currentBan = null;
    let currentEmail = null;

    log('MyBill Payment: Script loaded, checkout URL:', checkoutPageUrl);

    /**
     * Initialize payment handlers
     */
    function init() {
        log('MyBill Payment: Initializing payment handlers...');

        // Listen for "Pay Now" button clicks
        document.addEventListener('click', handlePayNowClick);

        // Listen for custom events from dashboard
        document.addEventListener('mybill:invoiceLoaded', handleInvoiceLoaded);
        document.addEventListener('mybill:banChanged', handleBanChanged);
    }

    /**
     * Handle invoice data loaded
     */
    function handleInvoiceLoaded(event) {
        if (event.detail) {
            currentInvoice = event.detail.invoice;
            currentBan = event.detail.ban;
            currentEmail = event.detail.email;
            
            log('MyBill Payment: Invoice loaded');
        }
    }

    /**
     * Handle BAN account change
     */
    function handleBanChanged(event) {
        if (event.detail) {
            currentBan = event.detail.ban;
            currentEmail = event.detail.email;
            
            log('MyBill Payment: BAN changed');
        }
    }

    /**
     * Handle "Pay Now" button clicks
     */
    function handlePayNowClick(event) {
        // Check if clicked element is a Pay Now button
        const payButton = event.target.closest('[data-action="pay-invoice"]');
        if (!payButton) return;

        event.preventDefault();
        log('MyBill Payment: Pay Now clicked');
        
        // Get payment data from button attributes or current invoice
        const invoiceNumber = payButton.dataset.invoiceNumber || currentInvoice?.invoiceNumber;
        const totalAmount = parseFloat(payButton.dataset.amount || currentInvoice?.totalAmount || 0);
        const banNumber = payButton.dataset.ban || currentBan;
        const email = payButton.dataset.email || currentEmail;
       
        // Check if we have required data
        if (!invoiceNumber || !banNumber || !email || !totalAmount) {
            // Log which specific data is missing
            const missingData = [];
            if (!invoiceNumber) missingData.push('invoiceNumber');
            if (!banNumber) missingData.push('banNumber');
            if (!email) missingData.push('email');
            if (!totalAmount) missingData.push('totalAmount');
            
            showAlert(`Unable to process payment. Missing: ${missingData.join(', ')}`, 'danger');
            return;
        }

        // Check payment type (full or partial)
        const paymentType = payButton.dataset.paymentType || 'full';
        let paymentAmount = totalAmount;
        let isPartialPayment = false;

        if (paymentType === 'partial') {
            const partialAmountInput = document.getElementById('partialPaymentAmount');
            if (partialAmountInput) {
                paymentAmount = parseFloat(partialAmountInput.value);
                isPartialPayment = true;

                if (isNaN(paymentAmount) || paymentAmount <= 0) {
                    showAlert('Please enter a valid payment amount', 'danger');
                    return;
                }

                if (paymentAmount < 10) {
                    showAlert('Minimum payment amount is $10', 'danger');
                    return;
                }

                if (paymentAmount > totalAmount) {
                    showAlert('Payment amount cannot exceed total invoice amount', 'danger');
                    return;
                }
            }
        }

        log('MyBill Payment: Initiating payment');

        // Create checkout session
        createCheckoutSession({
            email: email,
            banNumber: banNumber,
            invoiceNumber: invoiceNumber,
            fullAmount: totalAmount,
            paymentAmount: paymentAmount,
            isPartialPayment: isPartialPayment
        });
    }

    /**
     * Create checkout session via API
     */
    async function createCheckoutSession(paymentData) {
        log('MyBill Payment: Creating checkout session...');

        // Show loading indicator
        showLoadingOverlay('Creating secure checkout session...');

        try {
            const response = await fetch('/api/mybill-checkout/create-session', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: paymentData.email,
                    banNumber: paymentData.banNumber,
                    invoiceNumber: paymentData.invoiceNumber,
                    fullAmount: paymentData.fullAmount,
                    paymentAmount: paymentData.paymentAmount,
                    isPartialPayment: paymentData.isPartialPayment,
                    pageUrl: checkoutPageUrl
                })
            });

            const result = await response.json();

            if (response.ok && result.sessionId) {
                log('MyBill Payment: Checkout session created:', result.sessionId);
                
                // Redirect to checkout page with session ID
                const checkoutUrl = `${checkoutPageUrl}?sid=${result.sessionId}`;
                log('MyBill Payment: Redirecting to checkout');
                
                window.location.href = checkoutUrl;
            } else {
                error('MyBill Payment: Failed to create checkout session');
                hideLoadingOverlay();
                
                const errorMessage = result.message || 'Failed to create checkout session. Please try again.';
                showAlert(errorMessage, 'danger');
            }
        } catch (err) {
            error('MyBill Payment: Error creating checkout session:', err);
            hideLoadingOverlay();
            showAlert('An error occurred. Please try again.', 'danger');
        }
    }

    /**
     * Show loading overlay
     */
    function showLoadingOverlay(message) {
        let overlay = document.getElementById('mybillLoadingOverlay');
        
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'mybillLoadingOverlay';
            overlay.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0, 0, 0, 0.7);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 9999;
            `;
            overlay.innerHTML = `
                <div class="text-center text-white">
                    <div class="spinner-border mb-3" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <div class="fs-5" id="loadingMessage">${message}</div>
                </div>
            `;
            document.body.appendChild(overlay);
        } else {
            overlay.style.display = 'flex';
            const messageDiv = overlay.querySelector('#loadingMessage');
            if (messageDiv) {
                messageDiv.textContent = message;
            }
        }
    }

    /**
     * Hide loading overlay
     */
    function hideLoadingOverlay() {
        const overlay = document.getElementById('mybillLoadingOverlay');
        if (overlay) {
            overlay.style.display = 'none';
        }
    }

    /**
     * Show alert message
     */
    function showAlert(message, type) {
        let alertContainer = document.getElementById('dashboardAlertContainer');
        
        if (!alertContainer) {
            alertContainer = document.createElement('div');
            alertContainer.id = 'dashboardAlertContainer';
            alertContainer.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 9998; max-width: 400px;';
            document.body.appendChild(alertContainer);
        }

        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show shadow-lg`;
        alertDiv.role = 'alert';
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;

        alertContainer.appendChild(alertDiv);

        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            alertDiv.classList.remove('show');
            setTimeout(() => alertDiv.remove(), 150);
        }, 5000);
    }

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Export functions for external use
    window.MyBillPayment = {
        createCheckoutSession: createCheckoutSession,
        setCheckoutUrl: function(url) {
            checkoutPageUrl = url;
            log('MyBill Payment: Checkout URL updated');
        }
    };

})();
