namespace VFL.Renderer.Models.MyBillLoginForm
{
    public class MyBillLoginRequest
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        public string ErrorRedirectUrl { get; set; } = string.Empty;
        public string MembershipProviderName { get; set; } = string.Empty;
        public string sf_antiforgery { get; set; }
    }

    public class MyBillLoginRequest1
    {
        public string ban { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        
    }
}
