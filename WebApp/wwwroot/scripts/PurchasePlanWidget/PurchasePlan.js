(function () {
    document.addEventListener("DOMContentLoaded", function () {
        const pageSize = 8;
        let currentPage4g = 1, currentPage5g = 1;
        let plans4g = [], plans5g = [], currentPlans = [];
        let selectedPlanType = "", selectedPaymentMethod = "";
        let selectedPlanId = null, selectedPlanData = null;
        let isLoading = false, isPhoneValid = false;

        const planTypeFilter = document.getElementById("planTypeFilter");
        const paymentTypeFilter = document.getElementById("paymentTypeFilter");
        const planTypeFilter5g = document.getElementById("planTypeFilter5g");
        const paymentTypeFilter5g = document.getElementById("paymentTypeFilter5g");
        const sortFilter = document.getElementById("sortFilter");
        const sortFilter5g = document.getElementById("sortFilter5g");
        const btnContinue = document.getElementById("btnContinue");
        const phoneInput = document.getElementById("mobile_number");
        const billingSection = document.getElementById("billingAddressSection");
        const paymentRadios = document.querySelectorAll("input[name='paymentMethod']");
        const confirmModal = document.getElementById("ModalConfirmation-purchaseplan");
        const confirmCheckoutBtn = document.getElementById("confirmCheckoutBtn");
        const errorBox = document.getElementById("error-message");

        const showElement = el => el.style.display = "flex";
        const hideElement = el => el.style.display = "none";

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

        function showView(view) {
            const defaultView = document.getElementById("purchaseplan-default-view");
            const checkoutView = document.getElementById("selected-plan-view");
            if (!defaultView || !checkoutView) return;
            if (view === "default") {
                showElement(defaultView); hideElement(checkoutView);
            } else if (view === "checkout") {
                showElement(checkoutView); hideElement(defaultView);
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

        if (billingSection) billingSection.style.display = "none";
        paymentRadios.forEach(radio => {
            radio.addEventListener("change", function () {
                billingSection.style.display = this.value === "CARD" ? "block" : "none";
            });
        });

        function showError(msg) {
            errorBox.innerText = msg;
            errorBox.style.display = "block";
        }
        function clearError() {
            errorBox.innerText = "";
            errorBox.style.display = "none";
        }
       
        

        if (btnContinue) {
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

           
        }

        async function PhoneBlurHandler(phoneNumber, inputElement) {
            clearError();
            inputElement.classList.remove("is-invalid");
            if (phoneNumber.length !==7) {
                showError(" * The number entered is not a valid Vodafone number.");
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

        function bindPlanRadioEvents() {
            document.querySelectorAll(".plan-radio").forEach(radio => {
                radio.addEventListener("change", function () {
                    selectedPlanId = this.value;
                    loadPlanDetails(selectedPlanId);
                });
            });
        }

        async function loadPlansByType(planType) {
            const number = phoneInput.value.trim();
            try {
                const res = await fetch(`/PurchasePlan/GetPlansByType?number=${number}&planType=${planType}`);
                renderPlans(await res.json());
            } catch (err) { console.error("Load Plans Error:", err); }
        }

        async function loadPlanDetails(planId) {
            const number = phoneInput.value.trim();
            try {
                const res = await fetch(`/PurchasePlan/GetPlanById?number=${number}&planId=${planId}`);
                selectedPlanData = await res.json();
            } catch (err) { console.error("API Error", err); }
        }

        async function loadPlansByPaymentMethod(method) {
            const number = phoneInput.value.trim();
            try {
                const res = await fetch(`/PurchasePlan/GetPlansByPaymentMethod?number=${number}&paymentMethod=${method}`);
                renderPlans(await res.json());
            } catch (err) { console.error("Load Payment Plans Error:", err); }
        }

        async function loadPlansByTypeAndPayment(planType, paymentMethod) {
            const number = phoneInput.value.trim();
            try {
                const res = await fetch(`/PurchasePlan/GetPlansByTypeAndPayment?number=${number}&planType=${planType}&paymentMethod=${paymentMethod}`);
                if (!res.ok) throw new Error("HTTP Error " + res.status);
                renderPlans(await res.json());
            } catch (err) { console.error("Combined Load Error:", err); }
        }

        if (sortFilter) sortFilter.addEventListener("change", function () { applySorting(this.value); });
        if (sortFilter5g) sortFilter5g.addEventListener("change", function () { applySorting(this.value); });

        function getNumericPrice(plan) {
            return plan.amountWithCurrency ? parseFloat(plan.amountWithCurrency.replace(/[^0-9.]/g, "")) || 0 : 0;
        }
        function getNumericData(plan) {
            if (!plan) return 0;
            let text = plan.summary ? plan.summary.replace(/<[^>]+>/g, "") : plan.name || "";
            let match = text.match(/([0-9.]+)\s*GB/i);
            return match ? parseFloat(match[1]) : 0;
        }
        function applySorting(sortValue) {
            if (!currentPlans || currentPlans.length === 0) return;
            let sorted = [...currentPlans];
            switch (sortValue) {
                case "price_asc": sorted.sort((a, b) => getNumericPrice(a) - getNumericPrice(b)); break;
                case "price_desc": sorted.sort((a, b) => getNumericPrice(b) - getNumericPrice(a)); break;
                case "data_asc": sorted.sort((a, b) => getNumericData(a) - getNumericData(b)); break;
                case "data_desc": sorted.sort((a, b) => getNumericData(b) - getNumericData(a)); break;
            }
            renderPlans(sorted);
        }

        async function loadAllPlans(number) {
            if (isLoading) return;
            isLoading = true;
            try {
                const res = await fetch(`/PurchasePlan/GetAllPlans?number=${number}`);
                if (!res.ok) throw new Error("Failed to load plans");
                renderPlans(await res.json());
            } catch (err) { console.error(err); }
            finally { isLoading = false; }
        }

        if (planTypeFilter5g) planTypeFilter5g.addEventListener("change", function () { selectedPlanType = this.value || ""; applyFilters(); });
        if (paymentTypeFilter5g) paymentTypeFilter5g.addEventListener("change", function () { selectedPaymentMethod = this.value || ""; applyFilters(); });
        if (planTypeFilter) planTypeFilter.addEventListener("change", function () { selectedPlanType = this.value || ""; applyFilters(); });
        if (paymentTypeFilter) paymentTypeFilter.addEventListener("change", function () { selectedPaymentMethod = this.value || ""; applyFilters(); });

        function applyFilters() {
            const number = phoneInput.value.trim();
            const planType = selectedPlanType.trim();
            const payment = selectedPaymentMethod.trim();
            if (planType && payment) return loadPlansByTypeAndPayment(planType, payment);
            if (planType) return loadPlansByType(planType);
            if (payment) return loadPlansByPaymentMethod(payment);
            loadAllPlans(number);
        }

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
            currentPage4g = 1; currentPage5g = 1;
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

        if (confirmModal) {
            confirmModal.addEventListener("show.bs.modal", async function () {
                const luData = await getDecryptedCookie();
                let fullName = "Customer";
                if (luData) {
                    try {
                        fullName = (luData.firstName || "") + " " + (luData.lastName || "");
                        fullName = fullName.trim() || "Customer";
                    } catch (e) { console.error("luData parse error", e); }
                }
                document.getElementById("confirmUser").innerText = fullName;
                if (!selectedPlanData) return;
                document.getElementById("confirmPlan").innerText = selectedPlanData.name;
                document.getElementById("confirmPrice").innerText = selectedPlanData.amountWithCurrency;
                document.getElementById("confirmNumber").innerText = phoneInput.value;
                document.getElementById("confirmNumber").innerText = phoneInput.value;
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


        if (confirmCheckoutBtn) {
            confirmCheckoutBtn.addEventListener("click", async function () {
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
                const phone = phoneInput.value.trim();
                if (!phone) {
                    alert("Mobile number required");
                    return;
                }
                const luData = await getDecryptedCookie();
                const checkoutpageurl = document.getElementById("hdnCheckoutPage").value;
                setCheckoutData({
                    planCodes: selectedPlanData.planCodes, 
                    planName: selectedPlanData.name,

                    email: luData.email,
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
                    if (!result.sessionId) {
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
    });
})();
