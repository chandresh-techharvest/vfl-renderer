(function () {
    document.addEventListener("DOMContentLoaded", function () {
        // Utility functions
        const showElement = element => element.style.display = "flex";
        const hideElement = element => element.style.display = "none";

        async function getDecryptedCookie() {
            try {
                const response = await fetch('/api/Common/GetCookie', {
                    method: "GET",
                    credentials: "include"
                });
                if (!response.ok) return null;
                return await response.json();
            } catch (error) {
                console.error("Error fetching decrypted cookie:", error);
                return null;
            }
        }

        function showView(view) {
            const defaultView = document.getElementById("webtopup-default-view");
            const checkoutView = document.getElementById("webtopup-checkout-view");
            if (!defaultView || !checkoutView) return;
            if (view === "default") {
                showElement(defaultView);
                hideElement(checkoutView);
            } else if (view === "checkout") {
                showElement(checkoutView);
                hideElement(defaultView);
            }
        }
        showView("default");

        function getCheckoutData() {
            const data = localStorage.getItem("webtopup_checkout");
            return data ? JSON.parse(data) : null;
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
                billingSection.style.display = this.value === "CARD" ? "block" : "none";
            });
        });

        // Web Top Up Page
        const btnContinue = document.getElementById("btnContinue");
        if (btnContinue) {
            const phoneInput = document.getElementById("mobile_number");
            const errorBox = document.getElementById("error-message");
            let isAmountSelected = false;

            function showError(msg) {
                errorBox.innerText = msg;
                errorBox.style.display = "block";
            }
            function clearError() {
                errorBox.innerText = "";
                errorBox.style.display = "none";
            }
            //function updateContinueButton() {
            //    btnContinue.disabled = !isAmountSelected;
            //}

            document.querySelectorAll("input[name='WebTopUp_value']").forEach(radio => {
                radio.addEventListener("change", () => {
                    isAmountSelected = true;
                   // updateContinueButton();
                });
            });

            if (document.querySelector("input[name='WebTopUp_value']:checked")) {
                isAmountSelected = true;
              //  updateContinueButton();
            }

            btnContinue.addEventListener("click", async function () {
                clearError();
                phoneInput.classList.remove("is-invalid");
                if (!phoneInput.value.trim()) {
                    showError(" * Phone Number is required.");
                    phoneInput.classList.add("is-invalid");
                    phoneInput.focus();
                    return;
                }
                const phoneValid = await PhoneBlurHandler(phoneInput.value.trim(), phoneInput);
                if (!phoneValid) return;

                const selectedRadio = document.querySelector("input[name='WebTopUp_value']:checked");
                if (!selectedRadio) {
                    showError("Please select a plan");
                    return;
                }
                const luData = await getDecryptedCookie();
                const checkoutpageurl = document.getElementById("hdnCheckoutPage").value;
                setCheckoutData({
                    planCode: selectedRadio.value,
                    email: luData.email,
                    amount: selectedRadio.value,
                    phone: phoneInput.value.trim(),
                    pageUrl: `/main/webtopup/${checkoutpageurl}`,
                    pageType: "webtopup"
                });

                const data = getCheckoutData();
                const res = await fetch('/api/checkoutsession/create-session', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        email: data.email,
                        phone: data.phone,
                        planCode: data.planCode,
                        amount: data.amount,
                        pageUrl: data.pageUrl,
                        pageType: data.pageType
                    })
                });
                const result = await res.json();
                if (!result.sessionId) {
                    alert("Unable to start checkout");
                    return;
                }
                window.location.href = `${data.pageUrl}?sid=${result.sessionId}`;
            });
        }

        // Server-side validation
        async function PhoneBlurHandler(phoneNumber, inputElement) {
            clearError();
            inputElement.classList.remove("is-invalid");
            if (phoneNumber.length !== 7) {
                showError(" * The number entered is not a valid Vodafone number.");
                return false;
            }
           // btnContinue.disabled = true;
            try {
                const valResponse = await fetch("/api/Validation/CheckNumberIsValid_AllowInactiveNumber", {
                    method: 'POST',
                    body: JSON.stringify(phoneNumber),
                    headers: { 'Content-Type': 'application/json' }
                });
                const valRes = await valResponse.json();
                if (valRes.data?.isNumberValid === false) {
                    showError(" * The number entered is not a valid Vodafone number.");
                    inputElement.classList.add("is-invalid");
                    return false;
                } else {
                    clearError();
                    inputElement.classList.remove("is-invalid");
                    return true;
                }
            } catch (err) {
                showError("Unable to validate number. Please try again.");
                return false;
            }
        }

        //const phoneInput = document.getElementById("mobile_number");
        //if (phoneInput) {
        //    phoneInput.addEventListener("blur", async function () {
        //        isPhoneValid = await PhoneBlurHandler(phoneInput.value.trim(), phoneInput);
        //        if (typeof updateContinueButton === "function") updateContinueButton();
        //    });
        //}
    });
})();
