using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;

namespace VFL.Renderer.ViewModels.Checkout
{
    public class CheckoutViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PlanCode { get; set; }
        public decimal Amount { get; set; }

        // Billing
        public string PaymentMethod { get; set; }

        // Card billing
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
        /// <summary>
        /// Only page url without root eg:/webtopup
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        /// eg: WebTopUp, PurchasePlan
        /// </summary>
        public string PageType { get; set; }

        public string CustomerSuportPageUrl { get; set; }
        public string HomePageUrl { get; set; }

        public PageNodeDto TermsConditionsPage { get; set; }

    }
}
