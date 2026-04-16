using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using Progress.Sitefinity.AspNetCore.Widgets.Models.ResetPassword;
using System.Threading.Tasks;
using System;
using VFL.Renderer.Models.ResetPassword;
using VFL.Renderer.Entities.ResetPassword;

namespace VFL.Renderer.ViewComponents
{
    [SitefinityWidget(Title = "Custom Reset password", Order = 0, Section = WidgetSection.Other, Category = WidgetCategory.Content, IconName = "password", NotPersonalizable = true)]
    [ViewComponent(Name = "CustomResetPassword")]
    public class CustomResetPasswordViewComponent : ViewComponent
    {

        private ICustomResetPasswordModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordViewComponent"/> class.
        /// </summary>
        /// <param name="resetPasswordModel">The reset password model.</param>
        public CustomResetPasswordViewComponent(ICustomResetPasswordModel resetPasswordModel)
        {
            this.model = resetPasswordModel;
        }

        /// <summary>
        /// Invokes the login form widget creation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<CustomResetPasswordEntity> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var viewModel = await this.model.InitializeViewModel(context.Entity);

            if (!string.IsNullOrEmpty(viewModel.Warning))
            {
                context.SetWarning(viewModel.Warning);
            }

            if (!string.IsNullOrEmpty(HttpContext.Request.Query["user"]))
            {
                string user = HttpContext.Request.Query["user"];
                viewModel.UserName = user;
            }
            if (!string.IsNullOrEmpty(HttpContext.Request.Query["token"]))
            {
                string token = HttpContext.Request.Query["token"];
                viewModel.token = token;
                viewModel.IsResetPasswordRequest = true;
            }

            return this.View(context.Entity.SfViewName, viewModel);
        }
    }
}
