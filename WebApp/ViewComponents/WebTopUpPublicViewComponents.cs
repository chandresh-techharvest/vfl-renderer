//using Microsoft.AspNetCore.Mvc;
//using Progress.Sitefinity.AspNetCore.ViewComponents;
//using System.Threading.Tasks;
//using VFL.Renderer.Entities.Dashboard;
//using VFL.Renderer.Entities.WebTopUp;
//using VFL.Renderer.Models.Dashboard;
//using VFL.Renderer.Models.WebTopUp;
//using VFL.Renderer.Services.Plans;

//namespace VFL.Renderer.ViewComponents
//{

//    [SitefinityWidget(Title = "WebTopUpPublic", Order = 1, Category = WidgetCategory.Content, IconName = "content-section", NotPersonalizable = true)]
//    [ViewComponent(Name = "WebTopUpPublic")]
//    public class WebTopUpPublicComponents : ViewComponent
//    {

//        private readonly IWebTopUpModel model;
//        private readonly IPlansService _plansService;

//        public WebTopUpPublicComponents(IPlansService plansService, IWebTopUpModel model)
//        {

//            _plansService = plansService;
//            this.model = model;
//        }
//        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<WebTopUpEntity> context)
//        {

//            var viewModel = await this.model.InitializeViewModel(context.Entity);

//            var result = await _plansService.GetallPublicTopUpAmountsAsync<TopUpAmountResponse>();


//            viewModel.TopUpAmounts = result?.data?.AllTopUpAmounts;


//            return View(context.Entity.ViewName, viewModel);
//        }

//    }
//}
