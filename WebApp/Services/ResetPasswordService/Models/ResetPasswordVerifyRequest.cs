namespace VFL.Renderer.Services.ResetPasswordService.Models
{
    public class ResetPasswordVerifyRequest
    {
        public string Password { get; set; }
        public string Token { get; set; }
        public string User { get; internal set; }
    }
}
