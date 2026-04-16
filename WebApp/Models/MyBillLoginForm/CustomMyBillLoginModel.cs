using Microsoft.AspNetCore.Http;
using Progress.Sitefinity.AspNetCore.Widgets.Models.Common;
using Progress.Sitefinity.AspNetCore.Widgets.Models.LoginForm;
using Progress.Sitefinity.AspNetCore.Widgets.ViewComponents.Common;
using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using Progress.Sitefinity.RestSdk;
using Progress.Sitefinity.RestSdk.OData;
using System.Linq;
using System.Threading.Tasks;
using System;
using VFL.Renderer.Entities.MyBillLoginForm;
using VFL.Renderer.ViewModels.MyBillLoginForm;

namespace VFL.Renderer.Models.MyBillLoginForm
{
    /// <summary>
    /// Model for MyBill Login form widget
    /// Handles initialization and setup of MyBill-specific login form
    /// </summary>
    public class CustomMyBillLoginModel : ICustomMyBillLoginModel
    {
        private readonly IODataRestClient restService;
        private readonly IStyleClassesProvider styles;
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMyBillLoginModel"/> class.
        /// </summary>
        /// <param name="restService">The client for Sitefinity web services.</param>
        /// <param name="styles">The html classes for styling provider.</param>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        public CustomMyBillLoginModel(
            IODataRestClient restService,
            IStyleClassesProvider styles,
            IHttpContextAccessor httpContextAccessor)
        {
            this.restService = restService;
            this.styles = styles;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Initializes the MyBill login form view model
        /// </summary>
        /// <param name="entity">The MyBill login form entity from Sitefinity</param>
        /// <returns>Populated MyBill login form view model</returns>
        public virtual async Task<MyBillLoginFormViewModel> InitializeViewModel(MyBillLoginFormEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var viewModel = new MyBillLoginFormViewModel
            {
                LoginHandlerPath = "/api/MyBillLoginForm/Login",  // CRITICAL: Must have leading slash
                RememberMe = entity.RememberMe,
                Attributes = entity.Attributes,
            };

            // Set MyBill-specific labels
            viewModel.Labels.BanLabel = entity.BanLabel;
            viewModel.Labels.ErrorMessage = entity.ErrorMessage;
            viewModel.Labels.ForgottenPasswordLinkLabel = entity.ForgottenPasswordLinkLabel;
            viewModel.Labels.Header = entity.Header;
            viewModel.Labels.PasswordLabel = entity.PasswordLabel;
            viewModel.Labels.RememberMeLabel = entity.RememberMeLabel;
            viewModel.Labels.SubmitButtonLabel = entity.SubmitButtonLabel;
            viewModel.Labels.ValidationInvalidBanMessage = entity.ValidationInvalidBanMessage;
            viewModel.Labels.ValidationRequiredMessage = entity.ValidationRequiredMessage;
            
            viewModel.VisibilityClasses = this.styles.StylingConfig.VisibilityClasses;
            viewModel.InvalidClass = this.styles.StylingConfig.InvalidClass;

            // Handle post-login redirect page
            if (entity.PostLoginAction == PostLoginAction.RedirectToPage && 
                entity.PostLoginRedirectPage?.Content != null && 
                entity.PostLoginRedirectPage.Content.Length > 0 &&
                entity.PostLoginRedirectPage.Content[0]?.Variations != null &&
                entity.PostLoginRedirectPage.Content[0].Variations.Length > 0)
            {
                var pageNodes = await this.restService.GetItems<PageNodeDto>(entity.PostLoginRedirectPage, new GetAllArgs() { Fields = new[] { nameof(PageNodeDto.ViewUrl) } });
                var items = pageNodes.Items;
                if (items.Count == 1)
                    viewModel.RedirectUrl = items[0].ViewUrl;
            }

            // Handle reset password page
            if (entity.ResetPasswordPage?.Content != null && 
                entity.ResetPasswordPage.Content.Length > 0 &&
                entity.ResetPasswordPage.Content[0]?.Variations != null &&
                entity.ResetPasswordPage.Content[0].Variations.Length > 0)
            {
                var pageNodes = await this.restService.GetItems<PageNodeDto>(entity.ResetPasswordPage, new GetAllArgs() { Fields = new[] { nameof(PageNodeDto.ViewUrl) } });
                var items = pageNodes.Items;
                if (items.Count == 1)
                    viewModel.ForgottenPasswordLink = items[0].ViewUrl;
            }

            var margins = this.styles.GetMarginsClasses(entity);
            viewModel.CssClass = (entity.CssClass + " " + margins.Trim()).Trim();
            
            return viewModel;
        }
    }
}


