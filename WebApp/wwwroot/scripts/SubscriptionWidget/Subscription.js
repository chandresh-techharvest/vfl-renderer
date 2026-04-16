(function () {
    document.addEventListener("DOMContentLoaded", function () {
        const titleEl = document.getElementById("successTitle");
        const titlefailEl = document.getElementById("failTitle");
        const planIntro = document.getElementById("planintro");
        const confirmMsg = document.getElementById("confirmMessage");
        let processText = "Subscription";
        const responseotpCode = document.getElementById("oCresponse");
        const verifyBtn = document.getElementById("btnprepaygiftingsubscribe");
        let pageSize = 8;
        let currentPage4g = 1;
        let currentPage5g = 1;
        var tab4G = document.querySelector('[data-bs-target="#4gplans"]');
        var tab5G = document.querySelector('[data-bs-target="#5gplans"]')
        let plans4g = [];
        let plans5g = [];
        let selectedPlanType = "";
        const planTypeFilter5g = document.getElementById("planTypeFilter5g");
        const sortFilter5g = document.getElementById("sortFilter5g");
        const sortFilter = document.getElementById("sortFilter");
        const plansContainer = document.getElementById("plansContainer");
        const otpCode = document.getElementById("otpCode");
        const btnResendOtp = document.getElementById("btnResendOtp");
        var lastOtpRequest = null;
        var selectedPlanId = null;
        var selectedPlanData = null;
        let otpInterval;
        const otpTimerText = document.getElementById("otpTimer");
        let isLoading = false;
        var selectedProcessId = 1; 
        let currentPlans = [];
        let selectedPlanCode = "";
        document.getElementById("btnSubscribe")?.addEventListener("click", () => { processId = 1; });
        document.getElementById("btnUnsubscribe")?.addEventListener("click", () => { processId = 2; });
        document.getElementById("btnResubscribe")?.addEventListener("click", () => { processId = 3; });

       
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
        const params = new URLSearchParams(window.location.search);

        //formatting date and time
        function formatShortDate(dateValue)
        {
            if (!dateValue) return;
            const date = new Date(dateValue);
            return date.toLocaleString();
        }

        function bindPlanRadioEvents() {
            document.querySelectorAll(".plan-radio").forEach(radio => {
                radio.addEventListener("change", function () {
                    selectedPlanId = this.value;
                    loadPlanDetails(selectedPlanId);
                });
            });
        }

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

        //load all plan according to 4g and 5g tab

        tab4G.addEventListener("shown.bs.tab", async function () {

            const number = await getSelectedSenderFromCookie();
            if (!number) {
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
            }
            await loadAllPlans(number);   // or loadPlansByType(number,"4G")
        });
      

        tab5G.addEventListener("shown.bs.tab", async function () {

            const number =await  getSelectedSenderFromCookie();
            if (!number) {
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
            }
             await loadAllPlans(number);   // or loadPlansByType(number,"5G")
        });

        function showOtpSuccessMessage() {
            document.getElementById("otpSuccessMessage")?.classList.remove("d-none");
            document.getElementById("otpResendInfoMessage")?.classList.add("d-none");
        }
        function showResendInfoMessage() {
            document.getElementById("otpSuccessMessage")?.classList.add("d-none");
            document.getElementById("otpResendInfoMessage")?.classList.remove("d-none");
        }
    
        function setMaskedNumber(phoneNumber) {
            if (!phoneNumber || phoneNumber.length < 2) return;

            const usernumber = phoneNumber
            document.getElementById("maskedNumber").innerText = usernumber;
        }
      
        function startOtpTimer() {
            clearInterval(otpInterval);
            let timeLeft = 30; // seconds
            const timerEl = document.getElementById("otpTimer");
            showOtpSuccessMessage();
            btnResendOtp.disabled = true;
            otpTimerText.innerText = `Resend OTP in ${timeLeft} `;
            otpInterval = setInterval(() => {
                timeLeft--;
                timerEl.innerText = `Resend OTP in ${timeLeft} `;
                if (timeLeft <= 0) {
                    clearInterval(otpInterval);
                    otpTimerText.innerText = "";
                    showResendInfoMessage();
                    btnResendOtp.disabled = false;
                }   
            }, 1000);    
                
        }   
           
        var invalidateElement = function (element) {
            if (!element || !element.classList) return;
            if (Array.isArray(classInvalid) && classInvalid.length > 0) {
                element.classList.add(...classInvalid);
            }
            element.setAttribute(invalidDataAttr, "");
        };
           


        async function loadPlansByType(number, planType) {

            try {
                const res = await fetch(`/Subscription/GetPlansByType?number=${number}&planType=${planType}`);
                if (!res.ok) throw new Error("API failed");
                const plans = await res.json();
                renderPlans(plans);
            } catch (err) {
                console.error(err);
                Swal.fire("Error", "Failed to load plans", "error");
               
            }
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
              const cookieValue = await getDecryptedCookie()
            if (!cookieValue) return null;
            try {
                return cookieValue;
            } catch {
                return null;
            }
        }   
           
       //this function for reset the extra information of url after show the transaction detail 
       
        document.addEventListener("click", async function (e) {
            const btn = e.target.closest(".action-btn");
            if (!btn) return;
            try {
                selectedProcessId = parseInt(btn.dataset.process || "1");
                const planId = btn.dataset.planid;
                console.log("PlanId:", planId);
                
                const number = await getSelectedSenderFromCookie();
                if (!number) {
                    Swal.fire("Error", "Select Number first from Dashboard", "error");
                    return;
                }
                // 🔹 CALL API TO LOAD PLAN
                const res = await fetch(`/Subscription/GetPlanById?number=${number}&planId=${planId}`);
                const planData = await res.json();
                selectedPlanData = planData;
                // 🔹 Fill confirmation modal data
                document.getElementById("confirmPlan").innerText = planData.name;
                document.getElementById("confirmPrice").innerText = planData.amountWithCurrency;
                document.getElementById("confirmNumber").innerText = number;
                if (selectedProcessId === 1) {
                    planIntro.innerText = "You are about to subscribe to the following plan:";
                    confirmMsg.innerText = "Do you wish to continue with the subscription?";
                }
                else if (selectedProcessId === 2) {
                    planIntro.innerText = "You are about to unsubscribe from the following plan:";
                    confirmMsg.innerText = "Do you wish to continue with the unsubscription?";
                    selectedPlanCode = btn.dataset.plancode;
                }

                else if (selectedProcessId === 3) {
                    planIntro.innerText = "You are about to resubscribe to the following plan:";
                    confirmMsg.innerText = "Do you wish to continue with the resubscription?";
                    selectedPlanCode = btn.dataset.plancode;
                }
                // 🔹 Open confirmation modal
                new bootstrap.Modal(
                    document.getElementById("ModalConfirmation-prepaygifting")
                ).show();
            } catch (err) {
                console.error(err);
                Swal.fire("Error", "Unable to load plan details", "error");
               
            }
        });


        document.addEventListener("change", async function (e) {
            if (!e.target.classList.contains("plan-radio")) return;
            console.log("Radio clicked:", e.target.value);
            try {
                const planId = e.target.value;
                const number = await getSelectedSenderFromCookie();
                //const numberType = await checkNumberType(number);
                /* Get Plan  firs*/
                const res = await fetch(`/Subscription/GetPlanById?number=${number}&planId=${planId}`);
                const planData = await res.json();
                selectedPlanData = planData;

                const isPostpay = await checkNumberType(number);
                if (isPostpay) {


                    //await subscribeDirectly();
                    return;
                }
                else {
                    /* Get Balance */
                    const balance = await getRealMoneyBalance(number);
                    const planAmount = parseFloat(planData.amountWithCurrency.replace(/[^0-9.]/g, ""));
                    if (balance < planAmount) {
                        Swal.fire(
                            "Insufficient Balance ❌",
                            "You cannot proceed with this plan.",
                            "warning"
                        );
                        e.target.checked = false;
                        return;
                    }
                }
                
            } catch (err) {
                Swal.fire("Error", "Something went wrong", "error");
                e.target.checked = false;

            }
        });

        async function loadPlanDetails(planId) {
            try {
                const number = await getSelectedSenderFromCookie();
                const res = await fetch(`/Subscription/GetPlanById?number=${number}&planId=${planId}`);
                const data = await res.json();
                selectedPlanData = data;
            }
            catch (err) {
                console.error("API Error", err);
            }
        }


        async function loadAllPlans(number) {
            if (isLoading) return;
            isLoading = true;
            try {
                const res = await fetch(`/Subscription/GetAllPlans?number=${number}`);
                if (!res.ok) {
                    throw new Error("Failed to load plans");
                }
                const plans = await res.json();
                renderPlans(plans);
            }
            catch (err) {
                console.error(err);
            }
            finally {
                isLoading = false;
            }
        }
           

        if (planTypeFilter) {
            planTypeFilter.addEventListener("change", function () {
                selectedPlanType = this.value || "";
                applyFilters();
            });
        }


        if (planTypeFilter5g) {
            planTypeFilter5g.addEventListener("change", function () {
                selectedPlanType = this.value || "";
                applyFilters();
            });
        }    
                

        if (sortFilter) {
            sortFilter.addEventListener("change", function () {
                applySorting(this.value);
            });
        }

        if (sortFilter5g) {
            sortFilter5g.addEventListener("change", function () {
                applySorting(this.value);
            });
        }

        async function applyFilters() {
            const number = await getSelectedSenderFromCookie();
            const planType = selectedPlanType.trim();
            if (planType !== "") {
                loadPlansByType(number, planType);
                return;
            }
            loadAllPlans(number);
        }


       

        


       
       


        document.querySelectorAll(".plan-radio").forEach(radio => {

            radio.addEventListener("change", function () {

                selectedPlanId = this.value;

                //console.log("Selected Plan:", selectedPlanId);

                loadPlanDetails(selectedPlanId);
            });

        });

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


           


        //this event listner for plan detail
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
                    //ul.innerHTML = "<li></li>";
                }
            }
                    
        });  

              

              


                

     





          
      


        



               



           
  


       





               

               

                



              

               

                    

                    



          
              

               

        




                

                

                



                



               
           


       







       



        


   


       













       
      

       

        





        function renderPaginationControls(totalItems, currentPage, paginationId, containerId) {

            const pagination = document.getElementById(paginationId);
            const totalPages = Math.ceil(totalItems / pageSize);

            if (totalPages <= 1) return;

            let html = "";

            // Prev
            html += `
        <li class="page-item ${currentPage === 1 ? "disabled" : ""}">
            <a class="page-link" href="#" data-page="${currentPage - 1}" data-container="${containerId}">Prev</a>
        </li>`;

            // Numbers
            for (let i = 1; i <= totalPages; i++) {
                html += `
        <li class="page-item ${currentPage === i ? "active" : ""}">
            <a class="page-link" href="#" data-page="${i}" data-container="${containerId}">${i}</a>
        </li>`;
            }

            // Next
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

                    if (container === "plansContainer") {
                        currentPage4g = page;
                    } else {
                        currentPage5g = page;
                    }

                    renderPagedPlans();
                });
            });
        }




        const confirmModal = document.getElementById("ModalConfirmation-prepaygifting");

        if (confirmModal) {

            confirmModal.addEventListener("show.bs.modal", async function () {

                /* ================= USER NAME ================= */

                const myCookie = await getDecryptedCookie()

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
                console.log("fullname", fullName);
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
                    await getSelectedSenderFromCookie();


            });
        }






        function getNumericPrice(plan) {
            if (!plan.amountWithCurrency) return 0;
            return parseFloat(plan.amountWithCurrency.replace(/[^0-9.]/g, "")) || 0;
        }
        function getNumericData(plan) {

            if (!plan) return 0;

            // remove HTML from summary if present
            let text = plan.summary
                ? plan.summary.replace(/<[^>]+>/g, "")
                : plan.name || "";

            // extract number before GB
            let match = text.match(/([0-9.]+)\s*GB/i);

            if (match) {
                return parseFloat(match[1]);
            }

            return 0;
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

                case "data_asc":
                    sorted.sort((a, b) => getNumericData(a) - getNumericData(b));
                    break;

                case "data_desc":
                    sorted.sort((a, b) => getNumericData(b) - getNumericData(a));
                    break;
            }

            renderPlans(sorted);

        }

       


       
       

        function parseAmount(amountWithCurrency) {

            if (!amountWithCurrency) return 0;

            return parseFloat(
                amountWithCurrency.replace(/[^0-9.]/g, "")
            );
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






        document.addEventListener("click", function (e) {
            const btn = e.target.closest(".action-btn");
            if (!btn) return;

            selectedProcessId = parseInt(btn.dataset.process || "1");
            console.log("Selected Process:", selectedProcessId);
            if (selectedProcessId == 2 || selectedProcessId == 3)
                selectedPlanCode = btn.dataset.plancode;
        });


        ////////////////////send otp model
        //document.getElementById("btnSubscribe").addEventListener("click", function () {

        //    // check if any plan is selected
        //    const selectedPlan = document.querySelector(".plan-radio:checked");

        //    if (!selectedPlan) {

        //        Swal.fire({
        //            icon: "warning",
        //            title: "No Plan Selected",
        //            text: "Please select a plan first before subscribing.",
        //            confirmButtonText: "OK"
        //        });

        //        return;
        //    }

        //    // open confirmation modal if plan selected
        //    const modal = new bootstrap.Modal(
        //        document.getElementById("ModalConfirmation-prepaygifting")
        //    );

        //    modal.show();

        //});


        document.querySelectorAll(".btnSubscribe").forEach(btn => {
            btn.addEventListener("click", function () {

                const selectedPlan = document.querySelector(".plan-radio:checked");

                if (!selectedPlan) {
                    Swal.fire({
                        icon: "warning",
                        title: "No Plan Selected",
                        text: "Please select a plan first before subscribing."
                    });
                    return;
                }

                const modal = new bootstrap.Modal(
                    document.getElementById("ModalConfirmation-prepaygifting")
                );

                modal.show();
            });
        });

        document.getElementById("confirmBtn").addEventListener("click", async function () {

            try {

                const number = await getSelectedSenderFromCookie();

                // function that checks number type
                const isPostpay = await checkNumberType(number);

               

                if (isPostpay) {

                    // call direct subscription
                    await subscribeDirectly();

                } else {

                    // otherwise continue normal flow (OTP or payment)
               
                    const confirmModalEl = document.getElementById("ModalConfirmation-prepaygifting");
                    const confirmModal = bootstrap.Modal.getInstance(confirmModalEl);

                    if (confirmModal) {
                        confirmModal.hide();
                    }
                    const otpOption = new bootstrap.Modal(document.getElementById("exampleModalScrollable2"));
                    otpOption.show();

                }

            } catch (error) {
                console.error("Confirm button error:", error);
            }

        });






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


                var mobile = await getSelectedSenderFromCookie(); // fallback
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
               // const phone = document.getElementById("mobile_number").value;

                let planCode = "";

                if (selectedProcessId == 2 || selectedProcessId == 3) {
                    planCode = selectedPlanCode;
                } else {
                    if (selectedPlanData?.planCodes?.length > 0) {
                        const realMoneyPlan = selectedPlanData.planCodes.find(
                            p => p.paymentMethod === "REAL_MONEY"
                        );

                        planCode = realMoneyPlan?.pcode || null;
                    }
                }

                const usernum = await getSelectedSenderFromCookie();
                // Build request model
                const request = {
                    number: usernum,
                    option: optionValue,
                    pCode: planCode,
                    processId: selectedProcessId
                };

                lastOtpRequest = request;

                try {

                    btnSendOtp.disabled = true;
                    otpCode.value = "";
                    const res = await fetch("/Subscription/SendOtpRequest", {
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
                    bootstrap.Modal.getInstance(
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

                   // console.error(err);

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
            const res = await fetch("/Subscription/SendOtpRequest", {
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

            console.log("Entered OTP:", enteredOtp);
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


          
            const sender = getSelectedSenderFromCookie();
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

            if (selectedProcessId == 2 || selectedProcessId == 3) {
                planCode = selectedPlanCode;
            } else {
                if (selectedPlanData?.planCodes?.length > 0) {
                    const realMoneyPlan = selectedPlanData.planCodes.find(
                        p => p.paymentMethod === "REAL_MONEY"
                    );

                    planCode = realMoneyPlan?.pcode || null;
                }
            }
                



            // ✅ Build request model
            const request = {


                number: await getSelectedSenderFromCookie(),
             
                pCode: planCode,
                processId: selectedProcessId,
                otpCode: enteredOtp,        // User OTP
                      
                oCresponse: serverOtp       // Encrypted OTP
            };

            try {

                verifyBtn.disabled = true;
                let apiUrl = "/Subscription/Subscribe";

                if (selectedProcessId === 3) {
                    apiUrl = "/Subscription/Resubscribe";
                }
                else if (selectedProcessId === 2) {
                    apiUrl = "/Subscription/Unsubscribe";
                }
                const res = await fetch(apiUrl, {
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




                    // Hide main form (change ID if needed)
                    document.getElementById("selected-plan-view").style.display = "none";

                    // Show success div
                    document.getElementById("success").style.display = "block";



                    // Fill success details (if available)
                    const info = result.data.information || {};

                    titleEl.innerText = `${info.process} Successful`;


                    document.getElementById("successDate").innerText =
                        formatShortDate(info.date) || new Date().toLocaleDateString();

                    document.getElementById("successOrderRef").innerText =
                        info.orderReference || "N/A";

                    document.getElementById("successEmail").innerText =
                        info.email || "N/A";

                    document.getElementById("successNumber").innerText =
                        info.number || sender;

                    document.getElementById("successAmount").innerText =
                        info.amount || "N/A";
                    document.getElementById("successPlanName").innerText =
                        info.planName || "N/A";

                } else {

                    document.activeElement.blur();

                    const otpModal = bootstrap.Modal.getInstance(
                        document.getElementById("otpmodal")
                    );

                    if (otpModal) otpModal.hide();

                    document.querySelectorAll(".modal-backdrop").forEach(e => e.remove());

                    document.body.classList.remove("modal-open");
                    document.body.style = "";




                    // Hide main form (change ID if needed)
                    document.getElementById("selected-plan-view").style.display = "none";

                    // Show success div
                    document.getElementById("fail").style.display = "block";



                    // Fill success details (if available)
                    const info = result.data.information || {};

                    titlefailEl.innerText = `${info.process} Fail`;


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

                }






                // Redirect if needed
                // window.location.href = "/success";

            } catch (err) {

                Swal.fire({
                    icon: "error",
                    title: "Internal Server Error ⚠",
                    text: result?.detail || "Something went wrong. Please try again later."
                });

            } finally {

                verifyBtn.disabled = false;
            }
        });


        async function subscribeDirectly() {

            let planCode = "";
            if (selectedPlanData?.planCodes?.length > 0) {
                const realMoneyPlan = selectedPlanData.planCodes.find(
                    p => p.paymentMethod === "REAL_MONEY"
                );
                planCode = realMoneyPlan?.pcode || null;
            }
            const sendernumber = await getSelectedSenderFromCookie();
            const request = {
                number: sendernumber,
                pCode: planCode,
            
            };
           

            let apiUrl = "/Subscription/Subscribe";

            if (selectedProcessId === 3) {
                apiUrl = "/Subscription/Resubscribe";
            }
            else if (selectedProcessId === 2) {
                apiUrl = "/Subscription/Unsubscribe";
            }

            const res = await fetch(apiUrl, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(request)
            });

            const result = await res.json();

            if (result?.data?.isSuccessful === true ) {

                document.activeElement.blur();

                const otpModal = bootstrap.Modal.getInstance(
                    document.getElementById("otpmodal")
                );

                if (otpModal) otpModal.hide();

                document.querySelectorAll(".modal-backdrop").forEach(e => e.remove());

                document.body.classList.remove("modal-open");
                document.body.style = "";



                // Hide main form (change ID if needed)
                document.getElementById("selected-plan-view").style.display = "none";

                // Show success div
                document.getElementById("success").style.display = "block";

                // Fill success details (if available)
                const info = result.data.information || {};


                titleEl.innerText = `${info.process} Successful`;



                document.getElementById("successDate").innerText =
                    formatShortDate(info.date) || new Date().toLocaleDateString();

                document.getElementById("successOrderRef").innerText =
                    info.orderReference || "N/A";

                document.getElementById("successEmail").innerText =
                    info.email || "N/A";

                document.getElementById("successNumber").innerText =
                    info.number || sender;

                document.getElementById("successAmount").innerText =
                    info.amount || "N/A";
                document.getElementById("successPlanName").innerText =
                    info.planName || "N/A";

            } else {

                document.querySelectorAll(".modal-backdrop").forEach(e => e.remove());

                document.body.classList.remove("modal-open");
                document.body.style = "";

                document.getElementById("selected-plan-view").style.display = "none";

                // Show success div
                document.getElementById("fail").style.display = "block";



                // Fill success details (if available)
                const info = result.data.information || {};

                titlefailEl.innerText = `${info.process} Fail`;


                document.getElementById("failDate").innerText =
                    formatShortDate(info.date)  || new Date().toLocaleDateString();

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
            }
        }



        
        async function checkNumberType(number) {
            try {
                const valResponse = await fetch("/api/Validation/CheckNumberIsValid", {
                    method: 'POST',
                    body: JSON.stringify(number),
                    headers: { 'Content-Type': 'application/json' }
                });

                const valRes = await valResponse.json();

                // adjust based on your API response structure
                // expected example:
                // { data: { type: "PREPAY" } }
                const isPostpay = valRes.data?.isPostpayNumber;
                return isPostpay;

            } catch (err) {
                console.error("Number validation error:", err);
                return null;
            }
        }




    });
})();