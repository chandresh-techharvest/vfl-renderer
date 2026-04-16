$(function () {
    var viewMode = $("input[name='ViewMode']").val();
    var visibilityClassHidden = $("[data-sf-visibility-hidden]").data("sfVisibilityHidden");
    var $readContainer = $("[data-sf-role='read-container']");
    var $formContainer = $("[data-sf-role='form-container']");
    var $errorMessageContainer = $("[data-sf-role='error-message-container']");
    var $successMessageContainer = $("[data-sf-role='success-message-container']");
    var invalidPhotoErrorMessage = $formContainer.find("input[name='InvalidPhotoErrorMessage']").val();
    var invalidPasswordErrorMessage = $formContainer.find("input[name='InvalidPasswordErrorMessage']").val();
    var confirmEmailChangeRequest = $formContainer.find("input[name='ConfirmEmailChangeRequest']").val().toLowerCase() === 'true';
    var confirmEmailChangeError = $formContainer.find("input[name='ConfirmEmailChangeError']").val().toLowerCase() === 'true';
    


    function showElement($el) {
        if (!$el.length) return;
        if (visibilityClassHidden) {
            $el.removeClass(visibilityClassHidden);
        } else {
            $el.show();
        }
    }
    function hideElement($el) {
        if (!$el.length) return;
        if (visibilityClassHidden) {
            $el.addClass(visibilityClassHidden);
        } else {
            $el.hide();
        }
    }
    function redirect(url) {
        window.location = url;
    }

    function validateFile(file) {
        var fileSize = file.size;
        var maxSize = 25 * 1024 * 1024;
        var allowedExtensions = $formContainer.find('input[data-sf-role="sf-allowed-avatar-formats"]').val();
        var fileName = file.name;
        var fileExtension = fileName.substr(fileName.lastIndexOf('.')).toLowerCase();
        if (fileSize > maxSize) {
            showErrorMessage(invalidPhotoErrorMessage.replace('{0}', maxSize).replace('{1}', allowedExtensions));
            return false;
        }
        return true;
    }

    function showSuccessMessage() {
        hideElement($errorMessageContainer);
        showElement($successMessageContainer);
    }
    function hideMessages() {
        hideElement($errorMessageContainer);
        hideElement($successMessageContainer);
    }
    function showErrorMessage(message, isHtml) {
        if (message) {
            if (isHtml) {
                $errorMessageContainer.html(message);
            } else {
                $errorMessageContainer.text(message);
            }
        }
        if ($errorMessageContainer.text()) {
            hideElement($successMessageContainer);
            showElement($errorMessageContainer);
        }
    }

    function setAntiforgeryTokens() {
        return $.ajax({
            url: '/sitefinity/anticsrf',
            method: 'GET',
            headers: { 'X-SF-ANTIFORGERY-REQUEST': 'true' },
            dataType: 'json'
        }).done(function (response) {
            if (response && response.Value) {
                $("input[name='sf_antiforgery']").val(response.Value);
            }
        });
    }
    
    function initRead(viewMode) {
        if (viewMode === "ReadEdit") {
            $("[data-sf-role='editProfileLink']").on('click', function (e) {
                e.preventDefault();
                hideElement($readContainer);
                showElement($formContainer);
            });
        }
    }
    function isNotEmpty(attr) {
        return (attr && attr !== "");
    }
    function processCssClass(str) {
        var classList = str.split(" ");
        return classList;
    }
    function initEdit(viewMode) {
        var $form = $formContainer.find("form");
        var $fileUploadInput = $("[data-sf-role='edit-profile-upload-picture-input']");
        var $editProfileUserImage = $("[data-sf-role='sf-user-profile-avatar']");
        var $invalidClassElement = $("[data-sf-invalid]");
        var classInvalidValue = $invalidClassElement.data("sfInvalid");
        var classInvalid = classInvalidValue ? classInvalidValue.split(" ") : [];
        var invalidDataAttr = "data-sf-invalid";
        var showPasswordPrompt = false;
        var showConfirmEmailChanges = false;
        var invalidEmailErrorMessage = $formContainer.find("input[name='InvalidEmailErrorMessage']").val();
        var validationRequiredErrorMessage = $formContainer.find("input[name='ValidationRequiredMessage']").val();
        var activationMethod = $formContainer.find("input[name='ActivationMethod']").val();
        var $confirmChangeContainer = $("[data-sf-role='confirm-email-change-container']");
        var showSendAgainActivationLink = $confirmChangeContainer.find("[data-sf-role='show-send-again-activation-link']").val().toLowerCase() === 'true';
        var $confirmationForm = $confirmChangeContainer.find('form');

        // Password fields toggle and validation
        var $changePassword = $("#changePassword");
        var $newPasswordWrapper = $(".NewPasswordWrapper");
        var $confirmPasswordWrapper = $(".ConfirmPasswordWrapper");
        var $newPassword = $("#Password");
        var $confirmPassword = $("#ConfirmPassword");

        var invalidClassElement = document.querySelector('[data-sf-invalid]');
        var classInvalidValue = invalidClassElement.dataset && isNotEmpty(invalidClassElement.dataset.sfInvalid) ? invalidClassElement.dataset.sfInvalid : null;
        var classInvalid = classInvalidValue ? processCssClass(classInvalidValue) : null;
        var invalidDataAttr = "data-sf-invalid";
        const EmailalreadyRegisteredMessage = "* Another account exists with the email entered.";
        const EmailalreadyRegisteredNotConfirmedMessage = "* Email is registered, but not confirmed.";
        // Hide password fields initially
        $newPasswordWrapper.hide();
        $confirmPasswordWrapper.hide();

        // Custom validation method for strong password
        $.validator.addMethod("strongPassword", function (value, element) {
            return this.optional(element) || /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+]).{8,}$/.test(value);
        }, "Password must meet complexity requirements.");   
        // jQuery Validate setup
        $form.validate({
            onkeyup: false,
            onfocusout: false,  
            ignore: [],
            rules: {
                FirstName: { required: true },
                LastName: { required: true },
                Email: {
                    required: true,
                    email: true,
                }
            }, 
              
            messages: {
                FirstName: { required: validationRequiredErrorMessage },
                LastName: { required: validationRequiredErrorMessage },
                Email: {
                    required: validationRequiredErrorMessage,
                    email: invalidEmailErrorMessage
                }
            },
            errorElement: "p",
            errorPlacement: function (error, element) {
                var container = $("div[data-sf-role='error-message-container']");
                if (!container.length) return;
                var fieldName = $(element).attr('name');
                container.find('p[data-field="' + fieldName + '"]').remove();
                error.attr('data-field', fieldName);
                error.appendTo(container);
                invalidateElement(element);
                container.removeClass("d-none").show();
            },
            success: function (label, element) {
                $(element).removeClass(classInvalid.join(' '));
                $(element).removeAttr(invalidDataAttr);
                var fieldName = $(element).attr('name');
                $errorMessageContainer.find('p[data-field="' + fieldName + '"]').remove();
                if ($errorMessageContainer.find('p').length === 0) {
                    hideElement($errorMessageContainer);
                    $errorMessageContainer.addClass('d-none');
                }
            },
            submitHandler: function (form) {
                hideMessages();
                var initialMail = $formContainer.find("input[name='InitialEmail']").val();
                var currentEmail = $form.find("input[name='Email']").val();
                if (!showPasswordPrompt) {
                    $form.find("input[name='Password']").val("");
                }
                if (initialMail === currentEmail) {
                    if (showPasswordPrompt) {
                        setAntiforgeryTokens().then(function () {
                            submitFormHandler($form[0], null, postSubmitAction, onSubmitError);
                        }, function () {
                            showErrorMessage("Antiforgery token retrieval failed");
                        });
                    } else {
                        setAntiforgeryTokens().then(function () {
                            submitFormHandler($form[0], null, postSubmitAction, onSubmitError);
                        }, function () {
                            showErrorMessage("Antiforgery token retrieval failed");
                        });
                    }
                    return false;
                }  



                // Email is different — check if new email is already registered
                $.ajax({
                    url: "/api/Validation/CheckEmailIsRegistered",
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(currentEmail)
                }).done(function (res) {
                    if (res.data && res.data.isUserEmailExists === true) {
                        if (res.data.isEmailConfirmed === false) {
                            showErrorMessage("* Email is registered but not confirmed. Please use a different email.");
                        } else {
                            showErrorMessage("* Another account already exists with this email address.");
                        }
                        invalidateElement($form.find("input[name='Email']"));
                    } else {
                        // Email is not registered — proceed with update
                        if (!showPasswordPrompt) {
                            hideMessages();
                            hideElement($("[data-sf-role='edit-profile-container']"));
                            showElement($("[data-sf-role='password-container']"));
                            showPasswordPrompt = true;
                            showConfirmEmailChanges = true;
                            return;
                        }

                        setAntiforgeryTokens().then(function () {
                            submitFormHandler($form[0], null, postSubmitAction, onSubmitError);
                        }, function () {
                            showErrorMessage("Antiforgery token retrieval failed");
                        });
                    }
                }).fail(function () {
                    showErrorMessage("Unable to verify email. Please try again.");
                });

                return false;
            }
        });
              
            
           
           
               

        $fileUploadInput.on('change', function (event) {
            if (event.target.files && event.target.files[0]) {
                var reader = new FileReader();
                reader.onload = function (readerLoadedEvent) {
                    $editProfileUserImage.attr('src', readerLoadedEvent.target.result);
                };
                reader.readAsDataURL(event.target.files[0]);
            }
        });

        var invalidateElement = function (element) {
            var $element = $(element);
            if (!$element.length) return;
            if (Array.isArray(classInvalid) && classInvalid.length > 0) {
                $element.addClass(classInvalid.join(' '));
            } $element.attr(invalidDataAttr, "");
        };

           

           

        function submitFormHandler(form, url, onSuccess, onError) {
            url = url || $(form).attr('action');
            var request = new FormData();
            $.each(form.elements, function (i, field) {
                if (field.type === 'submit') return;
                if (field.type === 'file' && field.files.length > 0) {
                    request.append('ProfileImage', field.files[0]);
                    return;
                }
                if (field.value) {
                    request.append(field.name, field.value);
                }
            });
            if ($("input[name='Password']").val() != "") {
                console.log($form.find("input[name='Password']").val());
                request.append("Password", $("input[name='Password']").val());
            }
            console.log(request);
            $.ajax({
                url: url,
                method: 'POST',
                data: request,
                processData: false,
                contentType: false,
                dataType: 'json'
            }).done(function (res) {
                if (res.statusCode === 0 || (res.statusCode >= 200 && res.statusCode < 400)) {
                    if (res.data.isUpdated) {
                        Swal.fire({
                            title: "Profile Updated Successfully!!!",
                            text: "You clicked the button!",
                            icon: "success",
                            button: "Aww yiss!",
                        }).then(() => {
                            if ($("input[name='ConfirmPassword']").val()) {
                                fetch("/api/LoginForm/Logout", {
                                    method: "GET",
                                    credentials: "include"
                                })
                                    .then(function (response) {
                                        console.log(response);
                                        if (response.ok) {
                                            Swal.fire({
                                                title: "Logged out",
                                                text: "You have been logged out successfully.",
                                                icon: "success",
                                                timer: 1500,
                                                showConfirmButton: false
                                            }).then(() => {
                                                window.location.href = "/Login";
                                            });
                                        } else {
                                            Swal.fire(
                                                "Logout Failed",
                                                "Unable to logout. Please try again.",
                                                "error"
                                            );
                                        }
                                    })
                                    .catch(function (error) {
                                        console.error("Logout error:", error);
                                        Swal.fire(
                                            "Error",
                                            "Something went wrong. Please try again later.",
                                            "error"
                                        );
                                    });
                            }
                            else {
                                location.reload();
                            }
                        });
                    } else {
                        Swal.fire({
                            title: "Something went wrong",
                            text: res.frontendErrorMessage,
                            icon: "warning",
                            confirmButtonText: "Retry"
                        });
                    }
                } else {
                    if (onError) {
                        onError(res.frontendErrorMessage, res.errors, res.statusCode);
                    }
                }
            }).always(function () {
                if (showPasswordPrompt) {
                    $formContainer.find('input[name=Password]').val('');
                    showElement($("[data-sf-role='edit-profile-container']"));
                    hideElement($("[data-sf-role='password-container']"));
                    showPasswordPrompt = false;
                }
            });
        }

        function postSubmitAction(response) {
            if (showConfirmEmailChanges && activationMethod == "AfterConfirmation") {
                hideElement($("[data-sf-role='profile-container']"));
                showElement($confirmChangeContainer);
            } else {
                var action = $formContainer.find("input[name='PostUpdateAction']").val();
                switch (action) {
                    case "ViewMessage":
                        // Optionally re-bind form fields here
                        hideElement($errorMessageContainer);
                        showSuccessMessage();
                        break;
                    case "RedirectToPage":
                        var redirectUrl = $formContainer.find("input[name='RedirectUrl']").val();
                        redirect(redirectUrl);
                        break;
                    case "SwitchToReadMode":
                        hideElement($formContainer);
                        showElement($readContainer);
                        redirect(window.location);
                        break;
                }
            }
        }

        function onSubmitError(errorMessage, responseFieldsErrors, status) {
            if (status === 403) errorMessage = invalidPasswordErrorMessage;
            var fieldErrors = [];
            if (errorMessage) fieldErrors.push(errorMessage);
            if (responseFieldsErrors) {
                $.each(responseFieldsErrors, function (key, msg) {
                    var $input = $formContainer.find("input[name='" + key + "'], textarea[name='" + key + "'], img[name='" + key + "']");
                    if ($input.length) {
                        $input.addClass(classInvalid.join(' ')).attr(invalidDataAttr, "");
                        var label = $formContainer.find("label[for='" + $input.attr('id') + "']").text();
                        fieldErrors.push(msg.replace("{0}", label || key));
                    } else {
                        fieldErrors.push(msg);
                    }
                });
            }
            showErrorMessage(fieldErrors.join('<br />'), true);
        }

        function confirmationSuccess() {
            $confirmChangeContainer.find('[data-sf-role="confirm-email-change-title"] h2').text($confirmChangeContainer.find('[data-sf-role="confirm-email-change-success-title"]').val());
            $confirmChangeContainer.find('[data-sf-role="confirm-email-change-message"]').text($confirmChangeContainer.find('[data-sf-role="confirm-email-change-success-message"]').val());
            $confirmationForm.find('[type="submit"]').val($confirmChangeContainer.find('[data-sf-role="send-again-label"]').val());
        }

        if (confirmEmailChangeRequest && activationMethod == "AfterConfirmation") {
            if (confirmEmailChangeError) {
                hideElement($("[data-sf-role='profile-container']"));
                showElement($confirmChangeContainer);

                if (showSendAgainActivationLink) {
                    $confirmationForm.on('submit', function (event) {
                        event.preventDefault();
                        setAntiforgeryTokens().then(function () {
                            submitFormHandler($confirmationForm[0], null, confirmationSuccess);
                        }, function () {
                            showErrorMessage("Antiforgery token retrieval failed");
                        });
                    });
                    showElement($confirmationForm.find('[type="submit"]'));
                }
                return;
            } else {
                showSuccessMessage();
            }
        }



        // Toggle password fields and validation rules
        $changePassword.on("change", function () {

            if (this.checked) {
                $newPasswordWrapper.show();
                $confirmPasswordWrapper.show();
                showPasswordPrompt = true;
                $newPassword.rules("add", {
                    required: true,
                    strongPassword: true
                });
                $confirmPassword.rules("add", {
                    required: true,
                    equalTo: "#Password",
                    messages: {
                        equalTo: "* The confirm password entered does not match the new password value entered."
                    }
                });
            } else {
                $newPasswordWrapper.hide();
                $confirmPasswordWrapper.hide();

                $newPassword.rules("remove");
                $confirmPassword.rules("remove");
                $newPassword.val('');
                $confirmPassword.val('');
            }
        });




    }

    switch (viewMode) {
        case "Edit":
            initEdit(viewMode);
            showElement($formContainer);
            hideElement($readContainer);
            break;
        case "Read":
            initRead(viewMode);
            showElement($readContainer);
            hideElement($formContainer);
            break;
        case "ReadEdit":
            initRead(viewMode);
            initEdit(viewMode);
            showElement($readContainer);
            hideElement($formContainer);
            break;
    }

});

                   

               

               

                

               

      