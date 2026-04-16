using VFL.Renderer.Common;

namespace VFL.Renderer.Models.LoginForm
{
    public class RefreshTokenResponse : APIBaseResponseModel
    {
        public RefreshTokenDataResponse? data { get; set; }
    }

    public class RefreshTokenDataResponse 
    {
        public string? AccessToken { get; set; }
        public string? ExpiresIn { get; set; }
    }
}
