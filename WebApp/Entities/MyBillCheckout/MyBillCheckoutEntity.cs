using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VFL.Renderer.Entities.MyBillCheckout
{
    /// <summary>
    /// Entity for MyBill Checkout widget configuration in Sitefinity CMS
    /// Defines editable properties available in the widget designer
    /// </summary>
    public class MyBillCheckoutEntity
    {
        /// <summary>
        /// Allows selection of different checkout views
        /// </summary>
        [ViewSelector]
        public string ViewName { get; set; } = "Default";

        /// <summary>
        /// Page title displayed at the top of checkout page
        /// </summary>
        [ContentSection("Configuration", 1)]
        [DisplayName("Page Title")]
        [Description("Title displayed at the top of the checkout page")]
        public string PageTitle { get; set; } = "My Corporate";

        /// <summary>
        /// Section title for bill payment
        /// </summary>
        [ContentSection("Configuration")]
        [DisplayName("Section Title")]
        [Description("Section title for bill payment")]
        public string SectionTitle { get; set; } = "Bill Payment";

        /// <summary>
        /// Link to terms and conditions
        /// </summary>
        [ContentSection("Configuration")]
        [DisplayName("Terms and Conditions URL")]
        [Description("Link to terms and conditions page (e.g., /terms-and-conditions)")]
        [DefaultValue("/terms-and-conditions")]
        public string TermsConditionsUrl { get; set; } = "/terms-and-conditions";

        /// <summary>
        /// Terms and Conditions page selector (alternative to URL)
        /// </summary>
        [ContentSection("Configuration")]
        [DisplayName("Terms and Conditions Page (Optional)")]
        [Description("Select a page for terms and conditions (overrides URL if selected)")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext TermsConditionsPage { get; set; }

        /// <summary>
        /// Page to redirect after payment completion
        /// </summary>
        [ContentSection("Navigation", 2)]
        [DisplayName("Payment Result Page")]
        [Description("Page to redirect after payment completion")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext PaymentResultPage { get; set; }

        /// <summary>
        /// Customer Support URL
        /// </summary>
        [ContentSection("Success/Failure Links", 3)]
        [DisplayName("Customer Support URL")]
        [Description("URL to customer support page shown on success/failure screens (e.g., /contact-us)")]
        [DefaultValue("/contact-us")]
        public string CustomerSupportUrl { get; set; } = "/contact-us";

        /// <summary>
        /// Customer Support page selector (alternative to URL)
        /// </summary>
        [ContentSection("Success/Failure Links")]
        [DisplayName("Customer Support Page (Optional)")]
        [Description("Select a page for customer support (overrides URL if selected)")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext CustomerSupportPage { get; set; }

        /// <summary>
        /// Dashboard URL
        /// </summary>
        [ContentSection("Success/Failure Links")]
        [DisplayName("Dashboard URL")]
        [Description("URL to MyBill dashboard shown on success/failure screens (e.g., /mybill/mybill-dashboard)")]
        [DefaultValue("/mybill/mybill-dashboard")]
        public string DashboardUrl { get; set; } = "/mybill/mybill-dashboard";

        /// <summary>
        /// Dashboard page selector (alternative to URL)
        /// </summary>
        [ContentSection("Success/Failure Links")]
        [DisplayName("Dashboard Page (Optional)")]
        [Description("Select MyBill dashboard page (overrides URL if selected)")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext DashboardPage { get; set; }

        /// <summary>
        /// Back button URL
        /// </summary>
        [ContentSection("Navigation")]
        [DisplayName("Back Button URL")]
        [Description("URL for the Back button on checkout page (e.g., /mybill/mybill-dashboard)")]
        [DefaultValue("/mybill/mybill-dashboard")]
        public string BackButtonUrl { get; set; } = "/mybill/mybill-dashboard";

        /// <summary>
        /// Back button page selector (alternative to URL)
        /// </summary>
        [ContentSection("Navigation")]
        [DisplayName("Back Button Page (Optional)")]
        [Description("Select a page for the Back button (overrides URL if selected)")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext BackButtonPage { get; set; }

        /// <summary>
        /// Back button text
        /// </summary>
        [ContentSection("Navigation")]
        [DisplayName("Back Button Text")]
        [Description("Text displayed on the Back button")]
        [DefaultValue("Back")]
        public string BackButtonText { get; set; } = "Back";
    }
}
