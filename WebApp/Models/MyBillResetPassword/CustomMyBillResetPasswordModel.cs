using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Progress.Sitefinity.AspNetCore.Configuration;
using Progress.Sitefinity.AspNetCore.Web;
using Progress.Sitefinity.AspNetCore.Widgets.ViewComponents.Common;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using Progress.Sitefinity.RestSdk.OData;
using Progress.Sitefinity.RestSdk;
using Progress.Sitefinity.AspNetCore.RestSdk;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System;
using VFL.Renderer.ViewModels.MyBillResetPassword;
using VFL.Renderer.Entities.MyBillResetPassword;

namespace VFL.Renderer.Models.MyBillResetPassword
{
    public class CustomMyBillResetPasswordModel : ICustomMyBillResetPasswordModel
    {
        private const string PasswordRecoveryQueryStringKey = "vk";

        private readonly ISitefinityConfig config;
        private readonly IODataRestClient restService;
        private readonly IStyleClassesProvider styles;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IRenderContext renderContext;
        private readonly IStringLocalizer<CustomMyBillResetPasswordModel> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMyBillResetPasswordModel"/> class.
        /// </summary>
        public CustomMyBillResetPasswordModel(
            ISitefinityConfig config,
            IRenderContext renderContext,
            IODataRestClient restService,
            IHttpContextAccessor httpContextAccessor,
            IStyleClassesProvider styles,
            IStringLocalizer<CustomMyBillResetPasswordModel> localizer)
        {
            this.config = config;
            this.renderContext = renderContext;
            this.restService = restService;
            this.httpContextAccessor = httpContextAccessor;
            this.styles = styles;
            this.localizer = localizer;
        }

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intended. Security reasons.")]
        public virtual async Task<CustomMyBillResetPasswordViewModel> InitializeViewModel(CustomMyBillResetPasswordEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var viewModel = new CustomMyBillResetPasswordViewModel();
            viewModel.ResetUserPasswordHandlerPath = "/api/MyBillResetPassword/ResetUserPassword";
            viewModel.Attributes = entity.Attributes;
            viewModel.Labels.ResetPasswordHeader = entity.ResetPasswordHeader;
            viewModel.Labels.NewPasswordLabel = entity.NewPasswordLabel;
            viewModel.Labels.RepeatNewPasswordLabel = entity.RepeatNewPasswordLabel;
            viewModel.Labels.SaveButtonLabel = entity.SaveButtonLabel;
            viewModel.Labels.BackLinkLabel = entity.BackLinkLabel;
            viewModel.Labels.SuccessMessage = entity.SuccessMessage;
            viewModel.Labels.ErrorMessage = entity.ErrorMessage;
            viewModel.Labels.AllFieldsAreRequiredErrorMessage = entity.AllFieldsAreRequiredErrorMessage;
            viewModel.Labels.PasswordsMismatchErrorMessage = entity.PasswordsMismatchErrorMessage;

            viewModel.SendResetPasswordEmailHandlerPath = "/api/MyBillResetPassword/SendResetPasswordEmail";
            viewModel.Labels.ForgottenPasswordHeader = entity.ForgottenPasswordHeader;
            viewModel.Labels.EmailLabel = entity.BanLabel;
            viewModel.Labels.ForgottenPasswordLinkMessage = entity.ForgottenPasswordLinkMessage;
            viewModel.Labels.ForgottenPasswordSubmitMessage = entity.ForgottenPasswordSubmitMessage;
            viewModel.Labels.SendButtonLabel = entity.SendButtonLabel;
            viewModel.Labels.BackLinkLabel = entity.BackLinkLabel;
            viewModel.Labels.ForgottenPasswordLabel = entity.ForgottenPasswordLabel;
            viewModel.Labels.InvalidEmailFormatMessage = entity.InvalidBanFormatMessage;
            viewModel.Labels.FieldIsRequiredMessage = entity.FieldIsRequiredMessage;
            viewModel.VisibilityClasses = this.styles.StylingConfig.VisibilityClasses;
            viewModel.InvalidClass = this.styles.StylingConfig.InvalidClass;

            if (entity.LoginPage?.Content?[0]?.Variations?.Length != 0)
            {
                var pageNodes = await this.restService.GetItems<PageNodeDto>(entity.LoginPage, new GetAllArgs() { Fields = new[] { nameof(PageNodeDto.ViewUrl) } });
                var items = pageNodes.Items;
                if (items.Count == 1)
                    viewModel.LoginPageLink = items[0].ViewUrl;
            }

            this.httpContextAccessor.HttpContext.AddVaryByQueryParams(PasswordRecoveryQueryStringKey);
            if (this.IsResetPasswordRequest())
            {
                viewModel.IsResetPasswordRequest = true;
                this.httpContextAccessor.HttpContext.DisableCache();

                try
                {
                    // For MyBill, we don't need to validate security questions
                    // Just check if token and user are present in query string
                    var queryString = this.httpContextAccessor.HttpContext.Request.Query;
                    if (queryString.ContainsKey("user") && queryString.ContainsKey("token"))
                    {
                        viewModel.BAN = queryString["user"];
                        viewModel.Token = queryString["token"];
                    }
                    else
                    {
                        viewModel.Error = true;
                    }
                }
                catch (Exception)
                {
                    // In terms of security, if there is some error, we display common error message to the user.
                    viewModel.Error = true;
                }
            }
            else
            {
                if (this.renderContext.IsLive)
                {
                    var request = this.httpContextAccessor.HttpContext.Request;
                    viewModel.ResetPasswordUrl = $"{request.Scheme}://{request.Host}{request.Path}";
                }
            }

            var margins = this.styles.GetMarginsClasses(entity);
            viewModel.CssClass = (entity.CssClass + " " + margins.Trim()).Trim();

            return viewModel;
        }

        private bool IsResetPasswordRequest()
        {
            var query = this.httpContextAccessor.HttpContext.Request.Query;
            return this.renderContext.IsLive && (query.ContainsKey("user") && query.ContainsKey("token"));
        }
    }
}
