document.addEventListener("DOMContentLoaded", function () {

    function clearCheckoutData() {
        localStorage.removeItem("webtopup_checkout");
    }

    function getCheckoutData() {
        const data = localStorage.getItem("webtopup_checkout");
        return data ? JSON.parse(data) : null;
    }

    const checkoutData = getCheckoutData();
   
    // Change URL in browser without reloading
    if (window.location.pathname !== `/main/${checkoutData.pageType}/fail`) {
        window.history.replaceState({}, "", `/main/${checkoutData.pageType}/fail`);
    }


    const pageTitle = checkoutData.pageType;
    const titleElement = document.getElementById("pageTitle");
    if (titleElement) {
        titleElement.innerText = `${pageTitle} Unsuccessful`;
    }
    const paymentData = localStorage.getItem("payment_result");

   
    if (paymentData) {
        const data = JSON.parse(paymentData);

        // Fill table with correct keys
        document.getElementById("failDate").innerText = new Date(data.date).toLocaleDateString() || "-";
        document.getElementById("failOrderRef").innerText = data.orderReference || "-";
        document.getElementById("failEmail").innerText = data.email || "-";
        document.getElementById("failNumber").innerText = data.number || "-";
        document.getElementById("failAmount").innerText = data.amount || "-";

        // Remove data after use
        localStorage.removeItem("payment_result");
        clearCheckoutData();
    } else {
        // fallback if no data
        document.getElementById("failDate").innerText = "-";
        document.getElementById("failOrderRef").innerText = "-";
        document.getElementById("failEmail").innerText = "-";
        document.getElementById("failNumber").innerText = "-";
        document.getElementById("failAmount").innerText = "-";

        localStorage.removeItem("payment_result");
        clearCheckoutData();
    }
});