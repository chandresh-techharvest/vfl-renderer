using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;

namespace VFL.Renderer.ViewModels.MyBillLoginStatus
{
    /// <summary>
    /// View model for MyBill Login Status widget
    /// </summary>
    public class MyBillLoginStatusViewModel
    {
        public PageNodeDto LoginPage { get; set; }
        public PageNodeDto ProfilePage { get; set; }
        public PageNodeDto MyBillDashboard { get; set; }
        public PageNodeDto TransactionHistory { get; set; }
        
        public string CompanyName { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsAuthenticated { get; set; }
        public string LogoutRedirectUrl { get; set; }
    }
}
