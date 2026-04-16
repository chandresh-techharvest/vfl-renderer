using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Threading.Tasks;
using System;
using VFL.Renderer.Models.MyBillResetPassword;
using VFL.Renderer.Entities.MyBillResetPassword;

namespace VFL.Renderer.ViewComponents
{
    [SitefinityWidget(Title = "MyBill Reset password", Order = 0, Section = WidgetSection.Other, Category = WidgetCategory.Content, IconName = "password", NotPersonalizable = true)]
    [ViewComponent(Name = "CustomMyBillResetPassword")]
    public class CustomMyBillResetPasswordViewComponent : ViewComponent
    {
        private ICustomMyBillResetPasswordModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMyBillResetPasswordViewComponent"/> class.
        /// </summary>
        public CustomMyBillResetPasswordViewComponent(ICustomMyBillResetPasswordModel resetPasswordModel)
        {
            this.model = resetPasswordModel;
        }

        /// <summary>
        /// Invokes the MyBill reset password widget creation.
        /// </summary>
        public virtual async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<CustomMyBillResetPasswordEntity> context)
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
                string username = HttpContext.Request.Query["user"];
                viewModel.BAN = username;
            }
            if (!string.IsNullOrEmpty(HttpContext.Request.Query["token"]))
            {
                string token = HttpContext.Request.Query["token"];
                viewModel.Token = token;
                viewModel.IsResetPasswordRequest = true;
            }

            return this.View(context.Entity.SfViewName, viewModel);
        }
    }
}
