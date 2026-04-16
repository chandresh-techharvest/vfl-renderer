
(function () {
    document.addEventListener("DOMContentLoaded", function () {


        let pageSize = 8;

        let currentPage4g = 1;
        let currentPage5g = 1;

        let plans4g = [];
        let plans5g = [];
        let selectedPlanType = "";
        let selectedPaymentMethod = "";

        const planTypeFilter = document.getElementById("planTypeFilter");
        const plansContainer = document.getElementById("plansContainer");
        const paymentTypeFilter = document.getElementById("paymentTypeFilter");
        const planTypeFilter5g = document.getElementById("planTypeFilter5g");
        const paymentTypeFilter5g = document.getElementById("paymentTypeFilter5g");
        const sortFilter5g = document.getElementById("sortFilter5g");


        let currentPlans = [];
        var selectedPlanId = null;
        var selectedPlanData = null;


        let isLoading = false;

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


        function bindPlanRadioEvents() {

            document.querySelectorAll(".plan-radio").forEach(radio => {

                radio.addEventListener("change", function () {

                    selectedPlanId = this.value;

     

                    loadPlanDetails(selectedPlanId);
                });

            });
        }



        function showView(view) {
            let myCookie = getCookie("luData");
            const defaultView = document.getElementById("purchaseplan-default-view");
            const checkoutView = document.getElementById("selected-plan-view");

            const luData = JSON.parse(myCookie);

            if (luData != null) {
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

            //let isAmountSelected = false;
            let isEmailValid = false;

            function showError(msg) {
                errorBox.innerText = msg;
                errorBox.style.display = "block";
            }

            function clearError() {
                errorBox.innerText = "";
                errorBox.style.display = "none";
            }

         



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


                showView("checkout");
                await loadAllPlans(phoneInput.value.trim());



            });

        }



        /////-------------plan load by tyoe

        async function loadPlansByType(planType) {
            const number = document.getElementById("mobile_number").value.trim();
            try {

                let url = `/PurchasePlan/GetPlansByType?number=${number}&planType=${planType}`;

          

                const res = await fetch(url);

                const plans = await res.json();

               

                renderPlans(plans);

            } catch (err) {
                console.error("Load Plans Error:", err);
            }
        }



        async function loadPlanDetails(planId) {
            const number = document.getElementById("mobile_number").value.trim();

            try {

                const res = await fetch(`/PurchasePlan/GetPlanById?number=${number}&planId=${planId}`);

                const data = await res.json();

                selectedPlanData = data;

                

            }
            catch (err) {
                console.error("API Error", err);
            }
        }


        async function loadPlansByPaymentMethod(method) {
            const number = document.getElementById("mobile_number").value.trim();

            try {

                let url = `/PurchasePlan/GetPlansByPaymentMethod?number=${number}&paymentMethod=${method}`;

               

                const res = await fetch(url);

                const plans = await res.json();

              

                renderPlans(plans);

            } catch (err) {
                console.error("Load Payment Plans Error:", err);
            }
        }


        async function loadPlansByTypeAndPayment(planType, paymentMethod) {
            const number = document.getElementById("mobile_number").value.trim();

            try {

                let url =
                    `/PurchasePlan/GetPlansByTypeAndPayment?number=${number}&planType=${planType}&paymentMethod=${paymentMethod}`;

            

                const res = await fetch(url);

                if (!res.ok) {
                    throw new Error("HTTP Error " + res.status);
                }

                const plans = await res.json();

              

                renderPlans(plans);

            } catch (err) {
                console.error("Combined Load Error:", err);
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












        async function loadAllPlans(number) {

            if (isLoading) return;

            isLoading = true;

            try {
                const res = await fetch(`/PurchasePlan/GetAllPlans?number=${number}`);
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



        //5g plan filter

        if (planTypeFilter5g) {
            planTypeFilter5g.addEventListener("change", function () {
                selectedPlanType = this.value || "";
                applyFilters();
            });
        }

        if (paymentTypeFilter5g) {
            paymentTypeFilter5g.addEventListener("change", function () {
                selectedPaymentMethod = this.value || "";
                applyFilters();
            });
        }

        if (sortFilter5g) {
            sortFilter5g.addEventListener("change", function () {
                applySorting(this.value);
            });
        }




        if (planTypeFilter) {

            planTypeFilter.addEventListener("change", function () {

                selectedPlanType = this.value ||"";




                applyFilters();
            });
        }


        if (paymentTypeFilter) {

            paymentTypeFilter.addEventListener("change", function () {

                selectedPaymentMethod = this.value ||"";

            

                applyFilters();
            });
        }



        function applyFilters() {
            const number = document.getElementById("mobile_number").value.trim();
            const planType = selectedPlanType.trim();
            const payment = selectedPaymentMethod.trim();

            

            // Both selected
            if (planType !== "" && payment !== "") {

                loadPlansByTypeAndPayment(planType, payment);
                return;
            }

            // Only plan type
            if (planType !== "" && payment === "") {

                loadPlansByType(planType);
                return;
            }

            // Only payment
            if (planType === "" && payment !== "") {

                loadPlansByPaymentMethod(payment);
                return;
            }

            // Nothing selected
            loadAllPlans(number);
        }


        ///plan selected by id and show the information in confimation box




        document.querySelectorAll(".plan-radio").forEach(radio => {

            radio.addEventListener("change", function () {

                selectedPlanId = this.value;

             

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








        const confirmModal = document.getElementById("ModalConfirmation-purchaseplan");

        if (confirmModal) {

            confirmModal.addEventListener("show.bs.modal", function () {

                /* ================= USER NAME ================= */

                const myCookie = getCookie("luData");

                let fullName = "Customer";

                if (myCookie) {
                    try {
                        const luData = JSON.parse(myCookie);

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

                document.getElementById("confirmNumber").innerText =
                    document.getElementById("mobile_number").value;


                document.getElementById("confirmSummary").innerText = selectedPlanData.summary.replace(/<\/?p>/g, '');
            });
        }




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
                document.getElementById("ModalConfirmation-purchaseplan")
            );

            modal.show();

        });






        ///checkout common vie
        const confirmCheckoutBtn =
            document.getElementById("confirmCheckoutBtn");

        if (confirmCheckoutBtn) {

            confirmCheckoutBtn.addEventListener("click", async function () {

                /* ================== VALIDATION ================== */

                let planCode = "";

                if (selectedPlanData && selectedPlanData.planCodes && selectedPlanData.planCodes.length > 0) {
                    planCode = selectedPlanData.planCodes;
                } 
                let rawAmount = selectedPlanData.amountWithCurrency;
                let cleanAmount = rawAmount.replace(/[^0-9.]/g, "");
                let amount = parseFloat(cleanAmount);
                if (!selectedPlanId || !selectedPlanData) {
                    alert("Please select a plan first");
                    return;
                }    
                const phone = document.getElementById("mobile_number").value.trim();
                const email = document.getElementById("email_address").value.trim();
                if (!phone || !email) {
                    alert("Email and Mobile number required");
                    return;
                }
                var checkoutpageurl = document.getElementById("hdnCheckoutPage").value;
                setCheckoutData({
                    planCodes: selectedPlanData.planCodes, 
                    planName: selectedPlanData.name,
                    email: email,
                    phone: phone,
                    amount: amount,
                    pageUrl: `/main/purchaseplan/${checkoutpageurl}`,
                    pageType: "purchaseplan"
                });
                const data = getCheckoutData();
                try {
                    const res = await fetch("/api/checkoutsession/create-session", {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({
                            email: data.email,
                            phone: data.phone,
                            planCodes: data.planCodes,
                            planName: data.planName, 
                            amount: data.amount,
                            pageUrl: data.pageUrl,
                            pageType: data.pageType
                        })
                    });
                    const result = await res.json();
                    if (!result.sessionId){
                        alert("Unable to start checkout");
                        return;
                    } 
                    window.location.href = `${data.pageUrl}?sid=${result.sessionId}`;
                } catch (err) {
                    console.error("Checkout Error:", err);

                    alert("Something went wrong. Please try again.");
              

                   

                }

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

           
            if (!phoneNumber) {
                isPhoneValid = false;
             
                return false;
            }


            
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
                    isPhoneValid = false;
        
                    return false;
                }
                else {
                    clearError();
                    inputElement.classList.remove("is-invalid");
                    isPhoneValid = true;
            
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








    });
})();