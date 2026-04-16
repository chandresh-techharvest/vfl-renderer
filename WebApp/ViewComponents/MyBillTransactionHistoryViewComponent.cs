using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Config;
using VFL.Renderer.Entities.MyBillTransactionHistory;
using VFL.Renderer.Models.MyBillTransactionHistory;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// View component for the MyBill Transaction History widget
    /// Registers as a Sitefinity widget in the CMS toolbox
    /// </summary>
    [SitefinityWidget(
        Title = "MyBill Transaction History",
        Order = 2,
        Category = WidgetCategory.Content,
        IconName = "history",
        NotPersonalizable = true)]
    [ViewComponent(Name = "MyBillTransactionHistory")]
    public class MyBillTransactionHistoryViewComponent : ViewComponent
    {
        private readonly IMyBillTransactionHistoryModel _model;
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<MyBillTransactionHistoryViewComponent> _logger;

        public MyBillTransactionHistoryViewComponent(
            IMyBillTransactionHistoryModel model,
            IOptions<ApiSettings> apiSettings,
            ILogger<MyBillTransactionHistoryViewComponent> logger)
        {
            _model = model;
            _apiSettings = apiSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the view component
        /// Called when the widget is rendered on a Sitefinity page
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<MyBillTransactionHistoryEntity> context)
        {
            // Check if we're in Sitefinity designer/preview mode
            var isDesignerMode = HttpContext.Request.Path.Value?.Contains("/Sitefinity/", System.StringComparison.OrdinalIgnoreCase) ?? false;
            var isPreviewMode = HttpContext.Request.Query.ContainsKey("sfaction") ||
                               HttpContext.Request.Headers.ContainsKey("X-SF-RENDERER");

            // DEBUG: Log authentication details
            _logger.LogInformation("===== MyBill Transaction History Auth Debug =====");
            _logger.LogInformation("Request Path = {Path}", HttpContext.Request.Path);
            _logger.LogInformation("User.Identity.IsAuthenticated = {IsAuth}", HttpContext.User?.Identity?.IsAuthenticated);
            _logger.LogInformation("User.Identity.AuthenticationType = {AuthType}", HttpContext.User?.Identity?.AuthenticationType);

            // Check for MyBillAuthCookie
            var hasMyBillCookie = HttpContext.Request.Cookies.ContainsKey("MyBillAuthCookie");
            _logger.LogInformation("MyBillAuthCookie exists = {Exists}", hasMyBillCookie);

            // Use async authentication check — falls back to cookie-based auth
            // when Sitefinity's rendering pipeline overwrites HttpContext.User
            var isMyBillAuthenticated = await MyBillAuthHelper.IsMyBillAuthenticatedAsync(HttpContext);
            _logger.LogInformation("IsMyBillAuthenticated (via helper) = {IsMyBillAuth}", isMyBillAuthenticated);
            _logger.LogInformation("===== End MyBill Transaction History Auth Debug =====");

            // Get view model
            var viewModel = await _model.GetViewModelAsync(context.Entity);
            viewModel.IsAuthenticated = isMyBillAuthenticated;
            viewModel.IsDesignerMode = isDesignerMode || isPreviewMode;

            // Pass config to views for client-side JS
            ViewBag.MyBillJwtExpiryMinutes = _apiSettings.MyBillJwtExpiryMinutes;
            ViewBag.MyBillLoginPath = _apiSettings.MyBillLoginPath ?? "/my-bill-login";

            // If not authenticated and not in designer mode, show unauthorized view
            if (!isMyBillAuthenticated && !isDesignerMode && !isPreviewMode)
            {
                _logger.LogWarning("MyBill Transaction History: User not authenticated, showing login prompt");

                var loginPath = _apiSettings.MyBillLoginPath ?? "/my-bill-login";
                var currentUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                var redirectUrl = $"{loginPath}?returnUrl={System.Net.WebUtility.UrlEncode(currentUrl)}";

                viewModel.RedirectUrl = redirectUrl;
                return View("Unauthorized", viewModel);
            }

            _logger.LogInformation("MyBill Transaction History: Rendering view for authenticated user");
            return View(context.Entity.ViewName, viewModel);
        }
    }
}
