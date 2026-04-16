using VFL.Renderer.Common;

namespace VFL.Renderer.Services.Validation.Models
{
    public class ValidationResponse
    {
        public bool isUserEmailExists { get; set; }
        public bool isNumberRegistered { get; set; }
        public bool isNumberValid { get; set; }
        public bool isEmailConfirmed { get; set; }
        public bool isPostpayNumber { get; set; }
    }

    
}
