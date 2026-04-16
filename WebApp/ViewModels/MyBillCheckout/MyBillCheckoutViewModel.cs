namespace VFL.Renderer.ViewModels.MyBillCheckout
{
    /// <summary>
    /// View model for MyBill Checkout form
    /// Contains all fields needed for payment processing
    /// </summary>
    public class MyBillCheckoutViewModel
    {
        // Configuration
        public string PageTitle { get; set; }
        public string SectionTitle { get; set; }
        public string TermsConditionsUrl { get; set; }
        public string PaymentResultPageUrl { get; set; }

        // Session data
        public string SessionId { get; set; }
        public bool SessionExpired { get; set; }

        // Invoice/Payment info
        public string Email { get; set; }
        public string InvoiceNumber { get; set; }
        public string BanNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsPartialPayment { get; set; }

        // Payment method
        public string PaymentMethod { get; set; }

        // Card billing information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BillingEmail { get; set; }
        public string BillingPhone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CountryCode { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        // Success/Failure Links
        public string CustomerSupportUrl { get; set; }
        public string DashboardUrl { get; set; }

        // Back Button Configuration
        public string BackButtonUrl { get; set; }
        public string BackButtonText { get; set; }
    }
}


