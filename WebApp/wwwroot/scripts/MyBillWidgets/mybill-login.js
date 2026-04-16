(function () {
    const log = Function.prototype;
    const error = Function.prototype;

    document.addEventListener('DOMContentLoaded', function () {
        log('MyBill Login: Initializing...');
        
        var widgetContainer = document.querySelector('[data-sf-role="sf-login-form-container"]');
        if (!widgetContainer) {
            error('MyBill Login: Widget container not found');
            return;
        }
        
        var formContainer = widgetContainer.querySelector('[data-sf-role="form-container"]');
        if (!formContainer) {
            error('MyBill Login: Form container not found');
            return;
        }
        
        var form = formContainer.querySelector("form");
        if (!form) {
            error('MyBill Login: Form not found');
            return;
        }
        
        var redirectUrl = form.querySelector('[name="RedirectUrl"]');
        var errorContainer = widgetContainer.querySelector('[data-sf-role="error-container"]');
        var errorMessageContainer = widgetContainer.querySelector('[data-sf-role="error-message-container"]');
        var ModalVerificationUrl = document.querySelector("input[name='ModalVerificationUrl']");
        var username = document.querySelector("input[name='username']");
        var resendVerification = document.getElementById("resendVerification");
        var successMessageContainer = widgetContainer.querySelector('[data-sf-role="success-message-container"]');
        var resendContainer = widgetContainer.querySelector('[data-sf-role="resend-container"]');
        var loader = document.getElementById('loader');
        var visibilityClassElement = document.querySelector('[data-sf-visibility-hidden]');
        var visibilityClassHidden = visibilityClassElement?.dataset?.sfVisibilityHidden || null;
        var invalidClassElement = document.querySelector('[data-sf-invalid]');
        var classInvalidValue = invalidClassElement?.dataset?.sfInvalid || null;
        var classInvalid = classInvalidValue ? processCssClass(classInvalidValue) : null;
        var invalidDataAttr = "data-sf-invalid";

        function processCssClass(str) {
            return str.split(" ");
        }

        var serializeForm = function (form) {
            var obj = {};
            var formData = new FormData(form);
            for (var key of formData.keys()) {
                obj[key] = formData.get(key);
            }
            return obj;
        };

        var redirect = function (url) {
            log('MyBill Login: Redirecting to:', url);
            window.location.href = url;
        };

        function showLoader() {
            if (loader) {
                loader.classList.remove('d-none');
                loader.classList.remove('hidden');
            }
        }

        function hideLoader() {
            if (loader) {
                loader.classList.add('d-none');
            }
        }

        var showElement = function (element) {
            if (element) {
                element.classList.remove('d-none');
                if (visibilityClassHidden) {
                    element.classList.remove(visibilityClassHidden);
                }
                element.style.display = "";
            }
        };

        var hideElement = function (element) {
            if (element) {
                element.classList.add('d-none');
                if (visibilityClassHidden) {
                    element.classList.add(visibilityClassHidden);
                }
            }
        };

        function resetClientErrorMessage() {
            if (errorMessageContainer) {
                errorMessageContainer.innerText = "";
                hideElement(errorMessageContainer);
            }
            hideElement(errorContainer);
        }

        var invalidateElement = function (element) {
            if (element) {
                if (classInvalid) {
                    element.classList.add(...classInvalid);
                }
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
                var validationMsg = formContainer.querySelector("input[name='ValidationRequiredMessage']");
                if (errorMessageContainer && validationMsg) {
                    errorMessageContainer.innerText = validationMsg.value || "Please fill in all required fields.";
                    showElement(errorMessageContainer);
                }
                showElement(errorContainer);
                return false;
            }

            // Validate Business Account Number (BAN) - must be numeric only
            var usernameInput = form.querySelector("input[name='username']");
            var usernameValue = usernameInput?.value?.trim() || '';

            // BAN should only contain digits (8-12 digits)
            if (!/^\d{8,12}$/.test(usernameValue)) {
                invalidateElement(usernameInput);
                
                if (!/^\d+$/.test(usernameValue)) {
                    errorMessageContainer.innerText = "BAN Number must contain only digits";
                } else if (usernameValue.length < 8) {
                    errorMessageContainer.innerText = "BAN Number must be at least 8 digits";
                } else if (usernameValue.length > 12) {
                    errorMessageContainer.innerText = "BAN Number must not exceed 12 digits";
                } else {
                    errorMessageContainer.innerText = "Invalid BAN Number format";
                }
                
                showElement(errorMessageContainer);
                showElement(errorContainer);
                return false;
            }

            return true;
        };

        form.addEventListener('submit', function (event) {
            event.preventDefault();
            log('MyBill Login: Form submitted');
            
            showLoader();
            
            setAntiforgeryTokens().then(function() {
                if (!validateForm(form)) {
                    hideLoader();
                    return;
                }
                
                var url = form.attributes['action'].value;
                log('MyBill Login: Posting to:', url);
                
                var model = serializeForm(form);
                
                fetch(url, { 
                    method: 'POST', 
                    body: JSON.stringify(model), 
                    headers: { 'Content-Type': 'application/json' },
                    credentials: 'include'
                })
                .then(function(response) {
                    log('MyBill Login: Response status:', response.status);
                    return response.json();
                })
                .then(function(res) {
                    log('MyBill Login: Response received');
                    var status = res.statusCode;
                    
                    hideLoader();
                    
                    if (status === 0 || (status >= 200 && status < 400)) {
                        log('MyBill Login: Login successful!');
                        
                        // Check for returnUrl in query string
                        var urlParams = new URLSearchParams(window.location.search);
                        var returnUrlParam = urlParams.get('returnUrl');
                        
                        // Add cache-busting timestamp to force fresh data fetch
                        var cacheBuster = '_t=' + Date.now();
                        
                        // Use a small delay to ensure the cookie is set before redirect
                        setTimeout(function() {
                            var targetUrl;
                            if (returnUrlParam && returnUrlParam !== '/' && returnUrlParam !== '/my-bill-login') {
                                targetUrl = decodeURIComponent(returnUrlParam);
                            } else if (redirectUrl && redirectUrl.value) {
                                targetUrl = redirectUrl.value;
                            } else {
                                targetUrl = '/mybill-dashboard';
                            }
                            
                            // Add cache buster to URL
                            if (targetUrl.indexOf('?') > -1) {
                                targetUrl += '&' + cacheBuster;
                            } else {
                                targetUrl += '?' + cacheBuster;
                            }
                            
                            log('MyBill Login: Redirecting to:', targetUrl);
                            redirect(targetUrl);
                        }, 200);
                    }
                    else {
                        log('MyBill Login: Login failed with status:', status);
                        
                        var serverMessage = '';
                        var isUsernameNotRegistered = res.data?.isUsernameNotRegistered;
                        var isUsernameOrPasswordIncorrect = res.data?.isUsernameOrPasswordIncorrect;
                        var isEmailNotConfirmed = res.data?.isEmailNotConfirmed;

                        if (isUsernameOrPasswordIncorrect) {
                            serverMessage = form.querySelector("input[name='ServerInvalidCredentialMessage']")?.value || "Invalid BAN number or password.";
                        } else if (isUsernameNotRegistered) {
                            serverMessage = form.querySelector("input[name='ServerUsernameNotRegisteredMessage']")?.value || "BAN number is not registered.";
                        } else if (isEmailNotConfirmed) {
                            serverMessage = form.querySelector("input[name='ServerIsEmailNotConfirmed']")?.value || "Email was not confirmed. Confirmation email has been sent.";
                            showElement(resendContainer);
                        } else {
                            serverMessage = res.frontendErrorMessage || res.developerErrorMessage || "Incorrect BAN Number or Password entered.";
                        }
                        
                        if (errorMessageContainer) {
                            errorMessageContainer.innerText = serverMessage;
                            showElement(errorMessageContainer);
                        }
                        showElement(errorContainer);
                    }
                })
                .catch(function(err) {
                    error('MyBill Login: Error:', err);
                    hideLoader();
                    
                    if (errorMessageContainer) {
                        errorMessageContainer.innerText = "An error occurred. Please try again.";
                        showElement(errorMessageContainer);
                    }
                    showElement(errorContainer);
                });
            }).catch(function(err) {
                error('MyBill Login: Antiforgery token error:', err);
                hideLoader();
                
                if (errorMessageContainer) {
                    errorMessageContainer.innerText = "An error occurred. Please try again.";
                    showElement(errorMessageContainer);
                }
                showElement(errorContainer);
            });
        });

        if (resendVerification) {
            resendVerification.addEventListener('click', function (event) {
                var url = "/api/RegistrationWidget/ResendConfirmationEmail";
                var ResendVerificationModel = {
                    email: username?.value || '',
                    verificationPageUrl: ModalVerificationUrl?.value || ''
                };
                
                fetch(url, { 
                    method: 'POST', 
                    body: JSON.stringify(ResendVerificationModel), 
                    headers: { 'Content-Type': 'application/json' } 
                })
                .then(function(response) { return response.json(); })
                .then(function(res) {
                    if (res.statusCode >= 200 && res.statusCode < 400) {
                        alert("Confirmation email is sent successfully");
                        redirect("/");
                    } else {
                        if (errorMessageContainer) {
                            errorMessageContainer.innerText = res.frontendErrorMessage || res.title || "An error occurred";
                            showElement(errorMessageContainer);
                        }
                        showElement(errorContainer);
                    }
                })
                .catch(function(err) {
                    error('Error:', err);
                    if (errorMessageContainer) {
                        errorMessageContainer.innerText = "An error occurred";
                        showElement(errorMessageContainer);
                    }
                    showElement(errorContainer);
                });
            });
        }

        // Initialize - hide containers
        hideElement(successMessageContainer);
        hideElement(resendContainer);
        hideElement(errorContainer);
        hideLoader();
        
        function setAntiforgeryTokens() {
            return new Promise(function(resolve, reject) {
                var xhr = new XMLHttpRequest();
                xhr.open('GET', '/sitefinity/anticsrf');
                xhr.setRequestHeader('X-SF-ANTIFORGERY-REQUEST', 'true');
                xhr.responseType = 'json';
                xhr.onload = function () {
                    var response = xhr.response;
                    if (response != null && response.Value) {
                        var token = response.Value;
                        document.querySelectorAll("input[name='sf_antiforgery']").forEach(function(i) { 
                            i.value = token; 
                        });
                    }
                    resolve();
                };
                xhr.onerror = function () { 
                    error('MyBill Login: Failed to get antiforgery token');
                    resolve();
                };
                xhr.send();
            });
        }
        
        log('MyBill Login: Script initialized successfully');
    });
})();