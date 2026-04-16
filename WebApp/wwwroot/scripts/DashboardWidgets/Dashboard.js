
(function () {
    document.addEventListener('DOMContentLoaded', function () {
        var formContainer = document.querySelector('[data-sf-role="form-container"]');
        //var inputs = formContainer.querySelectorAll("input");
        const SelectPhone = document.getElementById('select-phone');
        const badge = document.getElementById('badge');
        const billamount = document.getElementById('billamount');
        var myCanvas = document.getElementById("sales-donut");
        const modelbody = document.getElementById("modelbody");
        const balances = document.getElementById("balances");
        const totaldonutvalue = document.getElementById("totaldonutvalue");
        const starrating = document.getElementById("star-rating");
        var form = document.querySelector("form");
        const number = document.getElementById("number");
        const responseotpCode = document.getElementById("oCresponse");
        const otpCode = document.getElementById("otpCode");
        //const btnRegister = document.getElementsByName("btnAddDevice");
        const btnRegister = document.querySelector("[name='btnAddDevice']");
        const MyBill = document.getElementById("MyBill");
        const btnRemove = document.getElementById("btnRemove");
        const btnSendOtp = document.getElementById('btnSendOtp');
        const addDeviceModalEl = document.getElementById('ModalAddDevice');
        const otpModalEl = document.getElementById('otpmodal');
        const btnCloseAddDevice = document.getElementById("btnCloseAddDevice");
        const addDeviceModal = bootstrap.Modal.getOrCreateInstance(addDeviceModalEl);
        const otpModal = bootstrap.Modal.getOrCreateInstance(otpModalEl);
        var invalidDataAttr = "data-sf-invalid";
        var errorMessageContainer = formContainer.querySelector('[data-sf-role="error-message-container"]');
        var visibilityClassElement = document.querySelector('[data-sf-visibility-hidden]');
        var visibilityClassHidden = visibilityClassElement.dataset ? visibilityClassElement.dataset.sfVisibilityHidden : null;
        var invalidClassElement = document.querySelector('[data-sf-invalid]');
        var classInvalidValue = invalidClassElement.dataset && isNotEmpty(invalidClassElement.dataset.sfInvalid) ? invalidClassElement.dataset.sfInvalid : null;
        var classInvalid = classInvalidValue ? processCssClass(classInvalidValue) : null;
        let otpCountdown = 30;
        let otpCountdownInterval;
        const otpTimerText = document.getElementById("otpTimer");
        const btnResendOtp = document.getElementById("btnResendOtp");
        const phoneinput = document.querySelector("input[name='number']");
        let otpInterval;
        var inputs = formContainer.querySelectorAll("input");
        

        function closeModalById(id) {
            const el = document.getElementById(id);
            if (!el) return;
            let instance = bootstrap.Modal.getInstance(el);
            if (!instance) {
                instance = bootstrap.Modal.getOrCreateInstance(el);
            }
            instance.hide();
        }
        function parseAmount(value) {
            if (value == null) return 0;
            if (typeof value === "string") {
                value = value.replace(/,/g, "").replace(/[^\d.]/g, "");
            }
            const num = parseFloat(value);
            return isNaN(num) ? 0 : num;
        }

        function getExpiryLabel(expiry) {
            const formatted = formatShortDate(expiry);
            return formatted === '* Recurring balance' ? formatted : 'Expires on ' + formatted;
        }
        function showOtpSuccessMessage() {
            document.getElementById("otpSuccessMessage")?.classList.remove("d-none");
            document.getElementById("otpResendInfoMessage")?.classList.add("d-none");
        }

        function showResendInfoMessage() {
            document.getElementById("otpSuccessMessage")?.classList.add("d-none");
            document.getElementById("otpResendInfoMessage")?.classList.remove("d-none");
        }
  


        function resetAddDeviceForm() {
            const form = document.querySelector('form[action="/api/dashboard/AddDevice"]');
            if (!form) return;
            form.reset();
            const formContainer = form.querySelector('[data-sf-role="form-container"]');
            const errorContainer = form.querySelector('[data-sf-role="error-message-container"]');
            formContainer.querySelectorAll('[data-sf-invalid]').forEach(el => {
                el.classList.remove('is-invalid');
                el.removeAttribute('data-sf-invalid');
            });
            if (errorContainer) {
                errorContainer.classList.add('d-none');
                errorContainer.innerText = '';
            }
            const otpCode = document.getElementById('otpCode');
            const oCresponse = document.getElementById('oCresponse');
            const maskedNumber = document.getElementById('maskedNumber');
            if (otpCode) otpCode.value = '';
            if (oCresponse) oCresponse.value = '';
            if (maskedNumber) maskedNumber.innerText = '00';
            if (window.otpTimerInterval) {
                clearInterval(window.otpTimerInterval);
            }
            const btnResendOtp = document.getElementById('btnResendOtp');
            if (btnResendOtp) btnResendOtp.disabled = true;
        }
  

        function getNearestUpcomingExpiry(balances) {
            if (!balances || balances.length === 0) return null;
            const today = new Date();
            today.setHours(0, 0, 0, 0); // normalize
            const upcomingExpiries = balances.map(b => new Date(b.expiry)).filter(d => !isNaN(d) && d >= today);
            if (upcomingExpiries.length === 0) return null;
            return upcomingExpiries.sort((a, b) => a - b)[0];
        }
        


        var showElement = function (element) {
            if (element) {
                if (visibilityClassHidden) {
                    if (element.classList != null) {
                        element.classList.remove(visibilityClassHidden);
                    }
                } else {
                    element.style.display = "";
                }
            }
        };
                   
              
        function setMaskedNumber(phoneNumber) {
            if (!phoneNumber || phoneNumber.length < 2) return;
            const usernumber = phoneNumber
            document.getElementById("maskedNumber").innerText = usernumber;

        }
         
        function startOtpTimer() {
            clearInterval(otpInterval);
            let timeLeft = 30;
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
                    // ⏱ Timer finished → show resend info
                    showResendInfoMessage();
                    btnResendOtp.disabled = false;
                }
            }, 1000);
        }

        function isNotEmpty(attr) {
            return (attr && attr !== "");
        }

        function processCssClass(str) {
            var classList = str.split(" ");
            return classList;
        }

       
        var myCanvas = document.getElementById("sales-donut");
        var myCanvasContext = myCanvas.getContext("2d");
        var myChart;
        myChart = new Chart(myCanvas, {
            type: "doughnut",
            data: {
                labels: ["Available", "Used"],
                datasets: [
                    {
                        data: [60, 60],
                        backgroundColor: ["rgb(90, 102, 241)", "rgb(96, 165, 250)"],
                        borderWidth: 0,
                    },
                ],
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false,
                    },
                },
                cutout: 90,
            },
        });

        function formatShortDate(dateValue) {
            if (!dateValue) return '* Recurring balance';
            const date = new Date(dateValue);
            if (isNaN(date.getTime())) {
                console.warn('Invalid API date:', dateValue);
                return '* Recurring balance';  // Fallback to raw
            }
            const year = date.getFullYear();
            if (year <= 1970) {
                return '* Recurring balance';
            }
            return date.toLocaleString();
        }
                
        document.getElementById('btnCloseData').addEventListener('click', resetAddDeviceForm);

        btnCloseAddDevice.addEventListener('click', function (event) {
            event.preventDefault();
            const modalElement = document.getElementById('ModalAddDevice');
            const emailVerificationModal = new bootstrap.Modal(modalElement);
            emailVerificationModal.hide();
            inputs.forEach(input => {
                if (input.type == "text" || input.type == "number") {
                    input.value = '';
                }
            });
            form.reset();
            if (errorMessageContainer) {
                errorMessageContainer.classList.add("d-none");
                errorMessageContainer.innerHTML = "";
            }
        });

        SelectPhone.addEventListener('change', function () {
            var selectedPhone = SelectPhone.value;
            fetch("/api/Dashboard/GetProfileInformation?selectedNumber=" + encodeURIComponent(selectedPhone));
            var url = "/api/Dashboard/GetBalanceInformation"
            window.fetch(url, { method: 'POST', body: JSON.stringify(selectedPhone), headers: { 'Content-Type': 'application/json' } })
                .then(response => response.json())
                .then(res => {
                    if (res.statusCode >= 200 && res.statusCode < 400) {
                        if (res.data != null) {
                            var isPostPay = res.data.isPostPay;
                            var stars = '';
                            for (var i = 1; i < 6; i++) {
                                if (i <= res.data.vodaStarRating) {
                                    stars += '<span class="filled">&#9733;</span>';
                                }
                                else {
                                    stars += '<span>&#9733;</span>';
                                }
                            }
                            starrating.innerHTML = stars;
                            var balanceplan = '<div class=""><div class="fw-normal fs-18">Current Plan</div><div class="text-muted fs-15" id="currentPlanName"> ' + res.data.currentPlan + '</div></div>';
                            modelbody.innerHTML = '';
                            if (isPostPay) {
                                badge.innerHTML = "<i class='ti ti - list - details'></i>Post Pay";
                                const pendingbalances = res.data.postpayBalances.balances.filter(balance => {
                                    return balance.balanceType === "Data"
                                });
                                MyBill.innerHTML = '<div class="d-flex align-items-center justify-content-between"><div class="d-flex align-items-center" ><span class="avatar   bg-secondary-transparent"><svg class="fill-secondary" style="height: 30px;" xmlns="http://www.w3.org/2000/svg" enable-background="new 0 0 24 24" viewBox="0 0 24 24"><path class="fill-secondary" d="M9.5,7h7C16.776123,7,17,6.776123,17,6.5S16.776123,6,16.5,6h-7C9.223877,6,9,6.223877,9,6.5S9.223877,7,9.5,7z M7.5,11h9c0.276123,0,0.5-0.223877,0.5-0.5S16.776123,10,16.5,10h-9C7.223877,10,7,10.223877,7,10.5S7.223877,11,7.5,11z M20.5,2H3.4993896C3.2234497,2.0001831,2.9998169,2.223999,3,2.5v19c-0.000061,0.1124268,0.0378418,0.2216187,0.1074829,0.3098755c0.1710205,0.2167358,0.4853516,0.2537231,0.7020874,0.0827026l2.8652344-2.2617188l2.3583984,1.7695312c0.1777954,0.1328125,0.421814,0.1328125,0.5996094,0L12,19.625l2.3671875,1.7753906c0.1777954,0.1328125,0.421814,0.1328125,0.5996094,0l2.3583984-1.7695312l2.8652344,2.2617188C20.2785034,21.9623413,20.3876343,22.0002441,20.5,22h0.0006104C20.7766113,21.9998169,21.0001831,21.7759399,21,21.5V2.4993896C20.9998169,2.2234497,20.776001,1.9998169,20.5,2z M20,20.46875l-2.3574219-1.8613281c-0.0882568-0.069519-0.1972656-0.1072998-0.3095703-0.1074219c-0.1080933-0.000061-0.2132568,0.0349121-0.2998047,0.0996094L14.6669922,20.375l-2.3671875-1.7753906c-0.1777954-0.1328125-0.421814-0.1328125-0.5996094,0L9.3330078,20.375l-2.3662109-1.7753906c-0.1817017-0.1348877-0.4311523-0.1317139-0.609375,0.0078125L4,20.46875V3h16V20.46875z M7.5,15h9c0.276123,0,0.5-0.223877,0.5-0.5S16.776123,14,16.5,14h-9C7.223877,14,7,14.223877,7,14.5S7.223877,15,7.5,15z"></path></svg></span><h6 class="mb-0 text-default fw-bold ms-3 fs-18">My Bill</h6></div></div><div class="mt-2"><h2 class="text-default mb-0 fs-24 fw-semibold" id="billamount">' + res.data.postpayBalances.unbilledAmount + '</h2><p class="mt-1 mb-0 text-muted">Amount displayed is VAT exclusive</p></div>';
                                var items = '';
                                var previewItems = '';
                                previewItems += balanceplan;
                                res.data.postpayBalances.balances.forEach((balance, index) => {
                                    var percentage = 0;
                                    if (balance.totalAmount !== 0) {
                                        percentage = Math.round(((balance.balanceRemainingAmount / balance.totalAmount) * 100));
                                    }
                                    var balanceRemainingAmount = balance.balanceRemainingAmount;
                                    var totalAmount = balance.totalAmount;
                                    if (balance.balanceType === "Data") {
                                        balanceRemainingAmount = formatBytes(balance.balanceRemainingAmount);
                                        totalAmount = formatBytes(balance.totalAmount);
                                    }
                                    else if (balance.balanceType === "Money") {
                                        balanceRemainingAmount = balance.balanceRemainingMeasurement + " " + balance.balanceRemainingAmount;
                                        totalAmount = balance.balanceRemainingMeasurement + " " + balance.totalAmount;
                                    }
                                    else {
                                        balanceRemainingAmount = balance.balanceRemainingAmount + " " + balance.balanceRemainingMeasurement;
                                        totalAmount = balance.totalAmount + " " + balance.balanceRemainingMeasurement;
                                    }
                                    var item = '<div class="mt-5" style="margin-top: 75px !important;"><div class="progress progress-sm progress-custom mb-5 progress-animate" role="progressbar" aria-valuenow="' + percentage + '" aria-valuemin="0" aria-valuemax="100"><h6 class="progress-bar-title">' + balance.name + '</h6><div class="progress-bar" style="width: ' + percentage + '%"><div class="progress-bar-value">' + percentage + '%</div></div><span class="progress-total-prepay mb-1-2-3">' + balanceRemainingAmount + '/ ' + totalAmount + '<div class="text-muted fs-12 text-center"> ' + getExpiryLabel(balance.expiry) + '</div></span></div></div>';
                                    items += item;
                                    if (index <= 1) {
                                        previewItems += item;
                                    }
                                });
                                modelbody.innerHTML = items;
                                balances.innerHTML = previewItems;
                                CreateDonut(pendingbalances);
                            }
                            else {
                                badge.innerHTML = "<i class='ti ti - list - details'></i>Pre Pay";
                                const pendingbalances = [];
                                res.data.prepayBalance.balances.forEach(balance => {
                                    if (balance.balanceType === "Data") {
                                        pendingbalances.push(balance);
                                    }
                                });
                                billamount.innerText = res.data.prepayBalance.unbilledAmount;
                                var items = '';
                                var previewItems = '';
                                previewItems += balanceplan;
                                res.data.prepayBalance.balances.forEach((balance, index) => {
                                    var percentage = 0;
                                    if (balance.totalAmount !== 0) {
                                        percentage = Math.round((balance.balanceRemainingAmount / balance.totalAmount) * 100);
                                    }
                                    var balanceRemainingAmount = balance.balanceRemainingAmount;
                                    var totalAmount = balance.totalAmount;
                                    if (balance.balanceType === "Data") {
                                        balanceRemainingAmount = formatBytes(balance.balanceRemainingAmount);
                                        totalAmount = formatBytes(balance.totalAmount);
                                    }
                                    else if (balance.balanceType === "Money") {
                                        balanceRemainingAmount = balance.balanceRemainingMeasurement + " " + (balance.balanceRemainingAmount);
                                        totalAmount = balance.balanceRemainingMeasurement + " " + (balance.totalAmount);
                                    }
                                    else {
                                        balanceRemainingAmount = balance.balanceRemainingAmount + " " + balance.balanceRemainingMeasurement;
                                        totalAmount = balance.totalAmount + " " + balance.balanceRemainingMeasurement;
                                    }
                                    var item = '<div class="mt-5" style="margin-top: 80px !important;"><div class="progress progress-sm progress-custom mb-5 progress-animate" role="progressbar" aria-valuenow="' + percentage + '" aria-valuemin="0" aria-valuemax="100"><h6 class="progress-bar-title">' + balance.name + '</h6><div class="progress-bar" style="width: ' + percentage + '%"><div class="progress-bar-value">' + percentage + '%</div></div><span class="progress-total-prepay mb-1-2-3">' + balanceRemainingAmount + '/ ' + totalAmount + '<div class="text-muted fs-12 text-center"> ' + getExpiryLabel(balance.expiry) + '</div></span></div></div>';
                                    items += item;
                                    if (index <= 1) {
                                        previewItems += item;
                                    }
                                });
                                modelbody.innerHTML = items;
                                balances.innerHTML = previewItems;
                                CreateDonut(pendingbalances);
                                const onNetMoneyBalances = res.data.prepayBalance.balances.filter(b => b.balanceType === "Money" && b.name === "On Net Money");
                                const purchasedCreditBalances = res.data.prepayBalance.balances.filter(b => b.balanceType === "Money" && b.name === "Purchased Credit");   
                                const onNetExpiry = getNearestUpcomingExpiry(onNetMoneyBalances);
                                const purchasedExpiry = getNearestUpcomingExpiry(purchasedCreditBalances);
                                const freeMoneyTotal = res.data.prepayBalance.balances.filter(b => b.balanceType === "Money" && b.name === "On Net Money").reduce((sum, b) => sum + parseAmount(b.balanceRemainingAmount), 0);
                                const purchasedCreditTotal = res.data.prepayBalance.balances.filter(b => b.balanceType === "Money" && b.name === "Purchased Credit").reduce((sum, b) => sum + parseAmount(b.balanceRemainingAmount), 0);
                                let billHtml = `<div class="d-flex align-items-center justify-content-between">   <div class="d-flex align-items-center"></div></div>`;
                                if (freeMoneyTotal > 0) {
                                    billHtml += ` <div class="mt-2"> <h2 class="text-default mb-0 fs-24 fw-semibold"> Free Money :  $${freeMoneyTotal.toFixed(2)} <p class="text-muted fs-12 mb-0"> ${getExpiryLabel(onNetExpiry)} </p> </div>`;
                                }
                                if (purchasedCreditTotal >= 0) {
                                    billHtml += `<div class="mt-2"> <h2 class="text-default mb-0 fs-24 fw-semibold"> Real Money :  $${purchasedCreditTotal.toFixed(2)} <p class="text-muted fs-12 mb-0"> ${getExpiryLabel(purchasedExpiry)}</p></h2 </div>`;
                                }
                                MyBill.innerHTML = billHtml;
                            }
                        }
                    }
                    document.querySelector('#loader').classList.add('hidden');
                });
        });

        // Create a new 'change' event
        const changeEvent = new Event('change');
       
        function formatBytes(bytes, decimals = 2) {
            if (bytes === 0) return '0 Bytes';
            const k = 1024;
            const dm = decimals < 0 ? 0 : decimals;
            const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
        }
        function formatBytesWithouttags(bytes, decimals = 2) {
            if (bytes === 0) return '0 Bytes';
            const k = 1024;
            const dm = decimals < 0 ? 0 : decimals;
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            const value = bytes / Math.pow(k, i);
            return parseFloat(value.toFixed(dm));
        }

        function CreateDonut(balances) {
            // If balances array is empty or null
            if (!balances || balances.length === 0) {
                totaldonutvalue.innerHTML = "0 GB";
                myChart.data.datasets[0].data = [0.0001, 1]; // tiny value to force render
                myChart.data.datasets[0].backgroundColor = ['#6c63ff', '#e0e0e0']; // purple + grey
                myChart.update();
                return;
            }
            let totalAmount = 0;
            let balanceRemainingAmount = 0;
            // Sum up total and remaining balances
            balances.forEach(b => {
                totalAmount += Number(b.totalAmount || 0);
                balanceRemainingAmount += Number(b.balanceRemainingAmount || 0);
            });
            const used = totalAmount - balanceRemainingAmount;
            // Update total value text
            totaldonutvalue.innerHTML = formatBytes(balanceRemainingAmount);
            const totalGB = formatBytesWithouttags(totalAmount);
            const remainingGB = formatBytesWithouttags(balanceRemainingAmount);
            const usedGB = totalGB - remainingGB;
            // If total is zero, render placeholder donut
            if (totalAmount === 0) {
                myChart.data.datasets[0].data = [0.0001, 1]; // tiny slice for Chart.js to render
                myChart.data.datasets[0].backgroundColor = ['#6c63ff', '#e0e0e0']; // purple + grey
            } else {
                myChart.data.datasets[0].data = [
                    remainingGB, // available
                    usedGB       // used
                ];
                myChart.data.datasets[0].backgroundColor = ['#6c63ff', '#84a9ff']; // normal colors
            }
            myChart.update();  
        }

        function handleDeviceChangeSuccess(res) {
            if (res && res.statusCode >= 200 && res.statusCode < 400) {
                LoadDevices();
            }
        }

        function RemoveDevice(e) {
            if (!e.target.classList.contains('btnRemove')) return;
            const number = e.target.dataset.number;
            Swal.fire({
                title: "Are you sure?",
                text: "Do you really want to remove this device?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonColor: "#6c757d",
                confirmButtonText: "Yes, remove it",
                cancelButtonText: "Cancel"
            }).then((result) => {
                if (result.isConfirmed) {
                    DeleteDevice(number);
                }
            });
        }
   
        function DeleteDevice(number) {
            var selectedPhone = number;
            var url = "/api/Dashboard/RemoveDevice"
            window.fetch(url, { method: 'POST', body: JSON.stringify(selectedPhone), headers: { 'Content-Type': 'application/json' } })
                .then(response => response.json())
                .then(res => {
                    if (res.statusCode >= 200 && res.statusCode < 400) {
                        Swal.fire({
                            title: "Device Removed Successfully!!!",
                            text: "You clicked the button!",
                            icon: "success",
                            button: "Aww yiss!",
                        });
                        handleDeviceChangeSuccess(res);
                    }
                });
        }

        function LoadDevices() {
            document.querySelector('#loader').classList.remove('hidden');
            const newUserDiv = document.getElementById('newuser');
            const registerUserDiv = document.getElementById('RegisterUser');
            document.querySelector('.list-group').innerHTML = '';
            url = "/api/Dashboard/GetProfileInformation";
            window.fetch(url, { method: 'GET', headers: { 'Content-Type': 'application/json' } })
                .then(response => response.json())
                .then(res => {
                    if (res.statusCode === 0 || (res.statusCode >= 200 && res.statusCode < 400)) {
                        const devices = res.data.devices || [];
                        const deviceCount = devices.length;
                        if (deviceCount === 0) {
                            newUserDiv.classList.remove('d-none');
                            registerUserDiv.classList.add('d-none');
                            return;
                        }
                        newUserDiv.classList.add('d-none');
                        registerUserDiv.classList.remove('d-none');
                        SelectPhone.innerHTML = '';
                        let selectedNumber = null;
                        res.data.devices.forEach(device => {
                            const tmpl = document.querySelector('#deviceTmpl').content.cloneNode(true);
                            tmpl.querySelector('.devicename').textContent = device.name;
                            tmpl.querySelector('.devicenumber').textContent = device.number;
                            const btn = tmpl.querySelector('.btnRemove');
                            btn.dataset.number = device.number;
                            document.querySelector('.list-group').appendChild(tmpl);
                            const option = document.createElement('option');
                            option.value = device.number;  // or item.value
                            option.textContent = '+679 ' + device.number;  // Display text
                            if (device.isSelected) {
                                option.selected = true;
                                selectedNumber = device.number;
                            }
                            SelectPhone.appendChild(option);
                        });
                        if (!selectedNumber && SelectPhone.options.length > 0) {
                            SelectPhone.selectedIndex = 0;
                        }
                        SelectPhone.dispatchEvent(changeEvent);
                        document.querySelectorAll('#btnRemove').forEach(button => {
                            button.addEventListener('click', RemoveDevice);
                        });
                    }
                    else {
                        window.location.href = "/login";
                    }
                });
        }   
        LoadDevices();

        form.addEventListener('submit', function (event) {
            event.preventDefault();
            if (!validateForm(form)) {
                //btnUpdate.disabled = true;
                return;
            }
            setAntiforgeryTokens().then(res => {
                if (validateForm(form)) {
                    submitFormHandler(form, null, postAddAction, onAddError);
                }
            }, err => {
                showError("Antiforgery token retrieval failed");
            })
        });
           
        var submitFormHandler = function (form, url, onSuccess, onError) {
            url = url || form.attributes['action'].value;
            var model = serializeForm(form);
            window.fetch(url, { method: 'POST', body: JSON.stringify(model), headers: { 'Content-Type': 'application/json' } })
                .then(response => response.json())
                .then(res => {
                    if (res.data.isAdded && res.data.isCodeValid) {
                        closeModalById("otpmodal");
                        Swal.fire({
                            title: "Device Added Successfully!!!",
                            text: "Congratulations!",
                            icon: "success",
                            button: "OK",
                        });

                        // alert("Device Added Successfully!!");
                        //otpModal.hide();
                        handleDeviceChangeSuccess(res);
                        const modalElement = document.getElementById('ModalAddDevice');
                        const emailVerificationModal = new bootstrap.Modal(modalElement);
                        emailVerificationModal.hide();
                        form.reset();
                    }
                    else {
                        Swal.fire({
                            title: "We couldn’t add the device",
                            text: "please enter a valid OTP",
                            icon: "info",
                            button: "OK",
                        });


                    }
                });
        };

        var postAddAction = function () {
            showSuccess("Device added successfully");
            form.reset();
        };

        btnSendOtp.addEventListener('click', async function () {

            if (!validateForm(form)) return;
            if (number.value.length !== 7) return;

            // 🚫 Prevent multiple clicks
            btnSendOtp.disabled = true;
          

            try {

                
                

                // 🔹 Call API in background
                const res = await fetch("/api/Dashboard/SendOTPCode", {
                    method: 'POST',
                    body: JSON.stringify(number.value),
                    headers: { 'Content-Type': 'application/json' }
                }).then(r => r.json());

                if (!res?.data?.code) {
                    throw new Error("OTP send failed");
                }

                responseotpCode.value = res.data.code;
                closeModalById("ModalAddDevice");

                bootstrap.Modal.getOrCreateInstance(
                    document.getElementById("otpmodal")
                ).show();
                startOtpTimer();
                setMaskedNumber(number.value);


            } catch (err) {

                otpModal.hide();

                Swal.fire({
                    title: "OTP Failed",
                    text: "We couldn't send OTP. Please try again.",
                    icon: "error"
                });

                btnSendOtp.disabled = false;
               
            }
        });

        ///resend otp api
        btnResendOtp.addEventListener("click", function () {

            btnResendOtp.disabled = true;

            var url = "/api/Dashboard/SendOTPCode";

            fetch(url, {
                method: 'POST',
                body: JSON.stringify(number.value),
                headers: { 'Content-Type': 'application/json' }
            })
                .then(r => r.json())
                .then(res => {

                    if (!res?.data?.code) {
                        alert("OTP resend failed");
                        btnResendOtp.disabled = false;
                        return;
                    }

                    responseotpCode.value = res.data.code;



                    startOtpTimer();
                });
        });

        var onAddError = function (errorMessage, status) {
            errorMessageContainer.innerText = errorMessage;
            showError(errorMessage);
            showElement(errorMessageContainer);
        };

        var validateForm = function (form) {
            let isValid = true;
            let messages = [];
            resetValidationErrors(form);
            hideElement(errorMessageContainer);

            const requiredInputs = form.querySelectorAll("[data-sf-role='required']");

            requiredInputs.forEach(input => {
                if (!input.value || input.value.trim() === "") {
                    // invalidateElement(input);
                    isValid = false;

                    // 🔥 Get label text
                    let fieldName = "";

                    if (input.name === "number") {
                        fieldName = "Phone number";
                    }
                    else if (input.name === "name") {
                        fieldName = "Device name";
                    }

                    messages.push(`* ${fieldName} is required`);
                }
            });

            if (!isValid) {
                errorMessageContainer.innerHTML = messages.join("<br>");
                showElement(errorMessageContainer);
            }

            return isValid;
        };

        phoneinput.addEventListener('blur', async function () {
            hideElement(errorMessageContainer);

            if (phoneinput.value.length > 7)
                return showValidationError(" * The number entered is not a valid Vodafone number.");

            btnSendOtp.disabled = true;

            try {


                // Check if number is valid
                const valRes = await fetch("/api/Validation/CheckNumberIsValid", {
                    method: 'POST',
                    body: JSON.stringify(phoneinput.value),

                    headers: { 'Content-Type': 'application/json' }
                }).then(r => r.json());

                if (valRes.data?.isNumberValid === false) {



                    showValidationError("* The number entered is not a valid Vodafone number.");
                    // invalidateElement(phoneinput);
                } else {
                    hideElement(errorMessageContainer);
                    btnSendOtp.disabled = false;

                }

            } catch (err) {
                console.error("Phone validation error:", err);
                btnSendOtp.disabled = false; // allow retry
            }
        });

        var resetValidationErrors = function (parentElement) {
            var invalidElements = parentElement.querySelectorAll(`[${invalidDataAttr}]`);
            invalidElements.forEach(function (element) {
                if (classInvalid) {
                    element.classList.remove(...classInvalid);
                }

                element.removeAttribute(invalidDataAttr);
            });
        };
        function showValidationError(message) {
            if (!errorMessageContainer) return;

            errorMessageContainer.innerText = message;
            errorMessageContainer.classList.remove("d-none");
        }

        var hideElement = function (element) {
            if (element) {

                if (visibilityClassHidden) {
                    element.classList.add(visibilityClassHidden);
                } else {
                    element.style.display = "none";
                }
            }
        };

        var invalidateElement = function (element) {
            if (element) {

                if (classInvalid) {
                    element.classList.add(...classInvalid);
                }

                //adding data attribute for queries, to be used instead of a class
                element.setAttribute(invalidDataAttr, "");
            }
        };

        otpCode.addEventListener('input', function () {
            if (otpCode.value.length == 6) {
                if (otpCode.value === responseotpCode.value) {
                    invalidateElement(input);
                    isValid = false;
                }
            }
        });
        var serializeForm = function (form) {
            var obj = {};
            var formData = new FormData(form);
            for (var key of formData.keys()) {
                obj[key] = formData.get(key);
            }
            return obj;
        };

        function setAntiforgeryTokens() {
            return new Promise((resolve, reject) => {
                let xhr = new XMLHttpRequest();
                xhr.open('GET', '/sitefinity/anticsrf');
                xhr.setRequestHeader('X-SF-ANTIFORGERY-REQUEST', 'true')
                xhr.responseType = 'json';
                xhr.onload = function () {
                    const response = xhr.response;
                    if (response != null) {
                        const token = response.Value;
                        document.querySelectorAll("input[name = 'sf_antiforgery']").forEach(i => i.value = token);
                        resolve();
                    }
                    else {
                        resolve();
                    }
                };
                xhr.onerror = function () { reject(); };
                xhr.send();
            });
        }


        //function hideLoader() {
        //    const loader = document.getElementById("loader");
        //    loader.classList.add("d-none")
        //};
      


    });
})();
