using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using Progress.Sitefinity.AspNetCore.Web;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using Progress.Sitefinity.RestSdk.OData;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Config;
using VFL.Renderer.Entities.MyBillDashboard;
using VFL.Renderer.Models.MyBillDashboard;
using VFL.Renderer.ViewModels.MyBillDashboard;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// View component for the MyBill Dashboard widget
    /// Registers as a Sitefinity widget in the CMS toolbox
    /// </summary>
    [SitefinityWidget(
        Title = "MyBill Dashboard", 
        Order = 1, 
        Category = WidgetCategory.Content, 
        IconName = "content-section", 
        NotPersonalizable = true)]
    [ViewComponent(Name = "MyBillDashboard")]
    public class MyBillDashboardViewComponent : ViewComponent
    {
        private readonly IMyBillDashboardModel _model;
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<MyBillDashboardViewComponent> _logger;
        private readonly IODataRestClient _restService;
        private readonly IRenderContext _renderContext;

        public MyBillDashboardViewComponent(
            IMyBillDashboardModel model, 
            IOptions<ApiSettings> apiSettings,
            ILogger<MyBillDashboardViewComponent> logger,
            IODataRestClient restService,
            IRenderContext renderContext)
        {
            _model = model;
            _apiSettings = apiSettings.Value;
            _logger = logger;
            _restService = restService;
            _renderContext = renderContext;
        }

        /// <summary>
        /// Invokes the view component
        /// Called when the widget is rendered on a Sitefinity page
        /// </summary>
        /// <param name="context">The view component context from Sitefinity</param>
        /// <returns>The view component result with populated view model</returns>
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<MyBillDashboardEntity> context)
        {
            // Check if we're in Sitefinity designer/preview mode
            var isDesignerMode = HttpContext.Request.Path.Value?.Contains("/Sitefinity/", System.StringComparison.OrdinalIgnoreCase) ?? false;
            var isPreviewMode = HttpContext.Request.Query.ContainsKey("sfaction") || 
                               HttpContext.Request.Headers.ContainsKey("X-SF-RENDERER");

            // Use async authentication check — falls back to cookie-based auth
            // when Sitefinity's rendering pipeline overwrites HttpContext.User
            var isMyBillAuthenticated = await MyBillAuthHelper.IsMyBillAuthenticatedAsync(HttpContext);

            // Always get view model (includes preview data for designer)
            var viewModel = await _model.GetViewModels(context.Entity);
            viewModel.IsAuthenticated = isMyBillAuthenticated;
            viewModel.IsDesignerMode = isDesignerMode || isPreviewMode;

            // Extract Checkout page URL
            if (context.Entity.CheckoutPage.HasSelectedItems())
            {
                var pageNode = await _restService.GetItem<PageNodeDto>(context.Entity.CheckoutPage);
                if (pageNode != null)
                {
                    viewModel.CheckoutPageUrl = "/" + pageNode.UrlName.TrimStart('/');
                }
            }

            // Pass config to views for client-side JS
            ViewBag.MyBillJwtExpiryMinutes = _apiSettings.MyBillJwtExpiryMinutes;
            ViewBag.MyBillLoginPath = _apiSettings.MyBillLoginPath ?? "/my-bill-login";

            // If not authenticated and not in designer mode, show unauthorized view
            if (!isMyBillAuthenticated && !isDesignerMode && !isPreviewMode)
            {
                var loginPath = _apiSettings.MyBillLoginPath ?? "/my-bill-login";
                var currentUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
                var redirectUrl = $"{loginPath}?returnUrl={System.Net.WebUtility.UrlEncode(currentUrl)}";
                
                viewModel.RedirectUrl = redirectUrl;
                return View("Unauthorized", viewModel);
            }

            return View(context.Entity.ViewName, viewModel);
        }
    }
}

