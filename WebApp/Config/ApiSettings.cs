namespace VFL.Renderer.Config
{
    /// <summary>
    /// Configuration settings for external APIs and page URLs
    /// Values loaded from appsettings.json
    /// </summary>
    public class ApiSettings
    {
        // ========================================
        // MyVodafone Portal Settings
        // ========================================
        
        /// <summary>
        /// MyVodafone backend API base URL
        /// Example: https://testonlineservices.vodafone.com.fj:4343/
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        // ========================================
        // MyBill Portal Settings
        // ========================================
        
        /// <summary>
        /// MyBill backend API base URL
        /// Example: https://testmybill.vodafone.com.fj:4343/
        /// </summary>
        public string MyBillBaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// MyBill login page URL
        /// Example: /mybill-login or /mybill/login
        /// </summary>
        public string MyBillLoginPath { get; set; } = "/my-bill-login";

        /// <summary>
        /// MyBill logout page URL
        /// Example: /mybill-logout or /
        /// </summary>
        public string MyBillLogoutPath { get; set; } = "/mybill-logout";

        /// <summary>
        /// MyBill dashboard page URL (redirect after successful login)
        /// Example: /mybill-dashboard or /mybill/dashboard
        /// </summary>
        public string MyBillDashboardPath { get; set; } = "/mybill-dashboard";

        /// <summary>
        /// MyBill profile page URL
        /// Example: /mybill-profile or /mybill/profile
        /// </summary>
        public string MyBillProfilePath { get; set; } = "/mybill-profile";

        /// <summary>
        /// MyBill transaction history page URL
        /// Example: /mybill-transactions or /mybill/transactions
        /// </summary>
        public string MyBillTransactionHistoryPath { get; set; } = "/mybill-transactions";

        /// <summary>
        /// MyBill reset password page URL
        /// Example: /mybill-reset-password or /mybill/reset-password
        /// </summary>
        public string MyBillResetPasswordPath { get; set; } = "/mybill-reset-password";

        /// <summary>
        /// MyBill support login redirect URL (where to redirect after successful support token login)
        /// If not set, falls back to MyBillDashboardPath
        /// Example: /mybill/mybill-dashboard
        /// </summary>
        public string MyBillSupportLoginRedirectUrl { get; set; } = "/mybill/mybill-dashboard";

        // ========================================
        // MyBill Token Settings
        // ========================================
        
        /// <summary>
        /// MyBill JWT expiry duration in minutes
        /// Default: 15 minutes
        /// </summary>
        public int MyBillJwtExpiryMinutes { get; set; } = 14;

        /// <summary>
        /// MyBill refresh token expiry duration in minutes
        /// This should match the backend refresh token expiry (7 days = 10080 minutes)
        /// Default: 10080 minutes (7 days)
        /// </summary>
        public int MyBillRefreshTokenExpiryMinutes { get; set; } = 10080;


        /// <summary>
        /// Refresh token expiry duration in minutes
        /// This should match the backend refresh token expiry (7 days = 10080 minutes)
        /// Default: 10080 minutes (7 days)
        /// </summary>
        public int RefreshTokenExpiryMinutes { get; set; } = 10080;

        /// <summary>
        /// JWT token expiry duration in minutes
        /// This should match the backend refresh token expiry 30 minutes
        /// Default: 30 minutes
        /// </summary>
        public int JwtExpiryMinutes { get; set; } = 30;

        public string LoginPath { get; set; }


    }
}
