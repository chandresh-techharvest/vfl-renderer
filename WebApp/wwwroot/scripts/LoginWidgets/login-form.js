
(function () {
    document.addEventListener('DOMContentLoaded', function () {
        var widgetContainer = document.querySelector('[data-sf-role="sf-login-form-container"]');
        var formContainer = widgetContainer.querySelector('[data-sf-role="form-container"]');
        var form = formContainer.querySelector("form");
        var redirectUrl = form.querySelector('[name="RedirectUrl"]');
        var errorContainer = widgetContainer.querySelector('[data-sf-role="error-container"]');
        var errorMessageContainer = widgetContainer.querySelector('[data-sf-role="error-message-container"]');
        var ModalVerificationUrl = document.querySelector("input[name='ModalVerificationUrl']");
        var username = document.querySelector("input[name='username']");
        var resendVerification = document.getElementById("resendVerification");
        var successMessageContainer = widgetContainer.querySelector('[data-sf-role="success-message-container"]');
        var resendContainer = widgetContainer.querySelector('[data-sf-role="resend-container"]');
        var visibilityClassElement = document.querySelector('[data-sf-visibility-hidden]');
        var visibilityClassHidden = visibilityClassElement.dataset ? visibilityClassElement.dataset.sfVisibilityHidden : null;
        var invalidClassElement = document.querySelector('[data-sf-invalid]');
        var classInvalidValue = invalidClassElement.dataset && isNotEmpty(invalidClassElement.dataset.sfInvalid) ? invalidClassElement.dataset.sfInvalid : null;
        var classInvalid = classInvalidValue ? processCssClass(classInvalidValue) : null;
        var invalidDataAttr = "data-sf-invalid";
        var PhoneInvalidMessage = "Phone number is incorrect";
       
        function processCssClass(str) {
            var classList = str.split(" ");
            return classList;
        }

        window.handleCredentialResponse = function (response) {
            const idToken = response.credential;
            console.log(idToken);
            // Send the token to the server for verification
            fetch('/api/LoginForm/GoogleCallback', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.getElementById('RequestVerificationToken').value
                },
                body: JSON.stringify({ idToken: idToken })
            })
                .then(response => response.json())
                .then(res => {
                    var status = res.statusCode;
                    console.log(res);
                    if (status === 0 || (status >= 200 && status < 400)) {
                        redirect(redirectUrl.value);
                    }
                    else {
                        alert("Google login failed: " + (res.frontendErrorMessage || res.title || "Unknown error"));
                    }
                });
        }
        window.onload = function () {

            if (window.google && google.accounts && google.accounts.id) {
                google.accounts.id.initialize({
                    client_id: "1084686236185-es6unoi9abpotqrg4mdfamrurg79irq0.apps.googleusercontent.com",
                    callback: window.handleCredentialResponse
                });
                google.accounts.id.renderButton(
                    document.getElementById("g_id_onload"),
                    { theme: "outline", size: "large" }
                );
                /*google.accounts.id.prompt();*/
            } else {
                console.error("Google Identity Services script not loaded.");
            }
        }

        function isNotEmpty(attr) {
            return (attr && attr !== "");
        }

        var serializeForm = function (form) {
            var obj = {};
            var formData = new FormData(form);
            for (var key of formData.keys()) {
                obj[key] = formData.get(key);
            }
            return obj;
        };


        form.addEventListener('submit', function (event) {
            event.preventDefault();
            document.querySelector('#loader').classList.remove('hidden');
            setAntiforgeryTokens().then(res1 => {
                if (validateForm(form)) {
                    //event.target.submit();
                    var url = url || form.attributes['action'].value;
                    var model = serializeForm(form);
                    window.fetch(url, { method: 'POST', body: JSON.stringify(model), headers: { 'Content-Type': 'application/json' } })
                        .then(response => response.json())
                        .then(res => {
                            var status = res.statusCode;
                            console.log(res);
                            if (status === 0 || (status >= 200 && status < 400)) {
                                redirect(redirectUrl.value);                                
                            }
                            else {
                                var isUsernameNotRegistered = res.data.isUsernameNotRegistered;
                                var isUsernameOrPasswordIncorrect = res.data.isUsernameOrPasswordIncorrect;
                                var isEmailNotConfirmed = res.data.isEmailNotConfirmed;                                
                                var serverMessage = '';

                                if (isUsernameOrPasswordIncorrect) {
                                    serverMessage = form.querySelector("input[name='ServerInvalidCredentialMessage']")?.value || "Invalid username or password.";
                                    errorMessageContainer.innerText = serverMessage;
                                    showElement(errorMessageContainer);
                                    showElement(errorContainer);
                                } else if (isUsernameNotRegistered) {
                                    serverMessage = form.querySelector("input[name='ServerUsernameNotRegisteredMessage']")?.value || "The email or phone number entered is not registered.";
                                    errorMessageContainer.innerText = serverMessage;
                                    showElement(errorMessageContainer);
                                    showElement(errorContainer);
                                } else if (isEmailNotConfirmed) {
                                    serverMessage = form.querySelector("input[name='ServerIsEmailNotConfirmed']")?.value || "Email was not Confirmed.";
                                    errorMessageContainer.innerText = serverMessage;
                                    showElement(errorMessageContainer);
                                    showElement(resendContainer);
                                    showElement(errorContainer);
                                } else {
                                    serverMessage = "Incorrect Email, Phone number or Password entered. If you are unable to login, please click on the Forgot Password link below to reset.";
                                    errorMessageContainer.innerText = serverMessage;
                                    showElement(errorMessageContainer);
                                    showElement(errorContainer);
                                }   
                            }
                            document.querySelector('#loader').classList.add('hidden');
                        });
                }
            }, err => {
                showError("Antiforgery token retrieval failed");
            })
        });        
        
         var validateForm = function (form) {
            var isValid = true;
            resetValidationErrors(form);
            resetClientErrorMessage(); 

            var requiredInputs = form.querySelectorAll("input[data-sf-role='required']");

            requiredInputs.forEach(function (input) {
                if (!input.value) {
                    invalidateElement(input);
                    isValid = false;
                }
            });

            if (!isValid) {
                errorMessageContainer.innerText = formContainer.querySelector("input[name='ValidationRequiredMessage']").value;
                showElement(errorMessageContainer);
                return isValid;
            }

            //var emailInput = form.querySelector("input[name='username']");
            //if (!isValidEmail(emailInput.value)) {
            //    errorMessageContainer.innerText = formContainer.querySelector("input[name='ValidationInvalidEmailMessage']").value;
            //    invalidateElement(emailInput);
            //    showElement(errorMessageContainer);
            //    return false;
            //}

            var usernameInput = form.querySelector("input[name='username']");
            var usernameValue = usernameInput.value.trim();

            // starts with alphabet → EMAIL MODE
            if (/^[A-Za-z]/.test(usernameValue)) {

                if (!isValidEmail(usernameValue)) {
                    invalidateElement(usernameInput);
                    errorMessageContainer.innerText =
                        formContainer.querySelector("input[name='ValidationInvalidEmailMessage']").value;
                    showElement(errorMessageContainer);
                    return false;
                }
            }
            // starts with number → PHONE MODE
            else if (/^\d/.test(usernameValue)) {

                if (!usernameValue.match(/^\d{7}$/)) {
                    invalidateElement(usernameInput);
                    errorMessageContainer.innerText = PhoneInvalidMessage;
                    showElement(errorMessageContainer);
                   
                    return false;
                }
            }
            return isValid;
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

        var resetValidationErrors = function (parentElement) {
            var invalidElements = parentElement.querySelectorAll(`[${invalidDataAttr}] `);
            invalidElements.forEach(function (element) {
                if (classInvalid) {
                    element.classList.remove(...classInvalid);
                }

                element.removeAttribute(invalidDataAttr);
            });
        };


        resendVerification.addEventListener('click', function (event) {
            var url = "/api/RegistrationWidget/ResendConfirmationEmail";
            const ResendVerificationModel = {};
            ResendVerificationModel.email = username.value;
            ResendVerificationModel.verificationPageUrl = ModalVerificationUrl.value;
            window.fetch(url, { method: 'POST', body: JSON.stringify(ResendVerificationModel), headers: { 'Content-Type': 'application/json' } })
                .then(response => response.json())
                .then(res => {
                    if (res.statusCode >= 200 && res.statusCode < 400) {
                        if (res.data != null && (!res.data.isNumberRegistered || res.data.isNumberValid)) {
                            Swal.fire({
                                title: "Email is sent successfully",
                                text: "Congratulations!",
                                icon: "success",
                                button: "OK",
                            });                            
                            redirect("/");
                        }
                        else {
                            res.frontendErrorMessage != null ? showError(res.frontendErrorMessage) : showError(res.title);
                            res.developerErrorMessage != null ?
                                console.log(res.developerErrorMessage) : console.log(res.errors.join());
                            showElement(Element);
                            invalidateElement(Element);
                        }
                    }
                    else {
                        res.frontendErrorMessage != null ? showError(res.frontendErrorMessage) : showError(res.title);
                        res.developerErrorMessage != null ?
                            console.log(res.developerErrorMessage) : console.log(res.errors.join());
                        showElement(Element);
                        invalidateElement(Element);                        
                    }
                })
                .catch(error => {
                    showError(error);
                    showElement(Element);
                    invalidateElement(Element);                    
                    console.error('Error fetching data:', error);
                });
        });

        var showElement = function (element) {
            if (element) {

                if (visibilityClassHidden) {
                    element.classList.remove(visibilityClassHidden);
                } else {
                    element.style.display = "";
                }
            }
        };

        var hideElement = function (element) {
            if (element) {

                if (visibilityClassHidden) {
                    element.classList.add(visibilityClassHidden);
                } else {
                    element.style.display = "none";
                }
            }
        };

        hideElement(successMessageContainer);
        hideElement(resendContainer);
        function resetClientErrorMessage() {
            errorMessageContainer.innerText = "";
            hideElement(errorMessageContainer);
        }



        var isValidEmail = function (email) {
            return /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w+)+$/.test(email);
        };
        var redirect = function (redirectUrl) {
            window.location = redirectUrl;
        };
        function setAntiforgeryTokens() {
            return new Promise((resolve, reject) => {
                let xhr = new XMLHttpRequest();
                xhr.open('GET', '/sitefinity/anticsrf');
                xhr.setRequestHeader('X-SF-ANTIFORGERY-REQUEST', 'true')
                xhr.responseType = 'json';
                xhr.onload = function () {
                    const response = xhr.response;
                    if(response != null)
                    {
                        const token = response.Value;
                        document.querySelectorAll("input[name = 'sf_antiforgery']").forEach(i => i.value = token);
                        resolve();
                    }
                    else{
                        resolve();
                    }
                };
                xhr.onerror = function () { reject(); };
                xhr.send();
            });
        }

        function showError(err) {
            document.getElementById('errorMessage').innerText = err;
        }
    });
})();