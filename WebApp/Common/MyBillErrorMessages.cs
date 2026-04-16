namespace VFL.Renderer.Common
{
    /// <summary>
    /// Centralized error messages for MyBill portal
    /// Use these constants instead of hardcoded strings for consistency
    /// </summary>
    public static class MyBillErrorMessages
    {
        // Authentication errors
        public const string SessionExpired = "Your session has expired. Please log in again.";
        public const string NotAuthenticated = "User is not authenticated";
        public const string InvalidAuthScheme = "Invalid authentication scheme";
        public const string AccessTokenMissing = "Access token is missing. Please log in again.";
        public const string TokenRefreshFailed = "Failed to refresh token";
        public const string LoginFailed = "Login failed";
        public const string InvalidCredentials = "Invalid BAN number or password.";
        public const string BanNotRegistered = "BAN number is not registered.";
        public const string EmailNotConfirmed = "Email was not confirmed. Confirmation email has been sent.";

        // Support login errors
        public const string TokenRequired = "Token is required";
        public const string TokenExpired = "This access token has expired. Please request a new token from support.";
        public const string InvalidToken = "Invalid or expired access token. Please request a new token from customer support.";

        // Validation errors
        public const string BanRequired = "BAN number is required";
        public const string EmailRequired = "At least one email is required";
        public const string InvalidEmailFormat = "Please enter a valid email address.";
        public const string RequiredFieldsMissing = "Please fill in all required fields.";
        public const string BanDigitsOnly = "BAN Number must contain only digits";
        public const string BanMinLength = "BAN Number must be at least 8 digits";
        public const string BanMaxLength = "BAN Number must not exceed 12 digits";
        public const string InvalidBanFormat = "Invalid BAN Number format";

        // Payment errors
        public const string PaymentMethodRequired = "Payment method is required";
        public const string InvalidPaymentMethod = "Invalid payment method";
        public const string PaymentAmountRequired = "Payment amount must be greater than 0";
        public const string PaymentAmountExceedsTotal = "Payment amount cannot exceed total amount";
        public const string MinimumPartialPayment = "Minimum partial payment is $10";
        public const string PaymentSessionExpired = "Checkout session expired. Please try again.";
        public const string PaymentServiceError = "Payment service error. Please try again.";
        public const string PaymentVerificationFailed = "Payment verification failed";
        public const string MissingTransactionId = "Missing transaction ID in callback";

        // Profile/Dashboard errors
        public const string FailedToRetrieveAccounts = "Failed to retrieve accounts";
        public const string FailedToRetrieveAccountSummary = "Failed to retrieve account summary";
        public const string FailedToRetrieveTransactions = "Failed to retrieve transaction history";
        public const string FailedToUpdateEmails = "Failed to update paperless emails. Please try again.";
        public const string NoBanAccountsFound = "No billing accounts found for this user. Please contact support to link your billing accounts.";
        public const string NoInvoicesFound = "No invoices found";
        public const string NoTransactionsFound = "No transactions found";

        // Download errors
        public const string FileNameRequired = "File name is required";
        public const string DownloadFailed = "Failed to download invoice";
        public const string InvoiceNotAvailable = "Invoice file not available for download";

        // General errors
        public const string UnexpectedError = "An unexpected error occurred. Please try again.";
        public const string ConnectionError = "Unable to connect to the server. Please check your internet connection and try again.";
        public const string ServerError = "An internal server error occurred. Please try again or contact support.";

        // Success messages
        public const string LoginSuccessful = "Login successful";
        public const string PaperlessEmailsUpdated = "Paperless emails updated successfully!";
        public const string PaymentSuccessful = "Payment successful";
        public const string PasswordResetEmailSent = "Password reset email sent successfully";
        public const string PasswordResetSuccessful = "Password reset successful";
    }
}
