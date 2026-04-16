using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;

namespace Vfl.Renderer.ViewModels.LoginStatus
{
    public class LoginStatusViewModel
    {
        public PageNodeDto LoginPage { get; set; }
        public PageNodeDto RegistrationPage { get; set; }
        public PageNodeDto ProfilePage { get; set; }
        public PageNodeDto Support { get; set; }
        public PageNodeDto UserDashboard { get; set; }

        public string Name { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Warning { get; set; }
    }
}
