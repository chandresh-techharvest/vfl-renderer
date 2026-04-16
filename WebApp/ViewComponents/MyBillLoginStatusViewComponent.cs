using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Config;
using VFL.Renderer.Entities.MyBillLoginStatus;
using VFL.Renderer.Models.MyBillLoginStatus;
using VFL.Renderer.Services.MyBillProfile;
using VFL.Renderer.Services.MyBillProfile.Models;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// View component for MyBill Login Status widget
    /// Shows login/logout for MyBill authenticated users
    /// </summary>
    [SitefinityWidget(
        Title = "MyBill Login Status",
        Category = WidgetCategory.Content,
        Section = "MyBill")]
    public class MyBillLoginStatusViewComponent : ViewComponent
    {
        private readonly IMyBillLoginStatusModel model;
        private readonly IMyBillProfileService _myBillProfileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiSettings _apiSettings;

        public MyBillLoginStatusViewComponent(
            IMyBillLoginStatusModel model, 
            IMyBillProfileService myBillProfileService, 
            IHttpContextAccessor httpContextAccessor,
            IOptions<ApiSettings> apiSettings)
        {
            this.model = model;
            _myBillProfileService = myBillProfileService;
            _httpContextAccessor = httpContextAccessor;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<MyBillLoginStatusEntity> context)
        {
            var viewModel = await this.model.GetViewModels(context.Entity);
            
            // Pass configuration to view via ViewBag
            ViewBag.MyBillLogoutUrl = $"/api/MyBillLoginForm/Logout";
            ViewBag.MyBillJwtExpiryMinutes = _apiSettings.MyBillJwtExpiryMinutes;
            ViewBag.MyBillLoginPath = _apiSettings.MyBillLoginPath ?? "/my-bill-login";
            
            // Set logout redirect URL: prefer Sitefinity-configured page, fallback to appsettings
            if (string.IsNullOrEmpty(viewModel.LogoutRedirectUrl))
            {
                viewModel.LogoutRedirectUrl = _apiSettings.MyBillLoginPath ?? "/my-bill-login";
            }

            // Use async authentication check — falls back to cookie-based auth
            // when Sitefinity's rendering pipeline overwrites HttpContext.User
            bool isAuthenticated = await MyBillAuthHelper.IsMyBillAuthenticatedAsync(HttpContext);
            
            viewModel.IsAuthenticated = isAuthenticated;

            if (isAuthenticated)
            {
                // Get username from claims
                string username = MyBillAuthHelper.GetUsername(HttpContext);

                try
                {
                    // Try to get profile information from MyBill backend to get primary account name
                    var profileViewModel = await _myBillProfileService.GetAllAccountsByPrimaryAsync<GraphQLAccountsResponse>();

                    if (profileViewModel?.Data?.AllAccountsByPrimary != null && profileViewModel.Data.AllAccountsByPrimary.Length > 0)
                    {
                        // Get primary account info (first item in the array)
                        var primaryAccount = profileViewModel.Data.AllAccountsByPrimary[0];
                        
                        // Use PrimaryAccountName for display (company name)
                        viewModel.CompanyName = primaryAccount.PrimaryAccountName ?? username;
                        viewModel.ProfileImageUrl = null; // Profile image not available in new API
                    }
                    else
                    {
                        viewModel.CompanyName = username;
                    }
                }
                catch (System.Exception)
                {
                    // If profile fetch fails, use username from claims
                    viewModel.CompanyName = username ?? "Corporate User";
                }
            }

            return View(viewModel);
        }
    }
}
