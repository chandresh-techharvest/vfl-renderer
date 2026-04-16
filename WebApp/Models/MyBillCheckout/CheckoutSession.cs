using System;

namespace VFL.Renderer.Models.MyBillCheckout
{
    /// <summary>
    /// Base class for MyBill checkout session data
    /// </summary>
    public class CheckoutSession
    {
        /// <summary>
        /// Unique identifier for the session
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Email address associated with the checkout
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// URL of the page where checkout was initiated
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        /// Type of page (e.g., MyBillPayment)
        /// </summary>
        public string PageType { get; set; }

        /// <summary>
        /// When the session was created
        /// </summary>
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
