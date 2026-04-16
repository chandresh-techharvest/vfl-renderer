namespace VFL.Renderer.Models.LoginForm
{
    public class LoginRequest
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        public string ErrorRedirectUrl { get; set; } = string.Empty;
        public string MembershipProviderName { get; set; } = string.Empty;
        public string sf_antiforgery { get; set; }
    }

    public class GoogleTokenRequest
    {
        public string IdToken { get; set; }
    }

    public class GoogleUserPayload
    {
        public string aud { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        // Add other fields as needed
    }
}
