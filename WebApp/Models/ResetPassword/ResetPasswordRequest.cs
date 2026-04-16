namespace VFL.Renderer.Models.ResetPassword
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
        public string VerificationPageUrl { get; set; }
        public string MembershipProviderName { get; set; }
        public string RegistrationPageUrl { get; set; }
        public string ResetPasswordUrl { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string user { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public string NewPassword { get; set; }
        public string VerificationPageUrl { get; set; }
        public string SecurityToken { get; set; }
        public string MembershipProviderName { get; set; }
    }
}
