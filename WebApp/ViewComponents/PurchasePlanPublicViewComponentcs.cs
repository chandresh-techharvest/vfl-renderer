using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Threading.Tasks;
using VFL.Renderer.Entities.PurchasePlan;
using VFL.Renderer.Entities.WebTopUp;
using VFL.Renderer.Models.PurchasePlan;
using VFL.Renderer.Models.WebTopUp;
using VFL.Renderer.Services.Plans;

namespace VFL.Renderer.ViewComponents
{

    [SitefinityWidget(Title = "PurchasePlanPublic", Order = 1, Category = WidgetCategory.Content, IconName = "content-section", NotPersonalizable = true)]
    [ViewComponent(Name = "PurchasePlanPublic")]
    public class PurchasePlanPublicViewComponentcs : ViewComponent
    {


        private readonly IPlansService _plansService;
        private readonly IPurchasePlanModel model;
        public PurchasePlanPublicViewComponentcs(IPlansService plansService, IPurchasePlanModel model)
        {

            _plansService = plansService;
            this.model = model;
        }



        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<PurchasePlanEntity> context)
        {

            var viewModel = await this.model.InitializeViewModel(context.Entity);

            
            var typesResult = await _plansService.GetPurchasePlanTypesAsync<PlanTypeResponse>();

            viewModel.PlanTypes = typesResult?.data?.AllCategories;

            var paymentResult =await _plansService.GetPurchasePlanPaymentMethodsAsync<PymentMethodResponse>();
            viewModel.PaymentMethods = paymentResult?.data?.AllCategories;

            return View(context.Entity.ViewName, viewModel);
        }



    }
}
