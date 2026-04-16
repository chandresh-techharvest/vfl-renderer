using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFL.Renderer.OSApi.Registration.Model
{
    public class RegistrationRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }        
        public string VerificationPageUrl { get; set; }
    }

    public class EmailVerify
    {
        public string token { get; set; }
        public string username { get; set; }
    }

    public class ResendEmailVerify
    {
        public string email { get; set; }
        public string verificationPageUrl { get; set; }
    }


    public class RegistrationNumber
    {
        public string number { get; set; }
    }

}
