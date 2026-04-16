using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillSupportLogin;
using VFL.Renderer.ViewModels.MyBillSupportLogin;
using Microsoft.Extensions.Logging;
using Progress.Sitefinity.AspNetCore.Widgets.ViewComponents.Common;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// View component for MyBill Support Login widget
    /// Handles customer support access using single-use access tokens
    /// </summary>
    [SitefinityWidget(
        Title = "MyBill Support Login", 
        Order = 0, 
        Section = WidgetSection.Other, 
        Category = WidgetCategory.Content, 
        IconName = "support-agent", 
        NotPersonalizable = true)]
    [ViewComponent(Name = "MyBillSupportLogin")]
    public class MyBillSupportLoginViewComponent : ViewComponent
    {
        private readonly ILogger<MyBillSupportLoginViewComponent> _logger;
        private readonly IStyleClassesProvider _styles;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyBillSupportLoginViewComponent"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="styles">The style classes provider.</param>
        public MyBillSupportLoginViewComponent(
            ILogger<MyBillSupportLoginViewComponent> logger,
            IStyleClassesProvider styles)
        {
            _logger = logger;
            _styles = styles;
        }

        /// <summary>
        /// Invokes the MyBill Support Login widget creation.
        /// </summary>
        /// <param name="context">The context containing the entity configuration.</param>
        /// <returns>A view component result.</returns>
        public virtual async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<MyBillSupportLoginEntity> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var entity = context.Entity;
            var viewModel = new MyBillSupportLoginViewModel();

            try
            {
                _logger.LogInformation("MyBill SupportLogin: Initializing widget");

                // Set labels from entity
                viewModel.Labels.Header = entity.Header ?? "Support Access";
                viewModel.Labels.LoadingMessage = entity.LoadingMessage ?? "Authenticating, please wait...";
                viewModel.Labels.SuccessMessage = entity.SuccessMessage ?? "Login successful! Redirecting to dashboard...";
                viewModel.Labels.InvalidTokenMessage = entity.InvalidTokenMessage ?? "Invalid or expired access token. Please request a new token.";
                viewModel.Labels.MissingTokenMessage = entity.MissingTokenMessage ?? "No access token provided. Please use a valid support access link.";

                viewModel.Attributes = entity.Attributes;
                viewModel.VisibilityClasses = _styles.StylingConfig.VisibilityClasses;

                // Default redirect to MyBill dashboard
                // For now, we use a hardcoded path. If PostLoginRedirectPage configuration is needed,
                // it can be implemented via a model class pattern like other widgets
                viewModel.RedirectUrl = "/mybill-dashboard";
                _logger.LogInformation("MyBill SupportLogin: Using redirect URL /mybill-dashboard");

                var margins = _styles.GetMarginsClasses(entity);
                viewModel.CssClass = (entity.CssClass + " " + margins.Trim()).Trim();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "MyBill SupportLogin: HTTP error initializing view model");
                viewModel.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill SupportLogin: Error initializing view model");
                viewModel.ErrorMessage = ex.Message;
            }

            return await Task.FromResult(View(entity.SfViewName, viewModel));
        }
    }
}
