/**
 * MyBill Checkout Success Page
 * Reads payment result from localStorage and populates the table.
 * Security: validates that the stored result actually has isSuccessful === true.
 * If not, redirects to the fail page so users cannot fake a success by editing the URL.
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

        // Security check: only show success page if the backend actually confirmed success
        if (!data.isSuccessful) {
            // Data exists but payment was not successful — redirect to fail page
            // Keep the data in localStorage so the fail page can display it
            window.location.href = window.location.pathname + "?result=fail";
            return;
        }

        // Payment was genuinely successful — populate the table
        document.getElementById("resultDate").innerText = data.date
            ? new Date(data.date).toLocaleDateString()
            : "-";
        document.getElementById("resultOrderRef").innerText = data.orderReference || "-";
        document.getElementById("resultEmail").innerText = data.fullName || data.email || "-";
        document.getElementById("resultInvoice").innerText = data.invoiceNumber || "-";
        document.getElementById("resultAmount").innerText = data.amount || "-";

        // Clean up after use
        localStorage.removeItem("mybill_payment_result");
    } catch (e) {
        localStorage.removeItem("mybill_payment_result");
        window.location.href = "/mybill/mybill-dashboard";
    }
});
