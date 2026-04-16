using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillProfileSettings;
using VFL.Renderer.Services.MyBillProfile;
using VFL.Renderer.Services.MyBillProfile.Models;
using VFL.Renderer.ViewModels.MyBillProfileSettings;

namespace VFL.Renderer.Models.MyBillProfileSettings
{
    /// <summary>
    /// Model for MyBill Profile Settings widget
    /// Retrieves user profile data from MyBill backend
    /// </summary>
    public class MyBillProfileSettingsModel : IMyBillProfileSettingsModel
    {
        private readonly IMyBillProfileService _myBillProfileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyBillProfileSettingsModel(
            IMyBillProfileService myBillProfileService,
            IHttpContextAccessor httpContextAccessor)
        {
            _myBillProfileService = myBillProfileService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the view models with preview data for Sitefinity designer mode
        /// </summary>
        public async Task<MyBillProfileSettingsViewModel> GetViewModels(MyBillProfileSettingsEntity entity)
        {
            try
            {
                var viewModel = new MyBillProfileSettingsViewModel();

                // Try to get user profile from MyBill backend
                try
                {
                    var profileInfo = await _myBillProfileService.GetAllAccountsByPrimaryAsync<GraphQLAccountsResponse>();
                    if (profileInfo?.Data?.AllAccountsByPrimary != null && profileInfo.Data.AllAccountsByPrimary.Length > 0)
                    {
                        var primaryAccount = profileInfo.Data.AllAccountsByPrimary[0];
                        
                        // Get selected BAN from cookie, fallback to first BAN
                        string selectedBan = GetSelectedBanFromCookie();
                        
                        // Find the selected BAN or use first as default
                        var selectedBanAccount = primaryAccount.BusinessAccountNumbers?
                            .FirstOrDefault(b => b.Number == selectedBan);
                        
                        if (selectedBanAccount == null && primaryAccount.BusinessAccountNumbers?.Length > 0)
                        {
                            selectedBanAccount = primaryAccount.BusinessAccountNumbers[0];
                        }
                        
                        if (selectedBanAccount != null)
                        {
                            viewModel.SelectedBan = selectedBanAccount.Number;
                            viewModel.AccountName = selectedBanAccount.AccountName ?? primaryAccount.PrimaryAccountName;
                        }
                        
                        // Primary account contact info (shared across all BANs)
                        viewModel.ContactFullName = primaryAccount.ContactFullName;
                        viewModel.ContactPhoneNumber = primaryAccount.PhoneNumber;
                        viewModel.Email = primaryAccount.Email;
                    }
                }
                catch (Exception)
                {
                    // User not authenticated or API call failed - use preview data for Sitefinity designer
                    viewModel = GetPreviewData();
                }

                // If no data loaded, use preview data for design mode
                if (string.IsNullOrEmpty(viewModel.ContactFullName))
                {
                    viewModel = GetPreviewData();
                }

                return viewModel;
            }
            catch (Exception)
            {
                // Return preview data on any error
                return GetPreviewData();
            }
        }

        /// <summary>
        /// Get selected BAN from mybillData cookie
        /// </summary>
        private string GetSelectedBanFromCookie()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null) return null;

                var cookieValue = httpContext.Request.Cookies["mybillData"];
                if (string.IsNullOrEmpty(cookieValue)) return null;

                var data = JsonConvert.DeserializeObject<MyBillProfileSettingsResponse>(cookieValue);
                var selectedBan = data?.BanAccounts?.FirstOrDefault(b => b.IsSelected);
                return selectedBan?.Number;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns preview data for Sitefinity designer mode
        /// </summary>
        private MyBillProfileSettingsViewModel GetPreviewData()
        {
            return new MyBillProfileSettingsViewModel
            {
                SelectedBan = "945939845",
                AccountName = "Main Corporate Account",
                ContactFullName = "John Doe",
                ContactPhoneNumber = "+679 1234567",
                Email = "john.doe@example.com"
            };
        }
    }
}
