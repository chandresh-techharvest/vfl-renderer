using System.Collections.Generic;

namespace VFL.Renderer.ViewModels.MyBillDashboard
{
    /// <summary>
    /// View model for MyBill Dashboard widget
    /// Contains data to be rendered in the Razor view
    /// </summary>
    public class MyBillDashboardViewModel
    {
        /// <summary>
        /// List of slider image URLs from Sitefinity media library
        /// </summary>
        public List<string> SliderImages { get; set; }

        /// <summary>
        /// Array of BAN accounts for the logged-in corporate user
        /// </summary>
        public BanAccount[] BanAccounts { get; set; }

        /// <summary>
        /// Redirect URL for unauthenticated users
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Indicates if the user is authenticated with MyBill
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Indicates if the widget is being viewed in Sitefinity designer mode
        /// </summary>
        public bool IsDesignerMode { get; set; }

        /// <summary>
        /// URL for the checkout page
        /// </summary>
        public string CheckoutPageUrl { get; set; }
    }

    /// <summary>
    /// Represents a BAN (Billing Account Number) account
    /// </summary>
    public class BanAccount
    {
        /// <summary>
        /// BAN account number (e.g., "945939845")
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Account name/label
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates if this BAN is currently selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// List of paperless email addresses associated with this account
        /// </summary>
        public PaperlessEmailInfo[] PaperlessEmails { get; set; }
    }

    /// <summary>
    /// Represents a paperless email information
    /// </summary>
    public class PaperlessEmailInfo
    {
        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Unique identifier for this paperless email
        /// </summary>
        public int Id { get; set; }
    }
}
