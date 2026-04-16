using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Threading.Tasks;
using VFL.Renderer.Config;
using VFL.Renderer.Entities.DirectTopUp;
using VFL.Renderer.Models.DirectTopUp;


namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// The view component for the Direct Top Up widget.
    /// </summary>
    [SitefinityWidget(Title = "Direct Top Up", Order = 1, Category = WidgetCategory.Content, IconName = "short-text", NotPersonalizable = true)]
    [ViewComponent(Name = "DirectTopUp")]
    public class DirectTopUpViewComponent : ViewComponent
    {
        private readonly IDirectTopUpModel model;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiSettings _apiSettings;

        public DirectTopUpViewComponent(IDirectTopUpModel model,IOptions<ApiSettings> options, IHttpContextAccessor httpContextAccessor)
        {
            this.model = model;
            this._httpContextAccessor = httpContextAccessor;
            _apiSettings = options.Value;
        }

        /// <summary>
        /// Invokes the view component.
        /// </summary>
        /// <param name="context">The view component context.</param>
        /// <returns>The view component result.</returns>
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<DirectTopUpEntity> context)
        {
            if (!User.Identity.IsAuthenticated)
                _httpContextAccessor.HttpContext.Response.Redirect(_apiSettings.LoginPath);

            var viewModel = await this.model.GetViewModels(context.Entity);
            return this.View(context.Entity.ViewName,viewModel);
        }
    }
}
