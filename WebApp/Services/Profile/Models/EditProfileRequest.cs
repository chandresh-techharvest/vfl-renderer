using Microsoft.AspNetCore.Http;

namespace VFL.Renderer.Services.Profile.Models
{
    public class EditProfileRequest
    {
        public IFormFile ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Id { get; set; }
        public string sf_antiforgery { get; set; }
    }

}
