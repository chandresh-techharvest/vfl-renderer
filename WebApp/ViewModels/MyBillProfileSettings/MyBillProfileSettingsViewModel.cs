namespace VFL.Renderer.ViewModels.MyBillProfileSettings
{
    /// <summary>
    /// View model for MyBill Profile Settings widget
    /// Contains user profile data and authentication state
    /// </summary>
    public class MyBillProfileSettingsViewModel
    {
        /// <summary>
        /// Indicates if user is authenticated with MyBill
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Indicates if widget is being viewed in Sitefinity designer mode
        /// </summary>
        public bool IsDesignerMode { get; set; }

        /// <summary>
        /// Redirect URL for authentication
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Selected BAN number
        /// </summary>
        public string SelectedBan { get; set; }

        /// <summary>
        /// Account name
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Contact full name (first + last)
        /// </summary>
        public string ContactFullName { get; set; }

        /// <summary>
        /// Contact phone number
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }
    }
}
