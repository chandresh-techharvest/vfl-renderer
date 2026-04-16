


(function () {
    document.addEventListener("DOMContentLoaded", function () {
        var showElement = function (element) {
            element.style.display = "flex";
        };

        var hideElement = function (element) {
            element.style.display = "none";
        };
        function getCookie(name) {
            let cookieArr = document.cookie.split(";");

            for (let i = 0; i < cookieArr.length; i++) {
                let cookiePair = cookieArr[i].split("=");

                // Removing whitespace at the beginning of the cookie name
                if (name == cookiePair[0].trim()) {
                    // Decode the value to handle special characters
                    return decodeURIComponent(cookiePair[1]);
                }
            }

            // Return null if not found
            return null;
        }
        //this function for reset the extra information of url after show the transaction detail 
        const params = new URLSearchParams(window.location.search);

        


        function showView(view) {
            let myCookie = getCookie("luData");
            const defaultView = document.getElementById("webtopup-default-view");
            const checkoutView = document.getElementById("webtopup-checkout-view");

            const luData = JSON.parse(myCookie);

            if (luData != null)
            {
               // document.getElementById("luName").innerText = luData.firstName + " " + luData.lastName;
                document.getElementById("email_address").innerText = luData.email;
            }
            if (!defaultView || !checkoutView) return;

            if (view === "default") {
                showElement(defaultView);
                hideElement(checkoutView);
            }

            if (view === "checkout") {
                showElement(checkoutView);
                hideElement(defaultView);

            }
        }
        showView("default");

        


        function populateOrderSummary() {
            const checkoutData = getCheckoutData();
            if (!checkoutData) return;

            //document.getElementById("summaryPhone").innerText = checkoutData.phone;
            //document.getElementById("summaryAmount").innerText = `$${checkoutData.amount}`;
            //document.getElementById("summaryTotal").innerText = `$${checkoutData.amount}`;
            //document.getElementById("summaryItems").innerText = "1";
        }


        




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

        function isValidEmail(email) {
            return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
        }

        /* =====================================================
           BILLING VISIBILITY
        ===================================================== */

        const billingSection = document.getElementById("billingAddressSection");
        const paymentRadios = document.querySelectorAll("input[name='paymentMethod']");

        if (billingSection) billingSection.style.display = "none";

        paymentRadios.forEach(radio => {
            radio.addEventListener("change", function () {
                billingSection.style.display = this.value === "CARD" ? "block" : "none";
            });
        });

        var invalidateElement = function (element) {


            if (!element || !element.classList) return;

            if (Array.isArray(classInvalid) && classInvalid.length > 0) {
                element.classList.add(...classInvalid);
            }

            element.setAttribute(invalidDataAttr, "");
        };



        /* =====================================================
           WEB TOP UP PAGE
        ===================================================== */

        const btnContinue = document.getElementById("btnContinue");

        if (btnContinue) {

            const phoneInput = document.getElementById("mobile_number");
            const emailInput = document.getElementById("email_address");
            const errorBox = document.getElementById("error-message");

            let isAmountSelected = false;
            let isEmailValid = false;

            function showError(msg) {
                errorBox.innerText = msg;
                errorBox.style.display = "block";
            }

            function clearError() {
                errorBox.innerText = "";
                errorBox.style.display = "none";
            }

            //function updateContinueButton() {
            //    if (isAmountSelected ) {
            //        btnContinue.disabled = false;
            //        //btnContinue.style.display = "inline-block";
            //    } else {
            //        btnContinue.disabled = true;
            //       // btnContinue.style.display = "none";
            //    }
            //}

            document.querySelectorAll("input[name='WebTopUp_value']").forEach(radio => {
                radio.addEventListener("change", () => {
                    isAmountSelected = true;
                  //  updateContinueButton();
                });
            });
            const defaultSelected = document.querySelector(
                "input[name='WebTopUp_value']:checked"
            );
            if (defaultSelected) {
                isAmountSelected = true;
               // updateContinueButton();
            }





            //emailInput.addEventListener("blur", function () {
            //    clearError();
            //    const email = emailInput.value.trim();
            //    if (!email) {
                    
            //        return;
            //    }

            //    // ✅ Only validate format if user typed something
            //    if (!isValidEmail(email)) {
            //        showError("* Invalid email address.");
            //        emailInput.classList.add("is-invalid");
            //    } else {
            //        emailInput.classList.remove("is-invalid");
            //    }
            //    updateContinueButton();
            //});

            // ✅ CONTINUE CLICK HANDLER (INSIDE)
            btnContinue.addEventListener("click", async function () {
                clearError();
                let errors = [];
                const email = emailInput.value.trim();
                const phone = phoneInput.value.trim();
                emailInput.classList.remove("is-invalid");
                phoneInput.classList.remove("is-invalid");

                if (!email) {
                    errors.push("* Email is required.");
                    emailInput.classList.add("is-invalid");
                }

                if (!phone) {
                    errors.push("* Phone number is required.");
                    phoneInput.classList.add("is-invalid");
                }

                // ❌ Stop if required fields missing
                if (errors.length > 0) {
                    showError(errors.join("\n"));
                    return;
                }

                // ✅ Email format validation
                if (!isValidEmail(email)) {
                    showError("* Invalid email address.");
                    emailInput.classList.add("is-invalid");
                    return;
                }

                // ✅ Phone API validation
                const phoneValid = await PhoneBlurHandler(phone, phoneInput);
                if (!phoneValid) return;




                const selectedRadio =
                    document.querySelector("input[name='WebTopUp_value']:checked");

                if (!selectedRadio) {
                    showError("Please select a plan");
                    return;
                }
                var checkoutpageurl = document.getElementById("hdnCheckoutPage").value;
                setCheckoutData({
                    planCode: selectedRadio.value,
                    email: emailInput.value.trim(),
                    amount: selectedRadio.value,
                    phone: phoneInput.value.trim(),
                    pageUrl: `/main/webtopup/${checkoutpageurl}`,
                    pageType: "webtopup"
                });

                populateOrderSummary();
                //showView("checkout"); // ✅ SWITCH VIEW HERE

                const data = getCheckoutData();
                //window.location.href =
                //    `/webtopup/checkout?email=${encodeURIComponent(data.email)}`
                //    + `&number=${encodeURIComponent(data.phone)}`
                //    + `&planCode=${encodeURIComponent(data.planCode)}`
                //    + `&amount=${encodeURIComponent(data.amount)}`;

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

                // 🔐 redirect with SAFE token only
                window.location.href = `${data.pageUrl}?sid=${result.sessionId}`;

            });

        }


        /* =====================================================
           SERVER SIDE VALIDATION CHECK NUMBER IS  VALID OR NOT
        ===================================================== */


        var PhoneBlurHandler = async function (phoneNumber, inputElement) {


            clearError();
            inputElement.classList.remove("is-invalid");


            if (phoneNumber.length !==7) {
                return showError(" * The number entered is not a valid Vodafone number.");
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
                }
                else {
                    clearError();
                    inputElement.classList.remove("is-invalid");
                    return true;
                }

            } catch (err) {
                showError("Unable to validate number. Please try again.");
            }
        };



        //const phoneInput = document.getElementById("mobile_number");

        //let isPhoneValid = false;

        //phoneInput.addEventListener("blur", async function () {
        //    isPhoneValid = await PhoneBlurHandler(
        //        phoneInput.value.trim(),
        //        phoneInput
        //    );
        //    updateContinueButton();
        //});



      


    });
})();