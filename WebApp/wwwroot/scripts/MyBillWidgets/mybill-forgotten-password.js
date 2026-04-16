(function () {
    document.addEventListener('DOMContentLoaded', function () {
        var widgetContainer = document.querySelector('[data-sf-role="sf-forgotten-password-container"]');
        var formContainer = widgetContainer.querySelector('[data-sf-role="form-container"]');
        var form = formContainer.querySelector("form");
        var banInput = form.querySelector("input[name='ban']");
        var verificationPageUrlInput = form.querySelector("input[name='verificationPageUrl']");
        var successMessageContainer = widgetContainer.querySelector('[data-sf-role="success-message-container"]');
        var errorMessageContainer = widgetContainer.querySelector('[data-sf-role="error-message-container"]');
        var sentBanLabel = successMessageContainer.querySelector('[data-sf-role="sent-ban-label"]');
        var visibilityClassElement = document.querySelector('[data-sf-visibility-hidden]');
        var visibilityClassHidden = visibilityClassElement.dataset ? visibilityClassElement.dataset.sfVisibilityHidden : null;
        var invalidClassElement = document.querySelector('[data-sf-invalid]');
        var classInvalidValue = invalidClassElement.dataset && isNotEmpty(invalidClassElement.dataset.sfInvalid) ? invalidClassElement.dataset.sfInvalid : null;
        var classInvalid = classInvalidValue ? processCssClass(classInvalidValue) : null;
        var invalidDataAttr = "data-sf-invalid";

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
            var model = serializeForm(form);
            var submitUrl = form.attributes['action'].value;
            
            
            window.fetch(submitUrl, { method: 'POST', body: JSON.stringify(model), headers: { 'Content-Type': 'application/json' } })
                .then(response => {
                    return response.json().then(data => ({ status: response.status, data: data }));
                })
                .then(result => {
                    var res = result.data; // This is ApiResponse<MyBillSubmitRequestResponse>
                    var httpStatus = result.status;
                    
                    // Check for success: HTTP 200 and res.data.data.isSent === true
                    if (httpStatus === 200 && res.data?.data?.isSent === true) {
                        
                        // Check if SweetAlert is available
                        if (typeof Swal !== 'undefined') {
                            Swal.fire({
                                title: "Email Sent Successfully",
                                text: "A password reset link has been sent to your email for BAN: " + banInput.value,
                                icon: "success",
                                confirmButtonText: "OK",
                                allowOutsideClick: false
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.href = "/my-bill-login";
                                }
                            });
                        } else {
                            // Fallback: Show success message container
                            sentBanLabel.innerText = sentBanLabel.innerText.replace("{0}", banInput.value);
                            hideElement(errorMessageContainer);
                            hideElement(formContainer);
                            showElement(successMessageContainer);
                        }
                    }
                    else if (httpStatus === 401 || res.status === 401) {
                        // Unauthorized - BAN not found
                        var errorMsg = res.detail || res.data?.detail || res.frontendErrorMessage || "Unable to find account with this BAN";
                        errorMessageContainer.innerText = errorMsg;
                        hideElement(successMessageContainer);
                        showElement(errorMessageContainer);
                    }
                    else if (httpStatus === 400 || res.statusCode === 400) {
                        // Bad Request
                        var errorMsg = res.title || res.detail || res.frontendErrorMessage || "Invalid request. Please check your BAN.";
                        errorMessageContainer.innerText = errorMsg;
                        hideElement(successMessageContainer);
                        showElement(errorMessageContainer);
                    }
                    else {
                        // Other errors
                        var errorMsg = res.frontendErrorMessage || res.title || res.detail || "An error occurred while processing your request.";
                        errorMessageContainer.innerText = errorMsg;
                        hideElement(successMessageContainer);
                        showElement(errorMessageContainer);
                    }
                })
                .catch(error => {
                    errorMessageContainer.innerText = "An error occurred while processing your request.";
                    showElement(errorMessageContainer);
                });
        });

        var validateForm = function (form) {
            var isValid = true;
            hideElement(errorMessageContainer);
            resetValidationErrors(form);

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

            if (!isValidBAN(banInput.value)) {
                errorMessageContainer.innerText = formContainer.querySelector("input[name='InvalidBanFormatMessage']").value;
                invalidateElement(banInput);
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

        var isValidBAN = function (ban) {
            // Basic validation - BAN should be numeric and have a certain length
            // Adjust this regex based on your BAN format requirements
            return /^\d{8,12}$/.test(ban);
        };

        var currentURL = window.location.href;
        verificationPageUrlInput.value = currentURL;
    });
})();
