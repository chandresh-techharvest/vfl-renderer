(function () {
    document.addEventListener("DOMContentLoaded", function () {
        // Utility: Fetch decrypted cookie
        async function getDecryptedCookie() {
            try {
                const response = await fetch('/api/Common/GetCookie', { method: "GET", credentials: "include" });
                if (!response.ok) return null;
                return await response.json();
            } catch (error) {
                console.error("Error fetching decrypted cookie:", error);
                return null;
            }
        }


        function getPlanCodeByPayment(planCodes, paymentMethod) {
            if (!planCodes || planCodes.length === 0) return null;

            let targetMethod = "";

            if (paymentMethod === "MPAISA") {
                targetMethod = "MPAISA";
            }
            else if (paymentMethod === "CARD") {
                targetMethod = "CREDIT"; // 🔥 important mapping
            }
            else {
                return null;
            }

            const selected = planCodes.find(p => p.paymentMethod === targetMethod);

            return selected ? selected.pcode : null;
        }




        // Remove payment query params from URL
        function clearPaymentQueryParams() {
            const cleanUrl = window.location.origin + window.location.pathname;
            window.history.replaceState({}, document.title, cleanUrl);
        }

        // Back button
        document.getElementById("backBtn")?.addEventListener("click", function () {
            window.history.back();
        });

        // Load country codes
        async function loadCountries() {
            try {
                const response = await fetch("/scripts/WebTopUpWidget/countrycode.json");
                if (!response.ok) throw new Error("Failed to load country list");
                const countries = await response.json();
                const select = document.getElementById("inputCountry");
                if (!select) return;
                countries.forEach(country => {
                    const option = document.createElement("option");
                    option.value = country.code;
                    option.textContent = country.name;
                    select.appendChild(option);
                });
            } catch (error) {
                console.error("Country load error:", error);
            }
        }
        loadCountries();

        // Payment form validation
        function validateBillingForm() {
            let isValid = true;
            document.querySelectorAll(".required-field").forEach(field => {
                const label = field.dataset.label || "This field";
                field.classList.remove("is-invalid");
                const feedback = field.nextElementSibling;
                if (feedback) feedback.textContent = "";
                if (!field.value || field.value.trim() === "" || field.value === "Choose...") {
                    field.classList.add("is-invalid");
                    if (feedback) feedback.textContent = `${label} is required`;
                    isValid = false;
                }
                if (field.name === "BillingEmail" && field.value) {
                    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(field.value)) {
                        field.classList.add("is-invalid");
                        if (feedback) feedback.textContent = "Invalid email format";
                        isValid = false;
                    }
                }
            });
            const terms = document.getElementById("invalidCheck");
            if (terms && !terms.checked) {
                terms.classList.add("is-invalid");
                isValid = false;
            } else if (terms) {
                terms.classList.remove("is-invalid");
            }
            return isValid;
        }

        // Format date
        function formatShortDate(dateValue) {
            if (!dateValue) return '';
            const date = new Date(dateValue);
            if (isNaN(date.getTime())) {
                console.warn('Invalid API date:', dateValue);
                return dateValue;
            }
            return date.toLocaleString();
        }

        // Show result view
        function showResultView(isSuccess) {
            const checkoutView = document.getElementById("checkout-view");
            const successView = document.getElementById("success");
            const failView = document.getElementById("fail");
            if (checkoutView) checkoutView.style.display = "none";
            if (isSuccess) {
                successView.style.display = "block";
                failView.style.display = "none";
            } else {
                successView.style.display = "none";
                failView.style.display = "block";
            }
        }

        // Checkout data helpers
        function getCheckoutData() {
            const data = localStorage.getItem("webtopup_checkout");
            return data ? JSON.parse(data) : null;
        }

        const summaryData = getCheckoutData();

        if (summaryData?.pageType === "purchaseplan") {
            const row = document.getElementById("planNameRow");
            const planEl = document.getElementById("planName");

            if (row) row.style.display = "block";

            if (planEl) {
                planEl.textContent = summaryData.planName || "-";
            } else {
                console.log("planName element not found");
            }
        }


        function setCheckoutData(data) {
            localStorage.setItem("webtopup_checkout", JSON.stringify(data));
        }
        function clearCheckoutData() {
            localStorage.removeItem("webtopup_checkout");
        }

        // Billing visibility
        const billingSection = document.getElementById("billingAddressSection");
        const paymentRadios = document.querySelectorAll("input[name='paymentMethod']");
        if (billingSection) billingSection.style.display = "none";
        paymentRadios.forEach(radio => {
            radio.addEventListener("change", function () {

                if (this.value === "CARD") {
                    billingSection.style.display = "block";
                } else {
                    billingSection.style.display = "none";
                    resetBillingForm(); // reset fields when switching away from card
                }

            });
        });

        //Reset biiling form after changing the payment method

        function resetBillingForm() {
            const fields = document.querySelectorAll("#billingAddressSection input, #billingAddressSection select");

            fields.forEach(field => {
                field.value = "";
                field.classList.remove("is-invalid");

                const feedback = field.nextElementSibling;
                if (feedback) feedback.textContent = "";
            });

            const terms = document.getElementById("invalidCheck");
            if (terms) terms.checked = false;
        }


        // Web Top Up Page
        const btnContinue = document.getElementById("btnContinue");
        if (btnContinue) {
            const phoneInput = document.getElementById("mobile_number");
            const emailInput = document.getElementById("email_address");
            const errorBox = document.getElementById("error-message");

            function showError(msg) {
                errorBox.innerText = msg;
                errorBox.style.display = "block";
            }
            function clearError() {
                errorBox.innerText = "";
                errorBox.style.display = "none";
            }

            btnContinue.addEventListener("click", async function () {
                const phoneValid = await PhoneBlurHandler(phoneInput.value.trim(), phoneInput);
                if (!phoneValid) return;
                const selectedRadio = document.querySelector("input[name='WebTopUp_value']:checked");
                if (!selectedRadio) {
                    showError("Please select a plan");
                    return;
                }
                setCheckoutData({
                    planCode: selectedRadio.value,
                    email: emailInput.value.trim(),
                    amount: selectedRadio.value,
                    phone: phoneInput.value.trim()
                    
                });
                const data = getCheckoutData();
                const res = await fetch('/api/checkoutsession/create-session', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        email: data.email,
                        phone: data.phone,
                        planCode: data.planCode,
                        amount: data.amount
                    })
                });
                const result = await res.json();
                if (!result.sessionId) {
                    Swal.fire({ icon: 'error', title: 'Oops...', text: 'Unable to start checkout' });
                    return;
                }
                window.location.href = `/checkout?sid=${result.sessionId}`;
            });
        }

        // Confirmation modal
        document.getElementById("placeOrderBtn")?.addEventListener("click", function (e) {
            const paymentMethod = document.querySelector("input[name='paymentMethod']:checked")?.value;
            if (!paymentMethod) {
                e.preventDefault();
                Swal.fire({
                    icon: "warning",
                    title: "Select payment method",
                    text: "Please choose a payment method before continuing."
                });
                return;
            }
            if (paymentMethod === "CARD" && !validateBillingForm()) {
                e.preventDefault();
                return;
            }
            const data = getCheckoutData();
            document.getElementById("confirmNumber").textContent = data?.phone ?? "-";
            document.getElementById("confirmAmount").textContent = `$${data?.amount}` ?? "-";
            document.getElementById("confirmPayment").textContent = paymentMethod ?? "-";
            new bootstrap.Modal(document.getElementById("exampleModalScrollable2")).show();
        });

        // Phone validation
        var PhoneBlurHandler = async function (phoneNumber, inputElement) {
            if (!phoneNumber) {
                btnContinue.disabled = true;
                return;
            }
            btnContinue.disabled = true;
            try {
                const valResponse = await fetch("/api/Validation/CheckNumberIsValid", {
                    method: 'POST',
                    body: JSON.stringify(phoneNumber),
                    headers: { 'Content-Type': 'application/json' }
                });
                const valRes = await valResponse.json();
                if (valRes.data?.isNumberValid === false) {
                    Swal.fire({
                        title: "Invalid Phone Number",
                        text: "Please enter a valid mobile number to continue.",
                        icon: "error",
                        confirmButtonText: "OK"
                    });
                    inputElement.classList.add("is-invalid");
                    return false;
                } else {
                    inputElement.classList.remove("is-invalid");
                    return true;
                }
            } catch (err) {
                Swal.fire({
                    title: "Validation Failed",
                    text: "Unable to validate phone number. Please try again.",
                    icon: "error",
                    confirmButtonText: "OK"
                });
            }
        };

        // Checkout submit
        document.getElementById("checkoutBtn")?.addEventListener("click", async function () {

            const paymentMethod = document.querySelector("input[name='paymentMethod']:checked")?.value;
            if (!paymentMethod) {
                Swal.fire({
                    title: "Oops!",
                    text: "Please select a payment method before proceeding.",
                    icon: "info",
                    confirmButtonText: "Select Payment"
                });
                return;
            }
            const checkoutData = JSON.parse(localStorage.getItem("checkoutData"));


            try {
                const form = this.closest("form");
                const formData = new FormData(form);

                const response = await fetch("/api/checkoutsession/checkout-submit", {
                    method: "POST",
                    headers: {
                        "RequestVerificationToken": form.querySelector('input[name="__RequestVerificationToken"]').value
                    },
                    body: formData
                });

                //  Handle backend validation errors (BadRequest)
                if (!response.ok) {
                    let errorMessage = "Something went wrong";

                    try {
                        errorMessage = await response.text();
                    } catch (e) {
                        console.error("Error reading response:", e);
                    }

                    Swal.fire({
                        icon: 'error',
                        title: 'Invalid Payment Method',
                        text: errorMessage,
                        confirmButtonText: 'OK'
                    });

                    return;
                }

                //  Success response
                const result = await response.json();

                if (!result.isSuccess) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Payment Failed',
                        text: result.message || "Payment failed",
                        confirmButtonText: 'OK'
                    });
                    return;
                }

                // ✅ Save payment method
                localStorage.setItem("payment_method", paymentMethod);

                // ✅ Redirect
                const redirectUrl = result.data?.url;

                if (redirectUrl) {
                    window.location.href = redirectUrl;
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'An internal error has occurred',
                        confirmButtonText: 'OK',
                        allowOutsideClick: false
                    }).then(() => {
                        window.history.back();
                    });
                }

            } catch (error) {
                console.error("Checkout error:", error);

                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Something went wrong. Please try again later.',
                    confirmButtonText: 'OK'
                });
            }
        });


        // Payment callback handler
        if (window.location.pathname.toLowerCase().includes("checkout")
            && window.location.search.length > 0 &&
            !new URLSearchParams(window.location.search).has("sid")
        ) {
            const params = new URLSearchParams(window.location.search);
            let paymentMethod = localStorage.getItem("payment_method");
            if (!paymentMethod) {
                if (params.has("rID") || params.has("tID") || params.has("token")) paymentMethod = "MPAISA";
                else if (params.has("sessionId")) paymentMethod = "CARD";
            }
            let provideUpdateRequest = { mPaisaPayment: null, creditCardPayment: null };
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
                provideUpdateRequest.creditCardPayment = { sessionId: params.get("sessionId") };
            }
            (async function confirmPayment() {
                try {
                    const res = await fetch("/api/checkoutsession/provide-update", {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify(provideUpdateRequest)
                    });
                    const result = await res.json();
                    const data = getCheckoutData();
                    const d = result.data;
                    localStorage.setItem("payment_result", JSON.stringify(d));
                    if (d.isSuccessful) {
                        window.location.href = `${data.pageUrl}?status=success` ;
                    } else {
                        window.location.href = `${data.pageUrl}?status=fail` ;
                    }
                    clearPaymentQueryParams();
                    localStorage.removeItem("payment_method");
               
                } catch (err) {
                    Swal.fire({
                        title: "Payment Confirmation Failed",
                        text: "We were unable to confirm your payment. Please check your details or try again.",
                        icon: "error",
                        confirmButtonText: "OK"
                    });
                }
            })();
        }


    });
})();
