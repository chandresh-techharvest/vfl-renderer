
(function () {
    document.addEventListener("DOMContentLoaded", function () {
        const responseotpCode = document.getElementById("oCresponse");
        const verifyBtn = document.getElementById("btnprepaygiftingsubscribe");
        const number = document.getElementById("mobile_number").value
        const plansContainer = document.getElementById("plansContainer");
        const otpCode = document.getElementById("otpCode");
        const btnResendOtp = document.getElementById("btnResendOtp");
        var lastOtpRequest = null;
        var selectedPlanId = null;
        var selectedPlanData = null;
        let otpInterval;
        const otpTimerText = document.getElementById("otpTimer");
        let isLoading = false;
        const errorBox = document.getElementById("errorBox");
        let pageSize = 8;
        const phoneInput = document.getElementById("mobile_number");
        let currentPage4g = 1;
        let currentPage5g = 1;
        let plans4g = [];
        let plans5g = [];
        let currentPlans = [];
        const btnContinue = document.getElementById("btnContinue");

        function showOtpSuccessMessage() {
            document.getElementById("otpSuccessMessage")?.classList.remove("d-none");
            document.getElementById("otpResendInfoMessage")?.classList.add("d-none");
        }

        function showResendInfoMessage() {
            document.getElementById("otpSuccessMessage")?.classList.add("d-none");
            document.getElementById("otpResendInfoMessage")?.classList.remove("d-none");
        }

        function showError(msg) {
            if (!errorBox) return;
            errorBox.innerText = msg;
            errorBox.style.display = "block";
        }

        function clearError() {
            if (!errorBox) return;
            errorBox.innerText = "";
            errorBox.style.display = "none";
        }


        function formatShortDate(dateValue) {
            if (!dateValue) return;
            const date = new Date(dateValue);
            return date.toLocaleString();
        }

        function setMaskedNumber(phoneNumber) {
            if (!phoneNumber || phoneNumber.length < 2) return;
            const usernumber = phoneNumber
            document.getElementById("maskedNumber").innerText = usernumber;
        }

           
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

       
        async function checkSenderNumberType() {
            const sender = await getSelectedSenderFromCookie();
            if (!sender) {
                Swal.fire({
                    icon: "error",
                    title: "No Number Selected",
                    text: "Please select a number from Dashboard",
                    confirmButtonText: "Go to Dashboard"
                }).then((result) => {
                    if (result.isConfirmed || result.dismiss === Swal.DismissReason.backdrop) {
                        window.location.href = "/main/dashboard";
                    }
                });
                return;
            };
            try {
                const res = await fetch("/api/Validation/CheckNumberIsValid", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(sender)
                });
                const data = await res.json();
                const isValid = data?.data?.isNumberValid;
                const isPostpay = data?.data?.isPostpayNumber;
                // ❌ POSTPAY → Show alert + redirect
                if (isValid && isPostpay) {
                    Swal.fire({
                        icon: "warning",
                        title: "Prepay Number Required",
                        text: "You have selected a postpay number for gifting. Please select a prepay number to proceed.",
                        confirmButtonText: "Go to Dashboard"
                    }).then(() => {
                        window.history.back();
                    });
                    return;
                }

                // ✅ PREPAY → Show blue info message
                if (isValid && !isPostpay) {

                    const infoBox = document.getElementById("gifterInfoBox");

                    if (infoBox) {
                        infoBox.innerText =
                            `The selected gifter number is ${sender}. Please enter the number you wish to gift to.`;
                        infoBox.style.display = "block";
                    }
                }
                

            } catch (err) {
                console.error("Sender check failed", err);
            }
        }

        checkSenderNumberType();

               
        function startOtpTimer() {
            clearInterval(otpInterval);

            let timeLeft = 30; // seconds
            const timerEl = document.getElementById("otpTimer");

            // ⏱ Timer started → show success message
            showOtpSuccessMessage();
            btnResendOtp.disabled = true;

            otpTimerText.innerText = `Resend OTP in ${timeLeft} `;

            otpInterval = setInterval(() => {
                timeLeft--;
                timerEl.innerText = `Resend OTP in ${timeLeft} `;

                if (timeLeft <= 0) {
                    clearInterval(otpInterval);
                    otpTimerText.innerText = "";
                    // ⏱ Timer finished → show resend info
                    showResendInfoMessage();
                    btnResendOtp.disabled = false;
                }
            }, 1000);
        }

        var showElement = function (element) {
            element.style.display = "flex";
        };
 
        var hideElement = function (element) {
            element.style.display = "none";
        };
               

               
        async function getSelectedSenderFromCookie() {
             const cookieValue = await getDecryptedCookie();
            if (!cookieValue) {
                console.error("Cookie not found:", cookieName);
                return null;
            }
            try {
                const data = cookieValue;
                const selectedDevice = data.devices?.find(d => d.isSelected === true);
                return selectedDevice ? selectedDevice.number : null;
            } catch (err) {
                console.error("Invalid cookie JSON:", err);
                return null;
            }
        }

        async function getUserFromCookie() {
            const cookieValue = await getDecryptedCookie();
            if (!cookieValue) return null;
            try {
                return cookieValue;
            } catch {
                return null;
            }
        }
          

        //this function for reset the extra information of url after show the transaction detail 
        const params = new URLSearchParams(window.location.search);

                

            
       



      





      


       



       

      
       


        





       
        
      



        


       



        







        document.getElementById("btnSubscribe").addEventListener("click", function () {

            // check if any plan is selected
            const selectedPlan = document.querySelector(".plan-radio:checked");

            if (!selectedPlan) {

                Swal.fire({
                    icon: "warning",
                    title: "No Plan Selected",
                    text: "Please select a plan first before subscribing.",
                    confirmButtonText: "OK"
                });

                return;
            }

            // open confirmation modal if plan selected
            const modal = new bootstrap.Modal(
                document.getElementById("ModalConfirmation-prepaygifting")
            );

            modal.show();

        });



        function showView(view) {
            const defaultView = document.getElementById("prepaygifting-default-view");
            const checkoutView = document.getElementById("selected-plan-view");
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
           
        var invalidateElement = function (element) {


            if (!element || !element.classList) return;

            if (Array.isArray(classInvalid) && classInvalid.length > 0) {
                element.classList.add(...classInvalid);
            }

            element.setAttribute(invalidDataAttr, "");
        };


        function updateContinueButton() {
            btnContinue.disabled = !isPhoneValid;
        }



        if (btnContinue && phoneInput) {
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
                showView("checkout");
                await loadAllPlans(phoneInput.value.trim());
            });

            phoneInput.addEventListener("blur", async function () {
                isPhoneValid = await PhoneBlurHandler(phoneInput.value.trim(), phoneInput);
                updateContinueButton();
            });
        }


       
        async function loadAllPlans(number) {
            if (isLoading) return;
            isLoading = true;

            try {
                const res = await fetch(`/PrepayGifiting/GetAllPlans`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({ number })
                });

                if (!res.ok) throw new Error("Failed to load plans");

                const plans = await res.json();



                renderPlans(plans);

            } catch (err) {
                console.error(err);
            }
            finally {
                isLoading = false;
            }
        }









      




       
       



       

        async function loadPlanDetails(planId,number) {
            try {
                const res = await fetch(`/PrepayGifiting/GetPlanById?number=${number}&planId=${planId}`);
                const data = await res.json();
                selectedPlanData = data;
            }
            catch (err) {
                console.error("API Error", err);
            }
        }
               

        const sortFilter = document.getElementById("sortFilter");
        if (sortFilter) {
            sortFilter.addEventListener("change", function () {
                applySorting(this.value);
            });
        }   

        function getNumericPrice(plan) {
            if (!plan.amountWithCurrency) return 0;
            return parseFloat(plan.amountWithCurrency.replace(/[^0-9.]/g, "")) || 0;
        }


        function applySorting(sortValue) {

            if (!currentPlans || currentPlans.length === 0) return;

            let sorted = [...currentPlans];

            switch (sortValue) {

                case "price_asc":
                    sorted.sort((a, b) => getNumericPrice(a) - getNumericPrice(b));
                    break;

                case "price_desc":
                    sorted.sort((a, b) => getNumericPrice(b) - getNumericPrice(a));
                    break;


            }

            renderPlans(sorted);

        } 

       


       
       
        function bindPlanRadioEvents() {

            document.querySelectorAll(".plan-radio").forEach(radio => {

                radio.addEventListener("change", async function () {

                    selectedPlanId = this.value;

                    const number = phoneInput.value;

                    if (!number) {
                        Swal.fire("Error", "Enter mobile number first", "error");
                        this.checked = false;
                        return;
                    }

                    try {

                        const sendernumber = await getSelectedSenderFromCookie(); 
                        // get plan
                        const res = await fetch(`/PrepayGifiting/GetPlanById?number=${number}&planId=${selectedPlanId}`);
                        const planData = await res.json();
                        selectedPlanData = planData;

                        // get balance
                        const balance = await getRealMoneyBalance(sendernumber);

                        const planAmount = parseFloat(
                            planData.amountWithCurrency.replace(/[^0-9.]/g, "")
                        );

                        console.log("Balance:", balance);
                        console.log("Plan:", planAmount);

                        if (balance < planAmount) {

                            Swal.fire({
                                icon: "warning",
                                title: "Insufficient Balance ❌",
                                html: `The selected number (gift sender - <b>${sendernumber}</b>) does not have sufficient balance.<br>Please select another plan.`
                            });
                            this.checked = false;
                            return;
                        }

                    } catch (err) {
                        console.error(err);
                    }

                });

            });
        }

        const sortFilter5g = document.getElementById("sortFilter5g");
        if (sortFilter5g) {
            sortFilter5g.addEventListener("change", function () {
                applySorting(this.value);
            });
        }





        function renderPlans(plans) {
            currentPlans = plans || [];
            const container4g = document.getElementById("plansContainer");
            const container5g = document.getElementById("plansContainer5g");

            if (!container4g || !container5g) return;

            plans4g = (plans || []).filter(p => (p.network || "").toUpperCase() === "4G");
            plans5g = (plans || []).filter(p => (p.network || "").toUpperCase() === "5G");

            currentPage4g = 1;
            currentPage5g = 1;
            renderPagedPlans();





        }

        function renderPagedPlans() {

            renderPage(plans4g, currentPage4g, "plansContainer", "pagination4g", "4G");
            renderPage(plans5g, currentPage5g, "plansContainer5g", "pagination5g", "5G");

            bindPlanRadioEvents();
        }

        function parseAmount(amountWithCurrency) {

            if (!amountWithCurrency) return 0;

            return parseFloat(
                amountWithCurrency.replace(/[^0-9.]/g, "")
            );
        }
        function renderPage(plans, page, containerId, paginationId, label) {
            const container = document.getElementById(containerId);
            const pagination = document.getElementById(paginationId);
            container.innerHTML = "";
            pagination.innerHTML = "";
            if (!plans || plans.length === 0) {
                container.innerHTML = `<p class="text-danger w-100">No ${label} plans available</p>`;
                return;
            }
            const start = (page - 1) * pageSize;
            const pagePlans = plans.slice(start, start + pageSize);
            pagePlans.forEach(plan => {
                const card = `<div class="col-sm-6 col-xl-3">
                                  <input type="radio" class="btn-check plan-radio" id="plan_${plan.planId}"  name="SelectedPlan"  value="${plan.planId}">
                                      <label class="plan-card" for="plan_${plan.planId}">
                                      <span class="plan-tick">✔</span>
                                       <h5 class="plan-price">${plan.amountWithCurrency}</h5>
                                       <div class="d-flex align-items-center justify-content-between">

                                       <h6 class="plan-name">${plan.name}</h6>
                                       <a href="javascript:void(0)" class="plan-info" aria-label="View plan details"  data-details='${plan.details}'   title="View plan details" data-bs-toggle="modal"data-bs-target="#Modalplandetails">
                                                                <i class="bi bi-info-circle"></i> Plan Details
                                                            </a>
                                                        </div>

                                       <div class="plan-desc">${plan.summary}</div>
                                       </label>
                               </div>`;
                container.insertAdjacentHTML("beforeend", card);
            });
            renderPaginationControls(plans.length, page, paginationId, containerId);
        }


        document.addEventListener("click", function (e) {
            if (e.target.closest(".plan-info")) {
                const link = e.target.closest(".plan-info");
                const details = link.getAttribute("data-details");
                const ul = document.querySelector("#Modalplandetails ul");
                ul.innerHTML = "";
                if (details) {

                    // if API returns HTML
                    ul.innerHTML = details;
                } else {
                    ul.innerHTML = "<li>No details available</li>";
                }
            }

        });



        function renderPaginationControls(totalItems, currentPage, paginationId, containerId) {
            const pagination = document.getElementById(paginationId);
            const totalPages = Math.ceil(totalItems / pageSize);
            if (totalPages <= 1) return;
            let html = "";
            html += `
        <li class="page-item ${currentPage === 1 ? "disabled" : ""}">
            <a class="page-link" href="#" data-page="${currentPage - 1}" data-container="${containerId}">Prev</a>
        </li>`;
            for (let i = 1; i <= totalPages; i++) {
                html += `
        <li class="page-item ${currentPage === i ? "active" : ""}">
            <a class="page-link" href="#" data-page="${i}" data-container="${containerId}">${i}</a>
        </li>`;
            }
            html += `
        <li class="page-item ${currentPage === totalPages ? "disabled" : ""}">
            <a class="page-link text-primary" href="#" data-page="${currentPage + 1}" data-container="${containerId}">Next</a>
        </li>`;
            pagination.innerHTML = html;
            pagination.querySelectorAll("a").forEach(a => {
                a.addEventListener("click", function (e) {
                    e.preventDefault();
                    const page = parseInt(this.dataset.page);
                    const container = this.dataset.container;
                    if (container === "plansContainer") currentPage4g = page;
                    else currentPage5g = page;
                    renderPagedPlans();
                });
            });
        }

        
        async function getRealMoneyBalance(number) {

            try {

                const res = await fetch(
                    "/PrepayGifiting/GetRealMoneyBalance?number=" + number,
                    {
                        method: "GET"
                    }
                );

                const result = await res.json();
                var value = result?.data || 0;
                value = value.toString().replace("M", "");

                return Number(value);

            } catch (err) {

                console.error("Balance API Error:", err);
                return 0;
            }
        }





        const confirmModal = document.getElementById("ModalConfirmation-prepaygifting");

        if (confirmModal) {

            confirmModal.addEventListener("show.bs.modal", async function () {

                /* ================= USER NAME ================= */

                const myCookie = await getDecryptedCookie();

                let fullName = "Customer";

                if (myCookie) {
                    try {
                        const luData = myCookie;

                        fullName =
                            (luData.firstName || "") + " " +
                            (luData.lastName || "");

                        fullName = fullName.trim() || "Customer";

                    } catch (e) {
                        console.error("luData parse error", e);
                    }
                }

                document.getElementById("confirmUser").innerText = fullName;


                /* ================= PLAN DATA ================= */


                if (!selectedPlanData) return;
                document.getElementById("confirmUser").innerText = fullName;

                document.getElementById("confirmPlan").innerText =
                    selectedPlanData.name;

                document.getElementById("confirmPrice").innerText =
                    selectedPlanData.amountWithCurrency;
                document.getElementById("confirmSummary").innerHTML = selectedPlanData.summary.replace(/<\/?p>/g, '');


                document.getElementById("confirmNumber").innerText =
                    document.getElementById("mobile_number").value;

            });
        }




        ////////////////////send otp model


        const btnSendOtp = document.getElementById("btnSendOtp");

        if (btnSendOtp) {

            btnSendOtp.addEventListener("click", async function () {




                // ✅ Get user data from cookie
                
       
                // ✅ Detect selected option
                const selectedOption = document.querySelector('input[name="otpOption"]:checked');

                if (!selectedOption) {
                    alert("Please select OTP option");
                    return;
                }
                

                const sendernumber = await getSelectedSenderFromCookie(); 
                var mobile = sendernumber ; // fallback
                const userData = await getUserFromCookie();
                var email = userData?.email || "";
                
                const isMobile = selectedOption.value === "1";
                const isEmail = selectedOption.value === "2";

                const otpMessage = document.getElementById("otpMessage");
                const maskedSpan = document.getElementById("maskedNumber");

                // ✅ Update message dynamically
                if (isMobile) {

                    otpMessage.innerHTML =
                        `Please enter the 4 digit code sent to <span id="maskedNumber">${mobile}</span>`;

                }
                else if (isEmail) {

                    otpMessage.innerHTML =
                        `Please enter the 4 digit code sent to <span id="maskedNumber">${email}</span>`;
                }



              

                const optionValue = parseInt(selectedOption.value);

                // Get required values
                const phone = document.getElementById("mobile_number").value;

                let planCode = "";

                if (selectedPlanData?.planCodes?.length > 0) {
                    planCode = selectedPlanData.planCodes[0].pcode;
                }


                // Build request model
                const request = {
                    option: optionValue,
                    pCode: planCode,
                    receiverNumber: phone,
                    senderNumber: sendernumber // change if needed
                };

                lastOtpRequest = request;

                try {

                    btnSendOtp.disabled = true;

                    const res = await fetch("/PrepayGifiting/SendOtpRequest", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify(request)
                    });

                    const result = await res.json();

                    //if (!res.ok || !result?.data) {
                    //    throw new Error("OTP send failed");
                    //}
                    responseotpCode.value = result.data.code;
                    //setMaskedNumber(sendernumber);
                    const otpSuccessMsg = document.getElementById("otpSuccessMessage");
                    otpSuccessMsg.classList.remove("d-none");
                    console.log("OTP Sent:", result);

                    // Close OTP option modal
                    bootstrap.Modal.getOrCreateInstance(
                        document.getElementById("exampleModalScrollable2")
                    ).hide();

                    // Open OTP modal
                    const otpModal = new bootstrap.Modal(
                        document.getElementById("otpmodal")
                    );

                    otpModal.show();
                    startOtpTimer();

                }
                catch (err) {

                    console.error(err);

                    Swal.fire({
                        icon: "error",
                        title: "OTP Failed ❌",
                        text: "Failed to send OTP. Please try again.",
                        confirmButtonText: "Retry"
                    });

                }
                finally {

                    btnSendOtp.disabled = false;
                }

            });

        }




        btnResendOtp.addEventListener("click", async function () {

            btnResendOtp.disabled = true;
            otpCode.value = "";
            const res = await fetch("/PrepayGifiting/SendOtpRequest", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(lastOtpRequest)
            });

            const result = await res.json();
            responseotpCode.value = result.data.code;
            startOtpTimer();
        });

        ///for verufy otp and subscribe
        verifyBtn.addEventListener("click", async function (e) {

            e.preventDefault(); // stop form submit

            const enteredOtp = otpCode.value;
            const serverOtp = responseotpCode.value?.trim() || "";

            console.log("Entered OTP:",enteredOtp);
            console.log("Encrypted OTP:", serverOtp);
            if (!enteredOtp) {
                Swal.fire({
                    icon: "warning",
                    title: "OTP Required ⚠️",
                    text: "Please enter the OTP to continue.",
                    confirmButtonText: "OK"
                });
                return;
            }

          
            const phone = document.getElementById("mobile_number").value.trim();
            const sender = await getSelectedSenderFromCookie(); 
            if (!sender) {
                Swal.fire({
                    icon: "info",
                    title: "Prepay Number Not Selected",
                    text: "Please select your Prepay number from the dashboard to continue.",
                    confirmButtonText: "OK"
                });
                return;
            }
            let planCode = "";

            if (selectedPlanData?.planCodes?.length > 0) {
                planCode = selectedPlanData.planCodes[0].pcode;
            }



            // ✅ Build request model
            const request = {
                otpCode: enteredOtp,        // User OTP
                pCode: planCode,            // Plan code
                receiverNumber: phone,      // Mobile
                senderNumber: sender,        // Mobile
                oCresponse: serverOtp       // Encrypted OTP
            };

            try {

                verifyBtn.disabled = true;

                const res = await fetch("/PrepayGifiting/PrepayGiftSubscribe", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(request)
                });

                const result = await res.json();


                if (result?.data?.isCodeValid === false) {
                    Swal.fire({
                        icon: "error",
                        title: "OTP Verification Failed",
                        text: result?.detail || "Invalid OTP or request failed"
                    });

                }





                else if (result?.data?.isSuccessful === true && result?.data?.isCodeValid === true) {

                    document.activeElement.blur();

                    const otpModal = bootstrap.Modal.getInstance(
                        document.getElementById("otpmodal")
                    );

                    if (otpModal) otpModal.hide();

                    document.querySelectorAll(".modal-backdrop").forEach(e => e.remove());

                    document.body.classList.remove("modal-open");
                    document.body.style = "";



                    const sender = await getSelectedSenderFromCookie(); 


                    // Hide main form (change ID if needed)
                    document.getElementById("selected-plan-view").style.display = "none";

                    // Show success div
                    document.getElementById("success").style.display = "block";

                    // Fill success details (if available)
                    const info = result.data.information || {};

                    document.getElementById("successDate").innerText =
                        formatShortDate(info.date)|| new Date().toLocaleDateString();

                    document.getElementById("successOrderRef").innerText =
                        info.orderReference || "N/A";

                    document.getElementById("successEmail").innerText =
                        info.email || "N/A";

                    document.getElementById("successNumber").innerText =
                        info.number || sender;

                    document.getElementById("successAmount").innerText =
                        info.amount || "N/A";
                    document.getElementById("successplanName").innerText =
                        info.planName || "N/A";

                    document.getElementById("successSender").innerText =
                        sender || "N/A";

                    document.getElementById("successReciever").innerText =
                        phoneInput.value || "N/A";



                } else {

                    document.activeElement.blur();

                    const otpModal = bootstrap.Modal.getInstance(
                        document.getElementById("otpmodal")
                    );

                    if (otpModal) otpModal.hide();

                    document.querySelectorAll(".modal-backdrop").forEach(e => e.remove());

                    document.body.classList.remove("modal-open");
                    document.body.style = "";

                    const sender = await getSelectedSenderFromCookie(); 


                    // Hide main form (change ID if needed)
                    document.getElementById("selected-plan-view").style.display = "none";

                    // Show success div
                    document.getElementById("fail").style.display = "block";



                    // Fill success details (if available)
                    const info = result.data.information || {};

                  

                    document.getElementById("failDate").innerText =
                        formatShortDate(info.date) || new Date().toLocaleDateString();

                    document.getElementById("failOrderRef").innerText =
                        info.orderReference || "N/A";

                    document.getElementById("failEmail").innerText =
                        info.email || "N/A";

                    document.getElementById("failNumber").innerText =
                        info.number || sender;

                    document.getElementById("failAmount").innerText =
                        info.amount || "N/A";
                    document.getElementById("failPlanName").innerText =
                        info.planName || "N/A";

                    document.getElementById("failSender").innerText =
                        sender || "N/A";

                    document.getElementById("failReciever").innerText =
                        phoneInput.value || "N/A";




                }

                console.log("Subscribe Result:", result);

               


              
                // Redirect if needed
                // window.location.href = "/success";

            } catch (err) {

                console.error(err);
                alert("Subscription Failed ❌");

            } finally {

                verifyBtn.disabled = false;
            }
        });



        


        

        async function PhoneBlurHandler(phoneNumber, inputElement) {
            clearError();
            inputElement.classList.remove("is-invalid");
            if (phoneNumber.length > 7) {
                showError(" * The number entered is not a valid Vodafone number.");
                return false;
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





       // const phoneInput = document.getElementById("mobile_number");

        var isPhoneValid = false;

        phoneInput.addEventListener("blur", async function () {
            isPhoneValid = await PhoneBlurHandler(
                phoneInput.value.trim(),
                phoneInput
            );
            updateContinueButton();
        });
        phoneInput.addEventListener("input", function () {
            clearError();
            phoneInput.classList.remove("is-invalid");
        });


        function readPlansFromDOM() {

            const cards = document.querySelectorAll(".plan-radio");
            const plans = [];

            cards.forEach(radio => {

                const label = document.querySelector(`label[for='${radio.id}']`);
                if (!label) return;

                const priceText = label.querySelector(".plan-price")?.innerText || "0";
                const name = label.querySelector(".plan-name")?.innerText || "";
                const summary = label.querySelector(".plan-desc")?.innerText || "";

                const network = radio.closest("#plansContainer5g") ? "5G" : "4G";

                plans.push({
                    planId: radio.value,
                    amountWithCurrency: priceText,
                    name: name,
                    summary: summary,
                    network: network
                });
            });

            currentPlans = plans;
        }

        readPlansFromDOM();
        renderPlans(currentPlans);

    });
})();