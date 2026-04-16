using VFL.Renderer.Common;
using VFL.Renderer.Entities.ProfileSettings;

namespace VFL.Renderer.Services.Profile.Models
{
    public class ProfileSettingsResponse
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public string email { get; set; }
        public string profileImageData { get; set; }
        public Device[] devices { get; set; }
        public string Warning { get; set; }
    }

    public class Device
    {
        public string number { get; set; }
        public string name { get; set; }

        public bool isSelected { get; set; }

    }


}
