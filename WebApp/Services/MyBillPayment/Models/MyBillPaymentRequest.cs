using VFL.Renderer.Services.WebTopUp.Models;

namespace VFL.Renderer.Services.MyBillPayment.Models
{
    /// <summary>
    /// Request model for MyBill payment processing
    /// </summary>
    public class MyBillPaymentRequest
    {
        /// <summary>
        /// Invoice number being paid
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Selected Billing Account Number (BAN)
        /// </summary>
        public string SelectedBAN { get; set; }

        /// <summary>
        /// Payment amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Indicates if this is a partial payment
        /// </summary>
        public bool IsPartialPayment { get; set; }

        /// <summary>
        /// Payment method and billing details
        /// </summary>
        public PaymentInformationRequest PaymentInformationRequest { get; set; }
    }
}
