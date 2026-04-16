using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using Progress.Sitefinity.AspNetCore.Web;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using Progress.Sitefinity.RestSdk.OData;
using System;
using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillCheckout;
using VFL.Renderer.Models.MyBillCheckout;
using VFL.Renderer.ViewModels.MyBillCheckout;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// View component for MyBill Checkout widget
    /// Displays payment checkout form for corporate bill payments
    /// </summary>
    [SitefinityWidget(
        Title = "MyBill Checkout", 
        Category = WidgetCategory.Content,
        Section = "MyBill",
        EmptyIconText = "MyBill Checkout",
        EmptyIconAction = EmptyLinkAction.None)]
    public class MyBillCheckoutViewComponent : ViewComponent
    {
        private readonly IRenderContext _renderContext;
        private readonly IODataRestClient _restService;
        private readonly ICheckoutSessionStore _sessionStore;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyBillCheckoutViewComponent(
            IRenderContext renderContext, 
            IODataRestClient restService,
            ICheckoutSessionStore sessionStore,
            IHttpContextAccessor httpContextAccessor)
        {
            _renderContext = renderContext;
            _restService = restService;
            _sessionStore = sessionStore;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<MyBillCheckoutEntity> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Initialize view model with configuration
            var viewModel = new MyBillCheckoutViewModel
            {
                PageTitle = context.Entity.PageTitle ?? "My Corporate",
                SectionTitle = context.Entity.SectionTitle ?? "Bill Payment",
                BackButtonText = context.Entity.BackButtonText ?? "Back"
            };

            // In edit mode (Sitefinity backend), show a design-time preview
            // without processing checkout sessions or resolving page links
            if (_renderContext.IsEdit)
            {
                viewModel.TermsConditionsUrl = context.Entity.TermsConditionsUrl ?? "/terms-and-conditions";
                viewModel.CustomerSupportUrl = context.Entity.CustomerSupportUrl ?? "/contact-us";
                viewModel.DashboardUrl = context.Entity.DashboardUrl ?? "/mybill/mybill-dashboard";
                viewModel.BackButtonUrl = context.Entity.BackButtonUrl ?? "/mybill/mybill-dashboard";
                viewModel.SessionExpired = false;
                viewModel.SessionId = string.Empty;
                viewModel.Email = "preview@example.com";
                viewModel.InvoiceNumber = "INV-PREVIEW";
                viewModel.BanNumber = "BAN-PREVIEW";
                viewModel.TotalAmount = 100.00m;
                viewModel.PaymentAmount = 100.00m;
                viewModel.IsPartialPayment = false;
                return View(context.Entity.ViewName, viewModel);
            }

            // Extract Terms and Conditions URL (Page selector takes precedence over URL)
            if (context.Entity.TermsConditionsPage != null && context.Entity.TermsConditionsPage.HasSelectedItems())
            {
                var termsPage = await _restService.GetItem<PageNodeDto>(context.Entity.TermsConditionsPage);
                if (termsPage != null)
                {
                    viewModel.TermsConditionsUrl = "/" + termsPage.UrlName.TrimStart('/');
                }
            }
            else
            {
                viewModel.TermsConditionsUrl = context.Entity.TermsConditionsUrl ?? "/terms-and-conditions";
            }

            // Extract Payment Result page URL
            if (context.Entity.PaymentResultPage != null && context.Entity.PaymentResultPage.HasSelectedItems())
            {
                var pageNode = await _restService.GetItem<PageNodeDto>(context.Entity.PaymentResultPage);
                if (pageNode != null)
                {
                    viewModel.PaymentResultPageUrl = "/" + pageNode.UrlName.TrimStart('/');
                }
            }

            // Extract Customer Support URL (Page selector takes precedence over URL)
            if (context.Entity.CustomerSupportPage != null && context.Entity.CustomerSupportPage.HasSelectedItems())
            {
                var supportPage = await _restService.GetItem<PageNodeDto>(context.Entity.CustomerSupportPage);
                if (supportPage != null)
                {
                    viewModel.CustomerSupportUrl = "/" + supportPage.UrlName.TrimStart('/');
                }
            }
            else
            {
                viewModel.CustomerSupportUrl = context.Entity.CustomerSupportUrl ?? "/contact-us";
            }

            // Extract Dashboard URL (Page selector takes precedence over URL)
            if (context.Entity.DashboardPage != null && context.Entity.DashboardPage.HasSelectedItems())
            {
                var dashboardPage = await _restService.GetItem<PageNodeDto>(context.Entity.DashboardPage);
                if (dashboardPage != null)
                {
                    viewModel.DashboardUrl = "/" + dashboardPage.UrlName.TrimStart('/');
                }
            }
            else
            {
                viewModel.DashboardUrl = context.Entity.DashboardUrl ?? "/mybill/mybill-dashboard";
            }

            // Extract Back Button URL (Page selector takes precedence over URL)
            if (context.Entity.BackButtonPage != null && context.Entity.BackButtonPage.HasSelectedItems())
            {
                var backPage = await _restService.GetItem<PageNodeDto>(context.Entity.BackButtonPage);
                if (backPage != null)
                {
                    viewModel.BackButtonUrl = "/" + backPage.UrlName.TrimStart('/');
                }
            }
            else
            {
                viewModel.BackButtonUrl = context.Entity.BackButtonUrl ?? "/mybill/mybill-dashboard";
            }

            // Get parameters from query string
            var request = _httpContextAccessor.HttpContext.Request;
            var sessionId = request.Query["sid"].ToString();
            var resultParam = request.Query["result"].ToString();
            
            // Check if this is showing a payment result (success or fail)
            if (!string.IsNullOrEmpty(resultParam))
            {
                // Render Success or Fail view based on result parameter
                var isSuccess = resultParam.Equals("success", StringComparison.OrdinalIgnoreCase);
                return View(isSuccess ? "Success" : "Fail", viewModel);
            }
            
            // Check if this is a payment callback (has payment gateway parameters but no sid)
            // Payment gateways add their own parameters when redirecting back
            var hasPaymentCallback = 
                (request.Query.ContainsKey("sessionId") ||  // Windcave/CARD callback
                 request.Query.ContainsKey("rID") ||        // M-PAISA callback
                 request.Query.ContainsKey("tID") ||        // M-PAISA callback
                 request.Query.ContainsKey("token")) &&     // M-PAISA callback
                string.IsNullOrEmpty(sessionId);            // No checkout session
            
            // Check for payment context cookie (set before redirecting to payment gateway)
            var hasPaymentContextCookie = request.Cookies.ContainsKey("mybill_payment_context");

            if (!string.IsNullOrEmpty(sessionId))
            {
                // Load session data for new checkout
                var session = _sessionStore.Get(sessionId) as MyBillCheckoutSession;
                if (session != null)
                {
                    viewModel.SessionId = sessionId;
                    viewModel.Email = session.Email;
                    viewModel.InvoiceNumber = session.InvoiceNumber;
                    viewModel.BanNumber = session.BanNumber;
                    viewModel.TotalAmount = session.FullAmount;
                    viewModel.PaymentAmount = session.PaymentAmount;
                    viewModel.IsPartialPayment = session.IsPartialPayment;
                }
                else
                {
                    // Session expired or invalid
                    viewModel.SessionExpired = true;
                }
            }
            else if (hasPaymentCallback || hasPaymentContextCookie)
            {
                // This is a payment callback - don't show session expired
                // JavaScript will handle the callback and show success/fail view
                // Payment context (invoice/BAN) will be retrieved from encrypted cookie via API
                viewModel.SessionExpired = false;
                // Set minimal data to avoid null reference errors in view
                viewModel.SessionId = string.Empty;
                viewModel.Email = string.Empty;
                viewModel.InvoiceNumber = string.Empty;
                viewModel.BanNumber = string.Empty;
                viewModel.TotalAmount = 0;
                viewModel.PaymentAmount = 0;
                viewModel.IsPartialPayment = false;
            }
            else
            {
                // No session ID and no payment callback parameters
                viewModel.SessionExpired = true;
            }

            return View(context.Entity.ViewName, viewModel);
        }
    }
}
