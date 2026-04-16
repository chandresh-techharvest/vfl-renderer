(function () {
    document.addEventListener('DOMContentLoaded', function () {
        var hdnLandingPage = document.getElementById("hdnLandingPage");
        var btnResend = document.getElementById("btnResend");
        var hdnEmail = document.getElementById("hdnEmail");
        var hdnVerificationPageUrl = document.getElementById("hdnVerificationPageUrl");
        
        setTimeout(function () {
            window.location.href = hdnLandingPage.value;
        }, 3000);

        btnResend.addEventListener('click', function (event) {
            event.preventDefault();
            var resendUrl = "/api/RegistrationWidget/ResendConfirmationEmail";
            const ResendVerificationModel = {};
            ResendVerificationModel.email = hdnEmail.value;
            ResendVerificationModel.verificationPageUrl = hdnVerificationPageUrl.value; 
            window.fetch(resendUrl, { method: 'POST', body: JSON.stringify(ResendVerificationModel), headers: { 'Content-Type': 'application/json' } })
                .then(response => response.json())
                .then(res => {
                    if ((res.status == 0 || res.status >= 200) && res.status < 400) {
                        Swal.fire({
                            title: "A new confirmation email has been sent to your email address.",
                            text: "Congratulations!",
                            icon: "success",
                            button: "OK",
                        });
                    }
                    else {
                        Swal.fire({
                            title: res.frontendErrorMessage,
                            text: res.developerErrorMessage,
                            icon: "error",
                            button: "OK",
                        });
                    }
                });
        });

    });
});