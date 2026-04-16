using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Polly;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using Progress.Sitefinity.AspNetCore.Widgets.Models.Profile;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.Config;
using VFL.Renderer.Models.Profile;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// The view component for the Profile widget.
    /// </summary>
    /// 
    [Authorize]
    [SitefinityWidget(Title = "Custom Profile", Order = 5, Section = WidgetSection.Other, Category = WidgetCategory.Content, IconName = "profile", NotPersonalizable = true)]
    [ViewComponent(Name = "CustomProfile")]
    public class CustomProfileViewComponent : ViewComponent
    {

        private ICustomProfileModel model;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiSettings _apiSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileViewComponent"/> class.
        /// </summary>
        /// <param name="profileModel">The reset password model.</param>
        public CustomProfileViewComponent(ICustomProfileModel profileModel, IOptions<ApiSettings> options, IHttpContextAccessor httpContextAccessor)
        {
            this.model = profileModel;
            this._httpContextAccessor = httpContextAccessor;
            _apiSettings = options.Value;
        }

        /// <summary>
        /// Invokes the Profile widget creation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<CustomProfileEntity> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            
            bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            CustomProfileViewModel customProfileViewModel = new CustomProfileViewModel();
            if (isAuthenticated)
            {

                try
                {
                    customProfileViewModel = await this.model.InitializeViewModel(context.Entity);
                }
                catch (HttpRequestException ex)
                {
                    customProfileViewModel.Warning = ex.Message;
                }
                catch (Exception ex)
                {
                    customProfileViewModel.Warning = ex.Message;
                }

                if (!string.IsNullOrEmpty(customProfileViewModel.Warning))
                {
                    context.SetWarning(customProfileViewModel.Warning);
                }
            }
            else {
                if (!User.Identity.IsAuthenticated)
                    _httpContextAccessor.HttpContext.Response.Redirect(_apiSettings.LoginPath);
            }
            return this.View(context.Entity.SfViewName, customProfileViewModel);
        }
    }
}
