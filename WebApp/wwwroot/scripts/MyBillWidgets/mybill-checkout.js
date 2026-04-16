/**
 * MyBill Checkout JavaScript
 * Follows the same pattern as the working WebTopUp Checkout.js:
 *   1. Payment callback ? store raw API response in localStorage ? redirect to ?result=success/fail
 *   2. Dedicated success/fail JS files read from localStorage and populate the DOM
 */
(function () {
    document.addEventListener("DOMContentLoaded", function () {

        // =====================================================
        // PAYMENT CALLBACK HANDLER — runs before anything else
        // Matches pattern from working Checkout.js
        // =====================================================
        if (
            window.location.pathname.toLowerCase().includes("checkout") &&
            window.location.search.length > 0 &&
            !new URLSearchParams(window.location.search).has("sid") &&
            !new URLSearchParams(window.location.search).has("result")
        ) {
            var params = new URLSearchParams(window.location.search);
            var paymentMethod = localStorage.getItem("mybill_payment_method");

            if (!paymentMethod) {
                if (params.has("rID") || params.has("tID") || params.has("token")) paymentMethod = "MPAISA";
                else if (params.has("sessionId")) paymentMethod = "CARD";
            }

            var provideUpdateRequest = {
                mPaisaPayment: null,
                creditCardPayment: null
            };

            // First get payment context (invoice/BAN/email) from cookie
            (async function confirmPayment() {
                try {
                    // Get payment context from encrypted cookie
                    var contextRes = await fetch("/api/mybill-checkout/get-payment-context", {
                        method: "GET",
                        credentials: "include"
                    });
                    var context = await contextRes.json();

                    if (!context.isSuccess) {
                        window.location.href = "/mybill/mybill-dashboard";
                        return;
                    }

                    // Add invoice/BAN to the request
                    provideUpdateRequest.invoiceNumber = context.invoiceNumber;
                    provideUpdateRequest.selectedBAN = context.banNumber;

                    if (paymentMethod === "MPAISA") {
                        provideUpdateRequest.mPaisaPayment = {
                            rCode: params.get("rCode"),
                            redirectUrl: window.location.href,
                            requestId: params.get("rID"),
                            tokenv2: params.get("tokenv2"),
                            transactionId: params.get("tID"),
                            customerPhoneNumber: params.get("customerphonenumber"),
                            token: params.get("token")
                        };
                    }
                    if (paymentMethod === "CARD") {
                        provideUpdateRequest.creditCardPayment = {
                            sessionId: params.get("sessionId")
                        };
                    }

                    var res = await fetch("/api/mybill-checkout/provide-update", {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        credentials: "include",
                        body: JSON.stringify(provideUpdateRequest)
                    });

                    var result = await res.json();
                    var d = result.data;

                    // Store raw API response in localStorage — same pattern as working Checkout.js
                    localStorage.setItem("mybill_payment_result", JSON.stringify(d));

                    // Clean URL and redirect to result page
                    localStorage.removeItem("mybill_payment_method");

                    if (d && d.isSuccessful) {
                        window.location.href = window.location.pathname + "?result=success";
                    } else {
                        window.location.href = window.location.pathname + "?result=fail";
                    }
                } catch (err) {
                    Swal.fire({
                        title: "Payment Confirmation Failed",
                        text: "We were unable to confirm your payment. Please check your dashboard.",
                        icon: "error",
                        confirmButtonText: "OK"
                    });
                }
            })();

            return; // Don't initialize checkout form
        }

        // =====================================================
        // If on result page, stop — the dedicated JS files handle it
        // =====================================================
        if (new URLSearchParams(window.location.search).has("result")) {
            return;
        }

        // =====================================================
        // NORMAL CHECKOUT FORM INITIALIZATION
        // =====================================================

        // Load country codes
        async function loadCountries() {
            try {
                var response = await fetch("/scripts/WebTopUpWidget/countrycode.json");
                if (!response.ok) return;
                var countries = await response.json();
                var select = document.getElementById("inputCountry");
                if (!select) return;
                countries.forEach(function (country) {
                    var option = document.createElement("option");
                    option.value = country.code;
                    option.textContent = country.name;
                    select.appendChild(option);
                });
            } catch (error) { }
        }
        loadCountries();

        // Payment method radios
        var paymentRadios = document.querySelectorAll("input[name='paymentMethod']");
        var billingSection = document.getElementById("billingAddressSection");
        var paymentOptionCards = document.querySelectorAll(".payment-option-card");

        // Partial payment elements
        var chkPartialPayment = document.getElementById("chkPartialPayment");
        var partialAmountSection = document.getElementById("partialAmountSection");
        var txtPartialAmount = document.getElementById("txtPartialAmount");
        var summaryAmount = document.getElementById("summaryAmount");
        var totalPayAmount = document.getElementById("totalPayAmount");

        // Hidden field values
        var hdnTotalAmount = parseFloat(document.getElementById("hdnTotalAmount")?.value || 0);

        // Buttons
        var placeOrderBtn = document.getElementById("placeOrderBtn");
        var checkoutBtn = document.getElementById("checkoutBtn");

        // Payment method selection
        if (billingSection) billingSection.style.display = "none";

        function updatePaymentCardSelection(selectedRadio) {
            paymentOptionCards.forEach(function (card) { card.classList.remove("selected"); });
            if (selectedRadio) {
                var parentCard = selectedRadio.closest(".payment-option-card");
                if (parentCard) parentCard.classList.add("selected");
            }
        }

        function resetBillingForm() {
            document.querySelectorAll(".required-field").forEach(function (field) {
                field.classList.remove("is-invalid");
                if (field.tagName === "SELECT") {
                    field.selectedIndex = 0;
                } else {
                    field.value = "";
                }
                var feedback = field.nextElementSibling;
                if (feedback && feedback.classList.contains("invalid-feedback")) feedback.textContent = "";
            });
            var address2 = document.querySelector("input[name='Address2']");
            if (address2) address2.value = "";
        }

        paymentRadios.forEach(function (radio) {
            radio.addEventListener("change", function () {
                billingSection.style.display = this.value === "CARD" ? "block" : "none";
                resetBillingForm();
                updatePaymentCardSelection(this);
            });
        });

        paymentOptionCards.forEach(function (card) {
            card.addEventListener("click", function () {
                var radio = this.querySelector("input[type='radio']");
                if (radio) {
                    radio.checked = true;
                    radio.dispatchEvent(new Event("change", { bubbles: true }));
                }
            });
        });

        // Partial payment handling
        if (chkPartialPayment) {
            chkPartialPayment.addEventListener("change", function () {
                if (this.checked) {
                    partialAmountSection.style.display = "block";
                    if (txtPartialAmount) {
                        txtPartialAmount.focus();
                        var partialValue = parseFloat(txtPartialAmount.value);
                        if (partialValue && partialValue > 0 && partialValue <= hdnTotalAmount) {
                            updatePaymentAmount(partialValue);
                        }
                    }
                } else {
                    partialAmountSection.style.display = "none";
                    updatePaymentAmount(hdnTotalAmount);
                    if (txtPartialAmount) txtPartialAmount.value = hdnTotalAmount.toFixed(2);
                }
            });
        }

        if (txtPartialAmount) {
            txtPartialAmount.addEventListener("input", function () {
                var value = parseFloat(this.value);
                this.classList.remove("is-invalid");
                var errorElement = document.getElementById("partialAmountError");
                if (errorElement) errorElement.textContent = "";
                if (!value || value <= 0) {
                    this.classList.add("is-invalid");
                    if (errorElement) errorElement.textContent = "Amount must be greater than $0.00";
                    return;
                }
                if (value < 10) {
                    this.classList.add("is-invalid");
                    if (errorElement) errorElement.textContent = "Minimum payment is $10.00";
                    return;
                }
                if (value > hdnTotalAmount) {
                    this.classList.add("is-invalid");
                    if (errorElement) errorElement.textContent = "Maximum payment is $" + hdnTotalAmount.toFixed(2);
                    return;
                }
                updatePaymentAmount(value);
            });
        }

        function updatePaymentAmount(amount) {
            var formatted = "$" + amount.toFixed(2);
            if (summaryAmount) summaryAmount.textContent = formatted;
            if (totalPayAmount) totalPayAmount.textContent = formatted;
        }

        function validateTermsAndConditions() {
            var terms = document.getElementById("invalidCheck");
            var termsError = document.getElementById("termsError");
            if (terms && !terms.checked) {
                terms.classList.add("is-invalid");
                if (termsError) termsError.style.display = "block";
                return false;
            } else if (terms) {
                terms.classList.remove("is-invalid");
                if (termsError) termsError.style.display = "none";
            }
            return true;
        }

        function validateBillingForm() {
            var isValid = true;
            document.querySelectorAll(".required-field").forEach(function (field) {
                var label = field.dataset.label || "This field";
                field.classList.remove("is-invalid");
                var feedback = field.nextElementSibling;
                if (feedback && feedback.classList.contains("invalid-feedback")) feedback.textContent = "";
                if (!field.value || field.value.trim() === "" || field.value === "Choose...") {
                    field.classList.add("is-invalid");
                    if (feedback && feedback.classList.contains("invalid-feedback")) feedback.textContent = label + " is required";
                    isValid = false;
                }
                if (field.name === "BillingEmail" && field.value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(field.value)) {
                    field.classList.add("is-invalid");
                    if (feedback && feedback.classList.contains("invalid-feedback")) feedback.textContent = "Invalid email format";
                    isValid = false;
                }
            });
            return isValid;
        }

        function validatePartialPayment() {
            if (!chkPartialPayment || !chkPartialPayment.checked) return true;
            if (!txtPartialAmount) return false;
            var value = parseFloat(txtPartialAmount.value);
            var errorElement = document.getElementById("partialAmountError");
            if (!value || value < 10) {
                txtPartialAmount.classList.add("is-invalid");
                if (errorElement) errorElement.textContent = "Minimum payment is $10.00";
                return false;
            }
            if (value > hdnTotalAmount) {
                txtPartialAmount.classList.add("is-invalid");
                if (errorElement) errorElement.textContent = "Maximum payment is $" + hdnTotalAmount.toFixed(2);
                return false;
            }
            return true;
        }

        // Place Order Button
        if (placeOrderBtn) {
            placeOrderBtn.addEventListener("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                var paymentMethod = document.querySelector("input[name='paymentMethod']:checked")?.value;
                if (!paymentMethod) {
                    Swal.fire({ icon: "warning", title: "Select payment method", text: "Please choose a payment method before continuing." });
                    return;
                }
                if (!validatePartialPayment()) {
                    Swal.fire({ icon: "warning", title: "Invalid payment amount", text: "Please enter a valid payment amount between $10.00 and $" + hdnTotalAmount.toFixed(2) });
                    return;
                }
                if (!validateTermsAndConditions()) {
                    Swal.fire({ icon: "warning", title: "Terms and Conditions Required", text: "Please agree to the terms and conditions before proceeding." });
                    return;
                }
                if (paymentMethod === "CARD" && !validateBillingForm()) return;

                var paymentAmount = chkPartialPayment && chkPartialPayment.checked ? parseFloat(txtPartialAmount.value) : hdnTotalAmount;
                var confirmInvoice = document.getElementById("confirmInvoice");
                var confirmBAN = document.getElementById("confirmBAN");
                var confirmAmount = document.getElementById("confirmAmount");
                var confirmPayment = document.getElementById("confirmPayment");
                if (confirmInvoice) confirmInvoice.textContent = document.getElementById("hdnInvoiceNumber")?.value || "-";
                if (confirmBAN) confirmBAN.textContent = document.getElementById("hdnBanNumber")?.value || "-";
                if (confirmAmount) confirmAmount.textContent = "$" + paymentAmount.toFixed(2);
                if (confirmPayment) confirmPayment.textContent = paymentMethod || "-";
                try {
                    var modalElement = document.getElementById("exampleModalScrollable2");
                    if (modalElement) new bootstrap.Modal(modalElement).show();
                } catch (error) { }
            });
        }

        // Checkout Button
        if (checkoutBtn) {
            checkoutBtn.addEventListener("click", async function () {
                var form = this.closest("form");
                if (!form) return;
                var formData = new FormData(form);
                if (chkPartialPayment && chkPartialPayment.checked && txtPartialAmount) {
                    formData.set("PaymentAmount", parseFloat(txtPartialAmount.value).toFixed(2));
                    formData.set("IsPartialPayment", "true");
                } else {
                    formData.set("PaymentAmount", hdnTotalAmount.toFixed(2));
                    formData.set("IsPartialPayment", "false");
                }
                try {
                    var response = await fetch("/api/mybill-checkout/submit", {
                        method: "POST",
                        headers: { "RequestVerificationToken": form.querySelector('input[name="__RequestVerificationToken"]').value },
                        body: formData
                    });
                    var result;
                    try { result = await response.json(); } catch {
                        Swal.fire({ icon: "error", title: "Error", text: "Invalid server response." });
                        return;
                    }
                    if (!response.ok || !result.isSuccess) {
                        Swal.fire({ icon: "error", title: "Payment Failed", text: result?.message || "Payment failed" });
                        return;
                    }
                    var paymentMethod = document.querySelector("input[name='paymentMethod']:checked")?.value;
                    localStorage.setItem("mybill_payment_method", paymentMethod);
                    if (result.data?.url) {
                        window.location.href = result.data.url;
                    } else {
                        Swal.fire({ icon: "error", title: "Error", text: "No redirect URL received." });
                    }
                } catch (error) {
                    Swal.fire({ icon: "error", title: "Error", text: "An error occurred while processing your payment." });
                }
            });
        }
    });
})();



