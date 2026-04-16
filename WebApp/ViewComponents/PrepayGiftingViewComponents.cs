using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Config;
using VFL.Renderer.Entities.PrepayGifting;
using VFL.Renderer.Services.Plans;
using VFL.Renderer.Services.PrepayGifting;
using VFL.Renderer.Services.PrepayGifting.Models;
using VFL.Renderer.Services.Profile.Models;
using VFL.Renderer.ViewModels.PrepayGifting;


namespace VFL.Renderer.ViewComponents
{


    [SitefinityWidget(Title = "PrepayGifting", Order = 1, Category = WidgetCategory.Content, IconName = "content-section", NotPersonalizable = true)]
    [ViewComponent(Name = "PrepayGifting")]
    public class PrepayGiftingViewComponents : ViewComponent
    {
        private readonly IPrepayGiftingService _prepayGiftingservice;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiSettings _apiSettings;
        public PrepayGiftingViewComponents(IPrepayGiftingService prepayGiftingService, IOptions<ApiSettings> options, IHttpContextAccessor httpContextAccessor)
        {

            _prepayGiftingservice = prepayGiftingService;
            this._httpContextAccessor = httpContextAccessor;
            _apiSettings = options.Value;
        }

        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<PrepayGiftingEntity> context)
        {
            if (!User.Identity.IsAuthenticated)
                _httpContextAccessor.HttpContext.Response.Redirect(_apiSettings.LoginPath);
            var viewModel = new PrepayGiftingViewModel();
            return View(context.Entity.ViewName, viewModel);
        }
    }
}
