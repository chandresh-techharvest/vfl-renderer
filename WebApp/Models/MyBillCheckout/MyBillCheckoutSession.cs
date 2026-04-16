namespace VFL.Renderer.Models.MyBillCheckout
{
    /// <summary>
    /// Session model for MyBill payment checkout
    /// Extends base CheckoutSession with MyBill-specific fields
    /// </summary>
    public class MyBillCheckoutSession : CheckoutSession
    {
        /// <summary>
        /// Billing Account Number (BAN) for the payment
        /// </summary>
        public string BanNumber { get; set; }

        /// <summary>
        /// Invoice number being paid
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Full amount of the invoice
        /// </summary>
        public decimal FullAmount { get; set; }

        /// <summary>
        /// Actual payment amount (can be partial)
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// Indicates if this is a partial payment
        /// </summary>
        public bool IsPartialPayment { get; set; }
    }
}
