using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.Renderer.Entities.Content;
using System.Text.Json.Serialization;

namespace VFL.Renderer.Services.Registration.Models
{
    
    public class RegistrationRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public string VerificationPageUrl { get; set; }
        public string ActivationPageUrl { get; set; }
        public string sf_antiforgery { get; set; }
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

}
