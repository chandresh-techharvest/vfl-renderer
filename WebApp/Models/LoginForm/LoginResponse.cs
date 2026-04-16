using VFL.Renderer.Common;

namespace VFL.Renderer.Models.LoginForm
{
    public class LoginResponse
    {
        public bool isLoggedIn { get; set; }
        public string jwtToken { get; set; }
        public bool isCustomerCare { get; set; }
        public string RefreshToken { get; set; }

        public bool isUsernameNotRegistered { get; set; }
        public bool isUsernameOrPasswordIncorrect { get; set; }
        public bool isEmailNotConfirmed { get; set; }

        public bool isConfirmEmailSent { get; set; }
        public string username { get; set; }
    }


}
