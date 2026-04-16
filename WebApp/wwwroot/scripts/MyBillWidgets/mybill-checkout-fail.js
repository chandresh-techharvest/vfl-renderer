/**
 * MyBill Checkout Fail Page
 * Reads payment result from localStorage and populates the table.
 * Security: if someone navigates here with no data, redirects to dashboard.
 * If stored data actually shows isSuccessful===true, redirects to success page.
 */
document.addEventListener("DOMContentLoaded", function () {
    var paymentData = localStorage.getItem("mybill_payment_result");

    if (!paymentData) {
        // No data at all — user navigated here directly, redirect to dashboard
        window.location.href = "/mybill/mybill-dashboard";
        return;
    }

    try {
        var data = JSON.parse(paymentData);

        // Security check: if payment was actually successful, redirect to success page
        if (data.isSuccessful === true) {
            // Keep data in localStorage so success page can display it
            window.location.href = window.location.pathname + "?result=success";
            return;
        }

        // Payment failed — populate the table
        document.getElementById("resultDate").innerText = data.date
            ? new Date(data.date).toLocaleDateString()
            : "-";
        document.getElementById("resultOrderRef").innerText = data.orderReference || "-";
        document.getElementById("resultEmail").innerText = data.fullName || data.email || "-";
        document.getElementById("resultInvoice").innerText = data.invoiceNumber || "-";
        document.getElementById("resultAmount").innerText = data.amount || "-";

        // Show error message if present
        if (data.errorMessage) {
            var errorText = document.getElementById("errorMessageText");
            var errorAlert = document.getElementById("errorMessageAlert");
            if (errorText) errorText.textContent = data.errorMessage;
            if (errorAlert) errorAlert.style.display = "block";
        }

        // Clean up after use
        localStorage.removeItem("mybill_payment_result");
    } catch (e) {
        localStorage.removeItem("mybill_payment_result");
        window.location.href = "/mybill/mybill-dashboard";
    }
});
