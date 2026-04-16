document.addEventListener("DOMContentLoaded", function () {



    function clearCheckoutData() {
        localStorage.removeItem("webtopup_checkout");
    }
    function getCheckoutData() {
        const data = localStorage.getItem("webtopup_checkout");
        return data ? JSON.parse(data) : null;
    }

    const data = getCheckoutData();

    // Change URL in browser without reloading
    if (window.location.pathname !== `/main/${data.pageType}/success` ) {
        window.history.replaceState({}, "", `/main/${data.pageType}/success`);
    }

    const pageTitle = data.pageType;
    const titleElement = document.getElementById("pageTitle");
    if (titleElement) {
        titleElement.innerText = `${pageTitle} Successful`;
    }
    const paymentData = localStorage.getItem("payment_result");
   
    if (paymentData) {
        const data = JSON.parse(paymentData);

        // Fill table with correct keys
        document.getElementById("successDate").innerText = new Date(data.date).toLocaleDateString(); // parse ISO string
        document.getElementById("successOrderRef").innerText = data.orderReference || "N/A";
        document.getElementById("successEmail").innerText = data.email || "N/A";
        document.getElementById("successNumber").innerText = data.number || "N/A";
        document.getElementById("successAmount").innerText = data.amount || "N/A";

        // Remove data after use
        localStorage.removeItem("payment_result");
        clearCheckoutData();
    } else {
        // fallback if no data
        document.getElementById("successDate").innerText = "-";
        document.getElementById("successOrderRef").innerText = "-";
        document.getElementById("successEmail").innerText = "-";
        document.getElementById("successNumber").innerText = "-";
        document.getElementById("successAmount").innerText = "-";
        localStorage.removeItem("payment_result");
        clearCheckoutData();
    }
});