using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Threading.Tasks;
using VFL.Renderer.Entities.Dashboard;
using VFL.Renderer.Entities.WebTopUp;

using VFL.Renderer.Models.WebTopUp;
using VFL.Renderer.Services.Plans;

namespace VFL.Renderer.ViewComponents
{

    [SitefinityWidget(Title = "WebTopUp", Order = 1, Category = WidgetCategory.Content, IconName = "content-section", NotPersonalizable = true)]
    [ViewComponent(Name = "WebTopUp")]
    public class WebTopUpViewComponents : ViewComponent
    {
     
        private readonly IPlansService _plansService;
        private readonly IWebTopUpModel model;

        public WebTopUpViewComponents( IPlansService plansService, IWebTopUpModel model)
        {
          
            _plansService = plansService;
            this.model = model;
        }
    
        public  async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<WebTopUpEntity> context)
        {

            var viewModel = await this.model.InitializeViewModel(context.Entity);

            var result =await _plansService.GetallPublicTopUpAmountsAsync<TopUpAmountResponse>();


            viewModel.TopUpAmounts = result?.data?.AllTopUpAmounts;
            bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            var viewName = isAuthenticated ? context.Entity.ViewName : "PublicView";

            return View(viewName, viewModel);


        }

    }
}
