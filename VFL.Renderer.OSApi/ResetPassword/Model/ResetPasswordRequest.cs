using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFL.Renderer.OSApi.ResetPassword.Model
{
    public class ResetPasswordRequest
    {
        public string user { get; set; }
        public string password { get; set; }
        public string token { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
        public string VerificationPageUrl { get; set; }
    }
}
