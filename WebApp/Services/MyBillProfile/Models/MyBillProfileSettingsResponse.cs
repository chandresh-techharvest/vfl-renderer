namespace VFL.Renderer.Services.MyBillProfile.Models
{
    /// <summary>
    /// Response model for MyBill user profile information
    /// </summary>
    public class MyBillProfileSettingsResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string ProfileImageData { get; set; }
        public VFL.Renderer.ViewModels.MyBillDashboard.BanAccount[] BanAccounts { get; set; }
        
        /// <summary>
        /// Primary account name (company name) from GraphQL API
        /// </summary>
        public string PrimaryAccountName { get; set; }
        
        /// <summary>
        /// Contact person full name
        /// </summary>
        public string ContactFullName { get; set; }
        
        /// <summary>
        /// Contact phone number
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
