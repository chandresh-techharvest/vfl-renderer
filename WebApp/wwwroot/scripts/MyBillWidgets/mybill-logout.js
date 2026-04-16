document.addEventListener("DOMContentLoaded", function () {
    var logoutLink = document.querySelector(".js-mybill-logout");
    if (!logoutLink) return;

    var logoutRedirectUrl = logoutLink.getAttribute("data-logout-redirect") || "/my-bill-login";

    logoutLink.addEventListener("click", function (event) {
        event.preventDefault();

        Swal.fire({
            title: "Are you sure?",
            text: "Do you want to logout from MyBill?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, logout",
            cancelButtonText: "Cancel",
            confirmButtonColor: "#d33",
            cancelButtonColor: "#6c757d"
        }).then((result) => {
            if (!result.isConfirmed) return;
            
            // Clear MyBill data cookie immediately on client-side
            document.cookie = "mybillData=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
            
            fetch("/api/MyBillLoginForm/Logout", {
                method: "GET",
                credentials: "include",
                redirect: "manual"
            })
                .then(function (response) {
                    if (response.ok || response.type === "opaqueredirect" || response.status === 0) {
                        Swal.fire({
                            title: "Logged out",
                            text: "You have been logged out successfully.",
                            icon: "success",
                            timer: 1500,
                            showConfirmButton: false
                        }).then(() => {
                            window.location.href = logoutRedirectUrl;
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
                    Swal.fire(
                        "Error",
                        "Something went wrong. Please try again later.",
                        "error"
                    );
                });
        });
    });
});
