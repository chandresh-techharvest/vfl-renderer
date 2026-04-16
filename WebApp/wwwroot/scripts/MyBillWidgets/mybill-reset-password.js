(function () {
    document.addEventListener('DOMContentLoaded', function () {
        var widgetContainer = document.querySelector('[data-sf-role="sf-reset-password-container"]');
        var visibilityClassElement = document.querySelector('[data-sf-visibility-hidden]');
        var visibilityClassHidden = visibilityClassElement.dataset ? visibilityClassElement.dataset.sfVisibilityHidden : null;
        var formContainer = widgetContainer.querySelector('[data-sf-role="form-container"]');
        var form = formContainer.querySelector("form");
        var errorMessageContainer = widgetContainer.querySelector('[data-sf-role="error-message-container"]');
        var successMessageContainer = widgetContainer.querySelector('[data-sf-role="success-message-container"]');
        var invalidClassElement = document.querySelector('[data-sf-invalid]');
        var classInvalidValue = invalidClassElement.dataset && isNotEmpty(invalidClassElement.dataset.sfInvalid) ? invalidClassElement.dataset.sfInvalid : null;
        var classInvalid = classInvalidValue ? processCssClass(classInvalidValue) : null;
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
                return;
            }
            
            var model = serializeForm(form);
            var submitUrl = form.attributes['action'].value;
            
            window.fetch(submitUrl, { method: 'POST', body: JSON.stringify(model), headers: { 'Content-Type': 'application/json' } })
                .then(response => {
                    return response.json().then(data => ({ status: response.status, data: data }));
                })
                .then(result => {
                    var res = result.data;
                    var httpStatus = result.status;
                    
                    // Check for success: statusCode 200 and data.data.isReset === true
                    if ((httpStatus === 200 || res.statusCode === 200) && res.data?.data?.isReset === true) {
                        
                        // Show success message using built-in container
                        hideElement(errorMessageContainer);
                        hideElement(formContainer);
                        showElement(successMessageContainer);
                        
                        // Redirect after 3 seconds
                        setTimeout(function() {
                            redirect("/my-bill-login");
                        }, 3000);
                    }
                    else if (httpStatus === 400 || res.status === 400 || res.statusCode === 400) {
                        // Validation errors
                        var errorMsg = "";
                        if (res.errors && res.errors.Password) {
                            errorMsg = res.errors.Password.join(', ');
                        } else {
                            errorMsg = res.title || res.frontendErrorMessage || "Validation error occurred.";
                        }
                        showError(errorMsg);
                    }
                    else {
                        var errorMsg = res.title || res.frontendErrorMessage || res.data?.message || "An error occurred while resetting your password.";
                        showError(errorMsg);
                    }
                })
                .catch(error => {
                    showError("An error occurred while resetting your password.");
                });
        });

        var validateForm = function (form) {
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
                errorMessageContainer.innerText = formContainer.querySelector("input[name='AllFieldsAreRequiredErrorMessage']").value;
                showElement(errorMessageContainer);

                return isValid;
            }

            var passwordFields = form.querySelectorAll("[type='password']");

            if (passwordFields[0].value !== passwordFields[1].value) {
                errorMessageContainer.innerText = formContainer.querySelector("input[name='PasswordsMismatchErrorMessage']").value;
                invalidateElement(passwordFields[1]);
                showElement(errorMessageContainer);

                return false;
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
                // Remove d-none class if it exists
                element.classList.remove('d-none');
                
                if (visibilityClassHidden) {
                    element.classList.remove(visibilityClassHidden);
                } else {
                    element.style.display = "";
                }
            }
        };

        var hideElement = function (element) {
            if (element) {
                // Add d-none class
                element.classList.add('d-none');
                
                if (visibilityClassHidden) {
                    element.classList.add(visibilityClassHidden);
                } else {
                    element.style.display = "none";
                }
            }
        };

        var redirect = function (redirectUrl) {
            window.location = redirectUrl;
        };

        function showError(err) {
            errorMessageContainer.innerText = err;
            showElement(errorMessageContainer);
        }
    });
})();
