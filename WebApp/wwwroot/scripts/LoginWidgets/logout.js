document.addEventListener("DOMContentLoaded", function () {
    var logoutLink = document.querySelector(".js-logout");
    if (!logoutLink) return;

     logoutLink.addEventListener("click", function (event) {
        event.preventDefault();

         Swal.fire({
             title: "Are you sure?",
             text: "Do you want to logout?",
             icon: "warning",
             showCancelButton: true,
             confirmButtonText: "Yes, logout",
             cancelButtonText: "Cancel",
             confirmButtonColor: "#d33",
             cancelButtonColor: "#6c757d"
         }).then((result) => {
             if (!result.isConfirmed) return;
             
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
    });
     });
});
