(function () {
    document.addEventListener('DOMContentLoaded', function () {
        var widgetContainer = document.querySelector('[data-sf-role="sf-forgotten-password-container"]');
        var formContainer = widgetContainer.querySelector('[data-sf-role="form-container"]');
        var form = formContainer.querySelector("form");
        var emailInput = form.querySelector("input[name='Email']");
        var VerificationPageUrl = form.querySelector("input[name='VerificationPageUrl']");
        var successMessageContainer = widgetContainer.querySelector('[data-sf-role="success-message-container"]');
        var errorMessageContainer = widgetContainer.querySelector('[data-sf-role="error-message-container"]');
        var sentEmailLabel = successMessageContainer.querySelector('[data-sf-role="sent-email-label"]');
        var visibilityClassElement = document.querySelector('[data-sf-visibility-hidden]');
        var visibilityClassHidden = visibilityClassElement.dataset ? visibilityClassElement.dataset.sfVisibilityHidden : null;
        var invalidClassElement = document.querySelector('[data-sf-invalid]');
        var classInvalidValue = invalidClassElement.dataset && isNotEmpty(invalidClassElement.dataset.sfInvalid) ? invalidClassElement.dataset.sfInvalid : null;
        var classInvalid = classInvalidValue ? processCssClass(classInvalidValue) : null;
        var invalidDataAttr = "data-sf-invalid";
        const EmailalreadyRegisteredMessage = "The account with the email entered does not exist. Please register.";
        const EmailalreadyRegisteredNotConfirmedMessage = "This email is already registered, but not confirmed, Please use button below to resend confirmation Email.";
        var ResendLink = document.getElementById("ResendLink");
        var resendConfirmEmail = document.getElementById("ResendLink");


        function processCssClass(str) {
            var classList = str.split(" ");
            return classList;
        }
        function isNotEmpty(attr) {
            return (attr && attr !== "");
        }

        form.addEventListener('submit', function (event) {
            event.preventDefault();
            if (!validateForm(form)) {
                return;
            }

            checkEmailAndSubmit();
        
        });


        emailInput.addEventListener('blur', function () {

            if (!emailInput.value) {
                hideElement(errorMessageContainer);
                return;
            }

            if (!isValidEmail(emailInput.value)) {
                showError("Please enter a valid email address.");
                invalidateElement(emailInput);
            } else {
                hideElement(errorMessageContainer);
                resetValidationErrors(form);
            }
        });
       


        function checkEmailAndSubmit() {
            fetch("/api/Validation/CheckEmailIsRegistered", {
                method: 'POST',
                body: JSON.stringify(emailInput.value),
                headers: { 'Content-Type': 'application/json' }
            })
                .then(response => response.json())
                .then(res => {
                    if (res.statusCode >= 200 && res.statusCode < 400 && res.data) {
                        resetValidationErrors(form);
                        if (!res.data.isUserEmailExists) {
                            showError(EmailalreadyRegisteredMessage);
                            invalidateElement(emailInput);
                            return;
                        }
                        if (!res.data.isEmailConfirmed) {
                            showEmailAlreadyExistsError(EmailalreadyRegisteredNotConfirmedMessage);
                            invalidateElement(emailInput);
                            return;
                        }
                        submitFormData();
                    }
                    else {
                        showError("Something went wrong while validating email.");
                    }
                })
                .catch(() => {
                    showError("Unable to validate email.");
                });
        }





        var redirect = function (redirectUrl) {
            window.location = redirectUrl;
        };


        resendConfirmEmail.addEventListener('click', function (event) {
            var url = "/api/RegistrationWidget/ResendConfirmationEmail";
            const ResendVerificationModel = {};
            ResendVerificationModel.email = emailInput.value;
            ResendVerificationModel.verificationPageUrl = VerificationPageUrl.value;
            window.fetch(url, { method: 'POST', body: JSON.stringify(ResendVerificationModel), headers: { 'Content-Type': 'application/json' } })
                .then(response => response.json())
                .then(res => {
                    console.log(res);
                    if (res.statusCode >= 200 && res.statusCode < 400) {
                        if (res.data != null && (!res.data.isNumberRegistered || res.data.isNumberValid)) {
                            Swal.fire({
                                title: "Confirmation Email sent Successfully!!",
                                text: "Congratulations!",
                                icon: "success",
                                confirmButtonText: "OK"
                            }).then(() => {
                                window.location.href = "/";
                            });
                        }
                        else {
                            showError(getErrorMessage(res));
                       
                        }
                    }
                    else {
                        showError(getErrorMessage(res));
  
                    }
                })
                .catch(error => {
                    showError(error.toString());
       
                    console.error('Error fetching data:', error);
                });
        });





        function submitFormData() {
            var model = serializeForm(form);
            var submitUrl = form.attributes['action'].value;
            window.fetch(submitUrl, {
                method: 'POST',
                body: JSON.stringify(model),
                headers: { 'Content-Type': 'application/json' }
            })
                .then(response => response.json())
                .then(res => {
                    if ((res.status == 0 || res.status >= 200) && res.status < 400) {
                        if (res.isException) {
                            errorMessageContainer.innerText = res.developerErrorMessage;
                            hideElement(formContainer);
                            showElement(errorMessageContainer);
                        } else {
                            sentEmailLabel.innerText = sentEmailLabel.innerText.replace("{0}", emailInput.value);
                            hideElement(formContainer);
                            showElement(successMessageContainer);
                        }
                    } else {
                        errorMessageContainer.innerText = res.frontendErrorMessage;
                        hideElement(formContainer);
                        showElement(errorMessageContainer);
                    }
                });
        }




        var validateForm = function (form) {
            var isValid = true;
            hideElement(errorMessageContainer);
            resetValidationErrors(form);

            if (!emailInput.value || emailInput.value.trim() === "") {
                errorMessageContainer.innerText = "Email is required.";
                invalidateElement(emailInput);
                showElement(errorMessageContainer);
                return false;
            }

            if (!isValidEmail(emailInput.value)) {
                errorMessageContainer.innerText = formContainer.querySelector("input[name='InvalidEmailFormatMessage']").value;
                invalidateElement(emailInput);
                showElement(errorMessageContainer);
                return false;
            }

            var requiredInputs = form.querySelectorAll("input[data-sf-role='required']");

            requiredInputs.forEach(function (input) {
                if (!input.value) {
                    invalidateElement(input);
                    isValid = false;
                }
            });

            if (!isValid) {
                errorMessageContainer.innerText = formContainer.querySelector("input[name='FieldIsRequiredMessage']").value;
                showElement(errorMessageContainer);

                return false;
            }



            return true;
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
                element.classList.remove("d-none");
                if (visibilityClassHidden) {
                    element.classList.remove(visibilityClassHidden);
                } else {
                    element.style.display = "";
                }
            }
        };

        var hideElement = function (element) {
            if (element) {
                element.classList.add("d-none");

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
        function showEmailAlreadyExistsError(err) {
            var errorMessageContainer = document.querySelector('[data-sf-role="error-message-container"]');
            if (!errorMessageContainer) return;
            errorMessageContainer.innerText = "";
            errorMessageContainer.innerText = err + "\n";
            ResendLink.classList.remove("d-none");
            errorMessageContainer.classList.remove("d-none");
            errorMessageContainer.scrollIntoView({ behavior: "smooth", block: "center" });
        }

        function showError(err) {
            var errorMessageContainer = document.querySelector('[data-sf-role="error-message-container"]');
            if (!errorMessageContainer) return;
            errorMessageContainer.innerText = "";
            errorMessageContainer.innerText = err + "\n";
            errorMessageContainer.classList.remove("d-none");
            errorMessageContainer.scrollIntoView({ behavior: "smooth", block: "center" });
        }

        var currentURL = window.location.href;
        // console.log(currentURL);
        VerificationPageUrl.value = currentURL;
        //$(".VerificationPageUrl").val(currentURL);
    });
})();
