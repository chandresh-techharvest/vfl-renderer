(function () {
    document.addEventListener('DOMContentLoaded', function () {
        var widgetContainer = document.querySelector('[data-sf-role="sf-registration-container"]');
        var formContainer = widgetContainer.querySelector('[data-sf-role="form-container"]');
        var form = formContainer.querySelector("form");
        var phoneinput = formContainer.querySelector("input[name='Phone']");
        var emailinput = formContainer.querySelector("input[name='Email']");
        var ModalVerificationUrl = document.querySelector("input[name='ModalVerificationUrl']");
        var inputs = formContainer.querySelectorAll("input");
        var btnRegister = formContainer.querySelector("input[name='btnRegister']"); 
        var errorMessageContainer = widgetContainer.querySelector('[data-sf-role="error-message-container"]');
        var errorMessageContainerResend = widgetContainer.querySelector('[data-sf-role="error-message-container-resend"]');
        var successRegistrationMessageContainer = widgetContainer.querySelector('[data-sf-role="success-registration-message-container"]');
        var confirmRegistrationMessageContainer = widgetContainer.querySelector('[data-sf-role="confirm-registration-message-container"]');
        var visibilityClassElement = document.querySelector('[data-sf-visibility-hidden]');
        var visibilityClassHidden = visibilityClassElement.dataset ? visibilityClassElement.dataset.sfVisibilityHidden : null;
        var invalidClassElement = document.querySelector('[data-sf-invalid]');
        var classInvalidValue = invalidClassElement.dataset && isNotEmpty(invalidClassElement.dataset.sfInvalid) ? invalidClassElement.dataset.sfInvalid : null;
        var classInvalid = classInvalidValue ? processCssClass(classInvalidValue) : null;
        var invalidDataAttr = "data-sf-invalid";
        var btnModalClose = document.getElementById("btnModalClose");
        var resendVerification = document.getElementById("resendVerification");
        var resendConfirmEmail = document.getElementById("resendConfirmEmail");
        var termCondition = document.getElementById("termCondition");
        const ResendLink = document.getElementById("ResendLink");

        const EmailalreadyRegisteredMessage = "* This email is already registered.";
        const EmailalreadyRegisteredNotConfirmedMessage = "* Email entered is already registered, but not confirmed. Please click on the resend button below to resend activation email";
        const PhonealreadyRegisteredMessage = "* This phone number is already registered.";
        const PhoneInvalidMessage = "* The number entered is not a valid Vodafone number.";

        // Custom validation method for strong password
        $.validator.addMethod("strongPassword", function (value, element) {
            return this.optional(element) || /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+]).{8,}$/.test(value);
        }, "Password must meet complexity requirements.");

         //Custom validation method for phone number format
        $.validator.addMethod("phoneFormat", function (value, element) {
            var test = this.optional(element) || value === "" || /^\d{7}$/.test(value);
            return this.optional(element) || value === "" || /^\d{7}$/.test(value);
        }, PhoneInvalidMessage);

       
        // Initialize jQuery Validation
        var validator = $(form).validate({
            onfocusout: function (element) {
                // Don't validate checkboxes on focusout
                if ($(element).attr('type') !== 'checkbox') {
                    this.element(element);
                }
            },
            onclick: function (element) {
                // Validate checkboxes on click
                if ($(element).attr('type') === 'checkbox') {
                    this.element(element);
                }
            },
            onkeyup: false,
            ignoreTitle: true,
            rules: {
                FirstName: {
                    required: true
                },
                LastName: {
                    required: true
                },
                
                Email: {
                    required: true,
                    email: true,
                    remote: {
                        url: "/api/Validation/CheckEmailIsRegistered",
                        type: 'POST',
                        contentType: "application/json; charset=utf-8", // Inform the server we're sending JSON
                        dataType: "json",
                        data: function () {
                            return JSON.stringify($('input[name="Email"]').val());
                        },
                        beforeSend: function (xhr, settings) {
                            // Override the data being sent to be the raw JSON string
                            settings.data = JSON.stringify($('input[name="Email"]').val());
                        },
                        dataFilter: function (response) {
                            console.log(response);
                            var res = JSON.parse(response);
                            if (res.data?.isUserEmailExists === true) {
                                if (res.data?.isEmailConfirmed === false) {
                                    // Return JSON-encoded "true" for valid
                                    invalidateElement($('input[name="Email"]'));
                                    if (ResendLink) ResendLink.classList.remove("d-none");
                                    return JSON.stringify(EmailalreadyRegisteredNotConfirmedMessage);     
                                }
                                else {
                                    // Return JSON-encoded error message
                                    invalidateElement($('input[name="Email"]'));
                                    return JSON.stringify(EmailalreadyRegisteredMessage);
                                }
                            }
                            else {
                                // Return JSON-encoded error message
                                return '"true"';
                                
                            }
                        }

                    }
                },
                Phone: {
                    phoneFormat: true,
                    remote: {
                        url: "/api/Validation/CheckNumber",
                        type: 'POST',
                        contentType: "application/json", // Inform the server we're sending JSON
                        dataType: "json",
                        data: function () {
                            return JSON.stringify($('#Phone').val());
                        },
                        beforeSend: function (xhr, settings) {
                            // Override the data being sent to be the raw JSON string
                            console.log(JSON.stringify($('#Phone').val()));
                            settings.data = JSON.stringify($('#Phone').val());
                        },
                        dataFilter: function (response) {
                            console.log(response);
                            var res = JSON.parse(response);
                            if (res.data?.isNumberValid === true) {
                                if (res.data?.isNumberRegistered === false) {
                                    // Return JSON-encoded "true" for valid
                                    return '"true"';
                                }
                                else {
                                    // Return JSON-encoded error message
                                    invalidateElement($('#Phone'));
                                    return JSON.stringify(PhonealreadyRegisteredMessage);
                                }
                            }
                            else {
                                // Return JSON-encoded error message
                                invalidateElement($('#Phone'));
                                return JSON.stringify(PhoneInvalidMessage);
                            }
                        }

                    }
                },
                Password: {
                    required: true,
                    strongPassword: true
                },
                RepeatPassword: {
                    required: true,
                    equalTo: "input[name='Password']"
                },
                termCondition: {
                    required: true
                }
            },
            messages: {
                FirstName: {
                    required: "* First Name is required"
                },
                LastName: {
                    required: "* Last Name is required"
                },
                Email: {
                    required: "* Email is required",
                    email: "* Please enter a valid email address"
                },
                Phone: {
                    phoneFormat: PhoneInvalidMessage,
                    remote: PhoneInvalidMessage
                },
                Password: {
                    required: "* Password is required",
                    strongPassword: "* Password must meet complexity requirements."
                },
                RepeatPassword: {
                    required: "* Please repeat your password",
                    equalTo: "* Passwords do not match"
                },
                termCondition: {
                    required: "* Please accept the terms and conditions."
                }
            },
            errorElement: "p",
            errorPlacement: function (error, element) {
                var container = $("div[data-sf-role='error-message-container']");
                if (!container.length) return;

                // Remove only the existing error for this specific element
                var fieldName = $(element).attr('name');
                container.find('p[data-field="' + fieldName + '"]').remove();

                // Add field identifier to the error element
                error.attr('data-field', fieldName);
                error.appendTo(container);
                invalidateElement(element);
                container.parent().removeClass("d-none");
            },
            success: function (label, element) {
                // Remove error for this field when it becomes valid
                var fieldName = $(element).attr('name');
                var container = $("div[data-sf-role='error-message-container']");
                container.find('p[data-field="' + fieldName + '"]').remove();
                validateElement(element);
                // Hide container if no more errors
                if (container.find('p').length === 0) {
                    container.addClass("d-none");
                }
            },
            submitHandler: function (formElement) {
                
                // Proceed with custom AJAX submission
                setAntiforgeryTokens().then(function () {
                    submitFormHandler(formElement, null, postRegistrationAction, onRegistrationError);
                }, function (err) {
                    showError("Antiforgery token retrieval failed");
                });

                return false; // Prevent default form submission
            },
            invalidHandler: function (event, validator) {
                // Show error container when validation fails
                var container = $("div[data-sf-role='error-message-container']");
                container.removeClass("d-none");
            }
        });

        // Ensure checkbox validation triggers on change
        $('#termCondition').on('change', function () {
            $(this).valid();
        });

        function clearErrors() {
            var container = $("div[data-sf-role='error-message-container']");
            container.html("");
            container.addClass("d-none");
            validator.resetForm();
            // Also clear any custom validation classes
            $(form).find('.error').removeClass('error');
            $(form).find('.valid').removeClass('valid');
            // Clear custom invalid attributes
            resetValidationErrors(form);
        }

        function getErrorMessage(res) {
            if (res.frontendErrorMessage) return res.frontendErrorMessage;
            if (res.developerErrorMessage) return res.developerErrorMessage;
            if (res.detail) return res.detail;

            if (res.errors) {
                const allErrors = Object.values(res.errors)
                    .flat()
                    .map(err => ` ${err}`)
                    .join("\n");
                if (allErrors) return allErrors;
            }
            if (res.title) return res.title;

            return "Registration failed.";
        }

        function getFieldLabel(input) {
            const wrapper = input.closest('.col-xl-6, .col-xl-12, .form-check');

            if (wrapper) {
                const label = wrapper.querySelector('label');
                if (label) {
                    return label.innerText.replace('*', '').trim();
                }
            }

            if (input.name) {
                return input.name.replace(/([A-Z])/g, ' $1').trim();
            }

            return 'This field';
        }

        function isNotEmpty(attr) {
            return (attr && attr !== "");
        }

        function processCssClass(str) {
            var classList = str.split(" ");
            return classList;
        }

        // Remove the manual form submit handler - let jQuery Validate handle it
        form.addEventListener('submit', function (event) {
            event.preventDefault();
        });

        btnModalClose.addEventListener('click', function (event) {
            event.preventDefault();

            var modalElement = document.getElementById('emailverification');
            var emailVerificationModal = new bootstrap.Modal(modalElement);
            emailVerificationModal.hide();

            form.reset();
            clearErrors();
        });

        resendVerification.addEventListener('click', function (event) {
            var url = "/api/RegistrationWidget/ResendConfirmationEmail";
            const ResendVerificationModel = {};
            ResendVerificationModel.email = emailinput.value;
            ResendVerificationModel.verificationPageUrl = ModalVerificationUrl.value;
            window.fetch(url, { method: 'POST', body: JSON.stringify(ResendVerificationModel), headers: { 'Content-Type': 'application/json' } })
                .then(response => response.json())
                .then(res => {
                    console.log(res);
                    if (res.statusCode >= 200 && res.statusCode < 400) {
                        if (res.data != null && (!res.data.isNumberRegistered || res.data.isNumberValid)) {
                            btnRegister.disabled = false;
                        }
                        else {
                            showError(getErrorMessage(res));
                            btnRegister.disabled = true;
                        }
                    }
                    else {
                        showError(getErrorMessage(res));
                        btnRegister.disabled = true;
                    }
                })
                .catch(error => {
                    showError(error.toString());
                    btnRegister.disabled = true;
                    console.error('Error fetching data:', error);
                });
        });

        resendConfirmEmail.addEventListener('click', function (event) {
            var url = "/api/RegistrationWidget/ResendConfirmationEmail";
            const ResendVerificationModel = {};
            ResendVerificationModel.email = emailinput.value;
            ResendVerificationModel.verificationPageUrl = ModalVerificationUrl.value;
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
                                button: "OK",
                            });

                            redirect("/");
                        }
                        else {
                            showError(getErrorMessage(res));
                            btnRegister.disabled = true;
                        }
                    }
                    else {
                        showError(getErrorMessage(res));
                        btnRegister.disabled = true;
                    }
                })
                .catch(error => {
                    showError(error.toString());
                    btnRegister.disabled = true;
                    console.error('Error fetching data:', error);
                });
        });

        var postRegistrationAction = function () {
            var action = formContainer.querySelector("input[name='PostRegistrationAction']").value;
            var activationMethod = formContainer.querySelector("input[name='ActivationMethod']").value;

            if (action === 'ViewMessage') {
                if (activationMethod == "AfterConfirmation") {
                    showSuccessAndConfirmationSentMessage();
                } else {
                    showSuccessMessage();
                }
            } else if (action === 'RedirectToPage') {
                var redirectUrl = formContainer.querySelector("input[name='RedirectUrl']").value;
                var modalElement = document.getElementById('emailverification');
                var emailVerificationModal = new bootstrap.Modal(modalElement);
                emailVerificationModal.show();
            }
        };

        var onRegistrationError = function (errorMessage, status) {
            errorMessageContainer.innerText = errorMessage;
            showError(errorMessage);
            showElement(errorMessageContainer);
        };

        var showSuccessMessage = function () {
            hideElement(errorMessageContainer);
            hideElement(formContainer);
            showElement(successRegistrationMessageContainer);
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
                    console.log(res);
                    if (res.statusCode >= 200 && res.statusCode < 400) {
                        if (res.data != null && (!res.data.isNumberRegistered || res.data.isNumberValid)) {
                            btnRegister.disabled = false;
                            if (onSuccess) {
                                onSuccess();
                                btnRegister.disabled = false;
                            }
                        }
                        else {
                            if (res.data?.isUserEmailExists === true) {
                                showError(EmailalreadyRegisteredMessage);
                                invalidateElement(emailinput);
                            }
                            else if (res.data?.isNumberRegistered === true) {
                                showError(PhonealreadyRegisteredMessage);
                                invalidateElement(phoneinput);
                            }
                            else {
                                showError(getErrorMessage(res));
                            }

                            btnRegister.disabled = true;
                        }
                    }
                    else {
                        showError(getErrorMessage(res));
                        btnRegister.disabled = true;
                    }
                })
                .catch(error => {
                    showError(error.toString());
                    btnRegister.disabled = true;
                    console.error('Error fetching data:', error);
                });
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
            var $element = $(element);

            if (!$element.length) return;

            if (Array.isArray(classInvalid) && classInvalid.length > 0) {
                $element.addClass(classInvalid.join(' '));
            }

            $element.attr(invalidDataAttr, "");
        };

        var validateElement = function (element) {
            var $element = $(element);

            if (!$element.length) return;

            if (Array.isArray(classInvalid) && classInvalid.length > 0) {
                $element.removeClass(classInvalid.join(' '));
            }

            $element.removeAttr(invalidDataAttr);
        };

        var resetValidationErrors = function (parentElement) {
            var $parent = $(parentElement);
            var $invalidElements = $parent.find('[' + invalidDataAttr + ']');

            $invalidElements.each(function () {
                if (classInvalid) {
                    $(this).removeClass(classInvalid.join(' '));
                }
                $(this).removeAttr(invalidDataAttr);
            });

            var $errorContainer = $("div[data-sf-role='error-message-container']");
            if ($errorContainer.length) {
                $errorContainer.addClass("d-none");
                if (ResendLink) {
                    $(ResendLink).addClass("d-none");
                }
            }
        };

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
            if (!errorMessageContainer) return;
            errorMessageContainer.innerText = "";
            errorMessageContainer.innerText = err + "\n";
            errorMessageContainer.classList.remove("d-none");
            errorMessageContainer.scrollIntoView({ behavior: "smooth", block: "center" });
        }

        function showEmailAlreadyExistsError(err) {
            var errorMessageContainer = document.querySelector('[data-sf-role="error-message-container"]');
            if (!errorMessageContainer) return;
            errorMessageContainer.innerText = "";
            errorMessageContainer.innerText = err + "\n";
            if (ResendLink) ResendLink.classList.remove("d-none");
            errorMessageContainer.classList.remove("d-none");
            errorMessageContainer.scrollIntoView({ behavior: "smooth", block: "center" });
        }

        var postResendAction = function () {
            var header = confirmRegistrationMessageContainer.querySelector('[data-sf-role="activation-link-header');
            header.innerText = confirmRegistrationMessageContainer.querySelector('[name="PleaseCheckYourEmailHeader"]').value;

            var activationLinkMessageContainer = confirmRegistrationMessageContainer.querySelector('[data-sf-role="activation-link-message-container"]');
            var sendAgainLabel = confirmRegistrationMessageContainer.querySelector("input[name='PleaseCheckYourEmailAnotherMessage']").value;
            var formData = new FormData(form);
            var email = formData.get("Email");
            activationLinkMessageContainer.innerText = sendAgainLabel.replace("{0}", email);
        };

        var userExists = Boolean(widgetContainer.querySelector("input[name='ExistingEmail']").value);
        var activationFailed = Boolean(widgetContainer.querySelector("input[name='ActivationFailed']").value);

        if (activationFailed || userExists) {
            showErrorMessage();
        }
    });
        // Remove the manual form submit handler - let jQuery Validate handle it
        
})();