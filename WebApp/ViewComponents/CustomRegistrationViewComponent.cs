using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;

using System.Threading.Tasks;
using System;
using VFL.Renderer.Models.Registration;
using VFL.Renderer.Entities.Registration;
using VFL.Renderer.Services.Registration.Models;
using VFL.Renderer.Services.Registration;
using System.Web;
using System.Linq;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// The view component for the registration widget.
    /// </summary>
    [SitefinityWidget(Title = "Custom Registration form", Order = 1, Category = WidgetCategory.Content, IconName = "registration", NotPersonalizable = true)]
    [ViewComponent(Name = "CustomRegistration")]
    public class CustomRegistrationViewComponent : ViewComponent
    {
        private IRegistrationModel model;
        private readonly IRegistrationService _registrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationViewComponent"/> class.
        /// </summary>
        /// <param name="registrationModel">The registration model.</param>
        public CustomRegistrationViewComponent(IRegistrationModel registrationModel, IRegistrationService registrationService)
        {
            this.model = registrationModel;
            _registrationService = registrationService;
        }

        /// <summary>
        /// Invokes the registration widget creation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<RegistrationEntity> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!string.IsNullOrEmpty(HttpContext.Request.Query["user"]) && !string.IsNullOrEmpty(HttpContext.Request.Query["token"]))
            {
                var rawPairs = HttpContext.Request.QueryString.Value.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries).Select(part => { var eqIndex = part.IndexOf('='); return new { Key = Uri.UnescapeDataString(part[..eqIndex]), Value = eqIndex > 0 ? Uri.UnescapeDataString(part[(eqIndex + 1)..]) : "" }; }).ToDictionary(x => x.Key, x => x.Value);

                string user = rawPairs["user"];
                string token = rawPairs["token"];

                EmailVerify emailVerify = new EmailVerify
                {
                    username = user.Trim(),
                    token = token.Trim()
                };
                var viewModel = await this.model.InitializeViewModel(context.Entity);
                var response = await _registrationService.ConfirmEmailAsync<RegistrationResponse>(emailVerify);
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ViewBag.Response = response.data;
                }
                else
                {
                    ViewBag.Response = null;
                }
                
                return this.View("ConfirmEmail", viewModel);
            }
            else
            {
                var viewModel = await this.model.InitializeViewModel(context.Entity);

                if (!string.IsNullOrEmpty(viewModel.Warning))
                {
                    context.SetWarning(viewModel.Warning);
                }

                return this.View(context.Entity.SfViewName, viewModel);
            }
        }        
    }
}
