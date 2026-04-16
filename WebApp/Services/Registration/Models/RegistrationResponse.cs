using VFL.Renderer.Common;

namespace VFL.Renderer.Services.Registration.Models
{
    public class RegistrationResponse : APIBaseResponseModel
    {
        public bool isRegistered { get; set; }
        public bool isConfirmed { get; set; }
          public bool accountIsActive { get; set; }
    }


}
