(function () {
    document.addEventListener('DOMContentLoaded', function () {        
        var form = document.querySelector("form");
        var inputs = formContainer.querySelectorAll("input");
        var btnUpdate = form.querySelector("input[name='btnUpdate']");
        var errorMessageContainer = form.querySelector('[data-sf-role="error-message-container"]');
        var successMessageContainer = form.querySelector('[data-sf-role="success-message-container"]');
        var confirmRegistrationMessageContainer = widgetContainer.querySelector('[data-sf-role="confirm-message-container"]');
        var visibilityClassElement = document.querySelector('[data-sf-visibility-hidden]');
        var visibilityClassHidden = visibilityClassElement.dataset ? visibilityClassElement.dataset.sfVisibilityHidden : null;
        var invalidClassElement = document.querySelector('[data-sf-invalid]');
        var classInvalidValue = invalidClassElement.dataset && isNotEmpty(invalidClassElement.dataset.sfInvalid) ? invalidClassElement.dataset.sfInvalid : null;
        var classInvalid = classInvalidValue ? processCssClass(classInvalidValue) : null;
        //btnRegister.disabled = true;
        var invalidDataAttr = "data-sf-invalid";

        function isNotEmpty(attr) {
            return (attr && attr !== "");
        }

        function processCssClass(str) {
            var classList = str.split(" ");
            return classList;
        }

        form.addEventListener('submit', function (event) {
            event.preventDefault();

            if (!validateForm(form)) {
                btnUpdate.disabled = true;
                return;
            }
            btnUpdate.disabled = false;
            setAntiforgeryTokens().then(res => {
                if (validateForm(form)) {
                    submitFormHandler(form, null, postRegistrationAction, onRegistrationError);
                }
            }, err => {
                showError("Antiforgery token retrieval failed");
            })
        });


        
        // Field-level validation on blur
        inputs.forEach(input => {
            input.addEventListener('blur', function (event) {
                event.preventDefault();
                validateField(input);
            });
        });

        function validateField(input) {
            let isValid = true;
            resetValidationErrors(form);

            if (input.hasAttribute('data-sf-role') && input.getAttribute('data-sf-role') === 'required' && !input.value) {
                invalidateElement(input);
                showError('This field is required.');
                btnRegister.disabled = true;
                return false;
            }

            if (input.name === 'Email') {
                if (!isValidEmail(input.value)) {
                    invalidateElement(input);
                    showError('Invalid email address.');
                    btnRegister.disabled = true;
                    return false;
                }
                // Async email validation
                emailAsyncValidation(input.value, input);
            }

            // Password match validation
            if (input.type === 'password') {
                let passwordFields = form.querySelectorAll("[type='password']");
                if (passwordFields.length === 2 && passwordFields[0].value !== passwordFields[1].value) {
                    invalidateElement(passwordFields[1]);
                    showError('Passwords do not match.');
                    btnRegister.disabled = true;
                    return false;
                }
            }

            btnRegister.disabled = false;
            hideElement(errorMessageContainer);
            return isValid;
        }

        

        function emailAsyncValidation(value, inputElement) {
            fetch('/api/Validation/CheckEmailIsRegistered', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: value
            })
                .then(response => response.json())
                .then(res => {
                    if (res.statusCode > 0 && !res.isException) {
                        btnRegister.disabled = false;
                        hideElement(errorMessageContainer);
                    } else {
                        invalidateElement(inputElement);
                        showError(res.frontendErrorMessage || 'Email is already registered or invalid.');
                        btnRegister.disabled = true;
                    }
                })
                .catch(() => {
                    invalidateElement(inputElement);
                    showError('Email validation failed.');
                    btnRegister.disabled = true;
                });
        }



        var postAction = function () {
            //var action = formContainer.querySelector("input[name='PostRegistrationAction']").value;
            //var activationMethod = formContainer.querySelector("input[name='ActivationMethod']").value;

            //if (action === 'ViewMessage') {
            //    if (activationMethod == "AfterConfirmation") {
            //        showSuccessAndConfirmationSentMessage();
            //    } else {
            //        showSuccessMessage();
            //    }
            //} else if (action === 'RedirectToPage') {
            //    var redirectUrl = formContainer.querySelector("input[name='RedirectUrl']").value;
            //    redirect(redirectUrl);
            //}
        };

        var onRegistrationError = function (errorMessage, status) {
            errorMessageContainer.innerText = errorMessage;
            showError(errorMessage);
            showElement(errorMessageContainer);
        };

        var showSuccessMessage = function () {
            hideElement(errorMessageContainer);
            hideElement(formContainer);
            showElement(successMessageContainer);
        };

        var showSuccessAndConfirmationSentMessage = function () {
            hideElement(formContainer);
            showElement(confirmRegistrationMessageContainer);

            var header = confirmRegistrationMessageContainer.querySelector('[data-sf-role="activation-link-header');
            header.innerText = confirmRegistrationMessageContainer.querySelector('[name="PleaseCheckYourEmailHeader"]').value;

            var activationLinkLabel = confirmRegistrationMessageContainer.querySelector("input[name='PleaseCheckYourEmailMessage']").value;

            var activationLinkMessageContainer = confirmRegistrationMessageContainer.querySelector('[data-sf-role="activation-link-message-container"]');

            var formData = new FormData(form);
            var email = formData.get("Email");

            activationLinkMessageContainer.innerText = activationLinkLabel + " " + email;

            var sendAgainBtn = confirmRegistrationMessageContainer.querySelector('[data-sf-role="sendAgainLink"]');
            sendAgainBtn.replaceWith(sendAgainBtn.cloneNode(true));
            sendAgainBtn = confirmRegistrationMessageContainer.querySelector('[data-sf-role="sendAgainLink"]');

            sendAgainBtn.innerText = confirmRegistrationMessageContainer.querySelector('input[name="SendAgainLink"]').value;

            var resendUrl = confirmRegistrationMessageContainer.querySelector("input[name='ResendConfirmationEmailUrl']").value;

            sendAgainBtn.addEventListener('click', function (event) {
                event.preventDefault();
                submitFormHandler(form, resendUrl, postResendAction);
            });
        };

        var showErrorMessage = function () {
            hideElement(formContainer);
            showElement(confirmRegistrationMessageContainer);

            var header = confirmRegistrationMessageContainer.querySelector('[data-sf-role="activation-link-header');
            header.innerText = confirmRegistrationMessageContainer.querySelector('[name="ActivateAccountTitle"]').value;

            var activationLinkMessageContainer = confirmRegistrationMessageContainer.querySelector('[data-sf-role="activation-link-message-container"]');
            var email = confirmRegistrationMessageContainer.querySelector('input[name="ExistingEmail"]').value;
            var activationLabel = confirmRegistrationMessageContainer.querySelector('input[name="ActivateAccountMessage"]').value.replace("{0}", email);
            activationLinkMessageContainer.innerText = activationLabel;
            form.querySelector("input[name='Email']").value = email;
            var userExists = Boolean(widgetContainer.querySelector("input[name='ExistingEmail']").value);

            var sendAgainBtn = confirmRegistrationMessageContainer.querySelector('[data-sf-role="sendAgainLink"]');

            if (userExists) {
                sendAgainBtn.innerText = confirmRegistrationMessageContainer.querySelector('input[name="ActivateAccountLink"]').value;

                var resendUrl = confirmRegistrationMessageContainer.querySelector("input[name='ResendConfirmationEmailUrl']").value;
                sendAgainBtn.addEventListener('click', function (event) {
                    event.preventDefault();
                    submitFormHandler(form, resendUrl, showSuccessAndConfirmationSentMessage);
                });
            } else {
                hideElement(sendAgainBtn);
            }
        };

        var submitFormHandler = function (form, url, onSuccess, onError) {
            url = url || form.attributes['action'].value;
            var model = serializeForm(form);
            window.fetch(url, { method: 'POST', body: JSON.stringify(model), headers: { 'Content-Type': 'application/json' } })
                .then(response => response.json())
                .then(res => {                    
                    if (res.statusCode === 0 || (res.statusCode >= 200 && res.statusCode < 400)) {
                        if (res.data.isUpdated) {
                            Swal.fire({
                                title: "Profile Updated Successfully!!!",
                                text: "You clicked the button!",
                                icon: "success",
                                button: "Aww yiss!",
                            });
                        } else {
                            Swal.fire({
                                title: "Something went wrong",
                                text: res.frontendErrorMessage,
                                icon: "warning",
                                confirmButtonText: "Retry"
                            });
                        }
                    }
                    else {
                        if (onError) {
                            onError(message, status);
                        }
                    }
                });
        };

        var PhoneBlurHandler = function (form, url, onSuccess, OnError, Element) {
            if (phoneinput.value !== '') {
                url = url || form.attributes['action'].value;
                var model = phoneinput.value;
                window.fetch(url, { method: 'POST', body: model, headers: { 'Content-Type': 'application/json' } })
                    .then((response) => {
                        var status = response.status;
                        response.json().then((res) => {
                            console.log(res);
                            var message = res;
                            if ((message.statusCode === 0 && message.isException == false) || (message.statusCode >= 200 && message.statusCode < 400)) {
                                btnRegister.disabled = false;
                                if (onSuccess) {
                                    btnRegister.disabled = false;
                                }
                            }
                            else {
                                showElement(Element);
                                invalidateElement(Element);
                                btnRegister.disabled = true;
                                return false;
                            }
                        });
                    });
            }
            else {
                showElement(Element);
                btnRegister.disabled = true;
            }

        };

        var EmailBlurHandler = function (form, url, onSuccess, OnError, Element) {
            if (emailinput.value !== '') {
                url = url || form.attributes['action'].value;
                var model = emailinput.value;
                window.fetch(url, { method: 'POST', body: model, headers: { 'Content-Type': 'application/json' } })
                    .then((response) => {
                        var status = response.status;
                        response.json().then((res) => {
                            console.log(res);
                            var message = res;
                            if (message.statusCode === 0 || (message.statusCode >= 200 && message.statusCode < 400)) {
                                btnRegister.disabled = false;
                                if (onSuccess) {
                                    btnRegister.disabled = false;
                                }
                            }
                            else {
                                //if (onError) {
                                //onError(message, status);
                                showElement(Element);
                                invalidateElement(Element);
                                btnRegister.disabled = true;
                                //}
                            }
                        });
                    });
            }
            else {
                showElement(Element);
                btnRegister.disabled = true;
            }

        };

        var postResendAction = function () {
            var header = confirmRegistrationMessageContainer.querySelector('[data-sf-role="activation-link-header');
            header.innerText = confirmRegistrationMessageContainer.querySelector('[name="PleaseCheckYourEmailHeader"]').value;

            var activationLinkMessageContainer = confirmRegistrationMessageContainer.querySelector('[data-sf-role="activation-link-message-container"]');
            var sendAgainLabel = confirmRegistrationMessageContainer.querySelector("input[name='PleaseCheckYourEmailAnotherMessage']").value;
            var formData = new FormData(form);
            var email = formData.get("Email");
            activationLinkMessageContainer.innerText = sendAgainLabel.replace("{0}", email);
        };

        var validateForm = function (form, Element) {
            var isValid = true;
            resetValidationErrors(form);
            hideElement(errorMessageContainer);

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

            var emailInput = form.querySelector("input[name='Email']");
            if (!isValidEmail(emailInput.value)) {
                errorMessageContainer.innerText = formContainer.querySelector("input[name='ValidationInvalidEmailMessage']").value;
                invalidateElement(emailInput);
                showElement(errorMessageContainer);
                return false;
            }

            var passwordFields = form.querySelectorAll("[type='password']");

            if (passwordFields[0].value !== passwordFields[1].value) {
                errorMessageContainer.innerText = formContainer.querySelector("input[name='ValidationMismatchMessage']").value;
                invalidateElement(passwordFields[1]);
                showElement(errorMessageContainer);

                return false;
            }


            if (Element != null && Element.name == 'Phone') {

                var phoneinput = formContainer.querySelector("input[name='Phone']");
                //Phonenumber Validation
                var returnvalue;
                if (phoneinput != '') {
                    returnvalue = PhoneBlurHandler(null, "/api/Validation/CheckNumberRegistered", postRegistrationAction, onRegistrationError, phoneinput);
                    returnvalue = PhoneBlurHandler(null, "/api/Validation/CheckNumberIsValid", postRegistrationAction, onRegistrationError, phoneinput);
                    return returnvalue;
                }
            }
            if (Element != null && Element.name == 'Phone') {
                var emailinput = formContainer.querySelector("input[name='Email']");
                if (emailinput != '') {
                    returnvalue = EmailBlurHandler(null, "/api/Validation/CheckEmailIsRegistered", postRegistrationAction, onRegistrationError, emailinput);
                    return returnvalue;
                }
            }
            return isValid;
        };

        var serializeForm = function (form) {
            var obj = {};
            var formData = new FormData(form);
            for (var key of formData.keys()) {
                obj[key] = formData.get(key);
            }
            return obj;
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
            var invalidElements = parentElement.querySelectorAll(`[${invalidDataAttr}]`);
            invalidElements.forEach(function (element) {
                if (classInvalid) {
                    element.classList.remove(...classInvalid);
                }

                element.removeAttribute(invalidDataAttr);
            });
        };

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

        function showError(err) {
            var errorMessageContainer = document.querySelector('[data-sf-role="error-message-container"]');
            errorMessageContainer.innerText = err;
        }

        var userExists = Boolean(widgetContainer.querySelector("input[name='ExistingEmail']").value);
        var activationFailed = Boolean(widgetContainer.querySelector("input[name='ActivationFailed']").value);

        if (activationFailed || userExists) {
            showErrorMessage();
        }
    });
})();
