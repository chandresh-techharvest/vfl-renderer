using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Threading.Tasks;
using VFL.Renderer.Config;
using VFL.Renderer.Entities.Dashboard;
using VFL.Renderer.Models.Dashboard;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// The view component for the Dashboard widget.
    /// </summary>
    [SitefinityWidget(Title = "Dashboard", Order = 1, Category = WidgetCategory.Content, IconName = "content-section", NotPersonalizable = true)]
    [ViewComponent(Name = "Dashboard")]
    public class DashboardViewComponent : ViewComponent
    {
        private readonly IDashboardModel model;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiSettings _apiSettings;

        public DashboardViewComponent(DashboardModel model, IOptions<ApiSettings> options, IHttpContextAccessor httpContextAccessor)
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
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<DashboardEntity> context)
        {
            if (!User.Identity.IsAuthenticated)
            {
                bool isBackendUser = HttpContext.User.IsInRole("Administrators");
                if (!isBackendUser)
                {
                    _httpContextAccessor.HttpContext.Response.Redirect(_apiSettings.LoginPath);
                }
            }

            var viewModels = await this.model.GetViewModels(context.Entity);
            return this.View(context.Entity.ViewName, viewModels);
        }
    }
}
