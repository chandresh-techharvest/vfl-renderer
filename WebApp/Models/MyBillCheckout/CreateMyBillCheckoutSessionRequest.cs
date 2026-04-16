namespace VFL.Renderer.Models.MyBillCheckout
{
    /// <summary>
    /// Request model for creating a MyBill checkout session
    /// </summary>
    public class CreateMyBillCheckoutSessionRequest
    {
        /// <summary>
        /// Email address for payment confirmation
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Billing Account Number (BAN)
        /// </summary>
        public string BanNumber { get; set; }

        /// <summary>
        /// Invoice number being paid
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Full invoice amount
        /// </summary>
        public decimal FullAmount { get; set; }

        /// <summary>
        /// Payment amount (can be partial)
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// Indicates if this is a partial payment
        /// </summary>
        public bool IsPartialPayment { get; set; }

        /// <summary>
        /// Page URL to redirect after payment
        /// </summary>
        public string PageUrl { get; set; }
    }
}
