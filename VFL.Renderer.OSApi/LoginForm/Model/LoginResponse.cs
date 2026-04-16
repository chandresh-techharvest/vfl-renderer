using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFL.Renderer.OSApi.Common;

namespace VFL.Renderer.OSApi.LoginForm.Model
{
    public class LoginResponse : BaseResponse
    {
        public LoginDataResponse data { get; set; }
        public string Message { get; set; }
    }
    public class LoginDataResponse
    {
        public bool isLoggedIn { get; set; }
        public string jwtToken { get; set; }
        public bool isCustomerCare { get; set; }
        public string? AccessToken { get; set; }
        public string? ExpiresIn { get; set; }
    }
}
