using VFL.Renderer.Services.WebTopUp.Models;

namespace VFL.Renderer.Services.MyBillPayment.Models
{
    /// <summary>
    /// Request model for payment verification callback
    /// Supports both M-PAISA and Credit Card payment callbacks
    /// </summary>
    public class MyBillProvideUpdateRequest
    {
        /// <summary>
        /// Invoice number
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Billing Account Number
        /// </summary>
        public string SelectedBAN { get; set; }

        /// <summary>
        /// Payment gateway transaction ID
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Payment status from gateway
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Additional data from payment gateway
        /// </summary>
        public string RawData { get; set; }

        /// <summary>
        /// M-PAISA payment callback data
        /// </summary>
        public MPaisaPaymentCallback mPaisaPayment { get; set; }

        /// <summary>
        /// Credit card payment callback data
        /// </summary>
        public CreditCardPaymentCallback creditCardPayment { get; set; }
    }
}
