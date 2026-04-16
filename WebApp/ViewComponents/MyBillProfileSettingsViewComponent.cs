using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Config;
using VFL.Renderer.Entities.MyBillProfileSettings;
using VFL.Renderer.Models.MyBillProfileSettings;
using VFL.Renderer.ViewModels.MyBillProfileSettings;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// View component for the MyBill Profile Settings widget
    /// Registers as a Sitefinity widget in the CMS toolbox
    /// </summary>
    [SitefinityWidget(
        Title = "MyBill Profile Settings", 
        Order = 2, 
        Category = WidgetCategory.Content, 
        IconName = "user-settings", 
        NotPersonalizable = true)]
    [ViewComponent(Name = "MyBillProfileSettings")]
    public class MyBillProfileSettingsViewComponent : ViewComponent
    {
        private readonly IMyBillProfileSettingsModel _model;
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<MyBillProfileSettingsViewComponent> _logger;

        public MyBillProfileSettingsViewComponent(
            IMyBillProfileSettingsModel model, 
            IOptions<ApiSettings> apiSettings,
            ILogger<MyBillProfileSettingsViewComponent> logger)
        {
            _model = model;
            _apiSettings = apiSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the view component
        /// Called when the widget is rendered on a Sitefinity page
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<MyBillProfileSettingsEntity> context)
        {
            // Check if we're in Sitefinity designer/preview mode
            var isDesignerMode = HttpContext.Request.Path.Value?.Contains("/Sitefinity/", System.StringComparison.OrdinalIgnoreCase) ?? false;
            var isPreviewMode = HttpContext.Request.Query.ContainsKey("sfaction") || 
                               HttpContext.Request.Headers.ContainsKey("X-SF-RENDERER");

            // Use async authentication check — falls back to cookie-based auth
            // when Sitefinity's rendering pipeline overwrites HttpContext.User
            var isMyBillAuthenticated = await MyBillAuthHelper.IsMyBillAuthenticatedAsync(HttpContext);

            _logger.LogInformation("MyBill Profile Settings: User.Identity.IsAuthenticated = {IsAuth}", 
                HttpContext.User?.Identity?.IsAuthenticated);
            _logger.LogInformation("MyBill Profile Settings: User.Identity.AuthenticationType = {AuthType}", 
                HttpContext.User?.Identity?.AuthenticationType);
            _logger.LogInformation("MyBill Profile Settings: IsMyBillAuthenticated (via helper) = {IsMyBillAuth}", 
                isMyBillAuthenticated);

            // Get view model
            var viewModel = await _model.GetViewModels(context.Entity);
            viewModel.IsAuthenticated = isMyBillAuthenticated;
            viewModel.IsDesignerMode = isDesignerMode || isPreviewMode;

            // Pass config to views for client-side JS
            ViewBag.MyBillJwtExpiryMinutes = _apiSettings.MyBillJwtExpiryMinutes;
            ViewBag.MyBillLoginPath = _apiSettings.MyBillLoginPath ?? "/my-bill-login";

            // If not authenticated and not in designer mode, show unauthorized view
            if (!isMyBillAuthenticated && !isDesignerMode && !isPreviewMode)
            {
                _logger.LogWarning("MyBill Profile Settings: User not authenticated, showing login prompt");
                
                var loginPath = _apiSettings.MyBillLoginPath ?? "/my-bill-login";
                var currentUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                var redirectUrl = $"{loginPath}?returnUrl={System.Net.WebUtility.UrlEncode(currentUrl)}";
                
                viewModel.RedirectUrl = redirectUrl;
                return View("Unauthorized", viewModel);
            }

            _logger.LogInformation("MyBill Profile Settings: Rendering profile settings view for authenticated user");
            return View(context.Entity.ViewName, viewModel);
        }
    }
}
