using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.Models;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Threading.Tasks;
using VFL.Renderer.Entities.Checkout;
using VFL.Renderer.Entities.Dashboard;
using VFL.Renderer.Entities.PageTitle;
using VFL.Renderer.Entities.WebTopUp;
using VFL.Renderer.Models.Checkout;
using VFL.Renderer.Models.PageTitle;
using VFL.Renderer.Models.WebTopUp;
using VFL.Renderer.ViewModels.Checkout;

namespace VFL.Renderer.ViewComponents
{

    [SitefinityWidget(Title = "Checkout", Order = 1, Category = WidgetCategory.Content, IconName = "content-section", NotPersonalizable = true)]
    [ViewComponent(Name = "Checkout")]
    public class CheckoutViewComponent : ViewComponent
    {
        private readonly ICheckoutSessionModel _store;

        public CheckoutViewComponent(ICheckoutSessionModel store)
        {
            _store = store;
        }

        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<CheckoutEntity> context)
        {
            var query = HttpContext.Request.Query;
            var viewModel = await this._store.InitializeViewModel(context.Entity);


            var status = query["status"].ToString().ToLower();

            if (status == "success")
                return View("Success", viewModel);

            if (status == "fail")
                return View("Fail", viewModel);

            var sid = query["sid"].ToString();
      
            bool isPaymentCallback =
                query.ContainsKey("rID") ||
                query.ContainsKey("tID") ||
                query.ContainsKey("token")||
                query.ContainsKey("sessionId"); 

            if (string.IsNullOrEmpty(sid) && !isPaymentCallback)
                return View("Expired", new CheckoutViewModel());

            if (!string.IsNullOrEmpty(sid))
            {
                var session = _store.Get(sid);
                if (session == null)
                    return View("Expired", new CheckoutViewModel());

                viewModel.Email = session.Email;
                viewModel.PhoneNumber = session.PhoneNumber;
                viewModel.PlanCode = session.PlanCode;
                viewModel.Amount = session.Amount;
                viewModel.PageType = context.Entity.PageType;
                viewModel.PageUrl = context.Entity.PageUrl;

                return View(context.Entity.ViewName, viewModel);
            }
            return View(context.Entity.ViewName, viewModel);
        }
    }
}
