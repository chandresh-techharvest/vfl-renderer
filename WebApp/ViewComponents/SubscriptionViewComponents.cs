using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Config;
using VFL.Renderer.Entities.Dashboard;
using VFL.Renderer.Entities.PurchasePlan;
using VFL.Renderer.Entities.Subscription;
using VFL.Renderer.Models.Subscription;

using VFL.Renderer.Services.Profile.Models;
using VFL.Renderer.Services.Subscription;
using VFL.Renderer.Services.Subscription.Models;

using VFL.Renderer.ViewModels.Subscription;

namespace VFL.Renderer.ViewComponents
{


    [SitefinityWidget(Title = "Plan Activation(Subscription)", Order = 1, Category = WidgetCategory.Content, IconName = "content-section", NotPersonalizable = true)]
    [ViewComponent(Name = "Subscription")]
    public class SubscriptionViewComponents : ViewComponent
    {


        private readonly ISubscriptionModel _subscriptionModel;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiSettings _apiSettings;
        public SubscriptionViewComponents(ISubscriptionModel model , ISubscriptionService subscriptionService, IOptions<ApiSettings> options, IHttpContextAccessor httpContextAccessor) {
            _subscriptionModel = model;
            _subscriptionService = subscriptionService;
            this._httpContextAccessor = httpContextAccessor;
            _apiSettings = options.Value;

        }
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<SubscriptionEntity> context)
        {
            if (!User.Identity.IsAuthenticated)
                _httpContextAccessor.HttpContext.Response.Redirect(_apiSettings.LoginPath);
            var viewModels = await _subscriptionModel.InitializeViewModel(context.Entity);
            if (viewModels == null)
            {
                viewModels = new SubscriptionViewModel();
            }
            var typesResult = await _subscriptionService.GetPurchasePlanTypesAsync<SubscriptionResponse>();
            viewModels.PlanTypes = typesResult?.data?.AllCategories;




            string selectedNumber = null;
            var cookieValue = CookieHelper.GetCookie(HttpContext.Request, "luData", true);
            if (!string.IsNullOrEmpty(cookieValue))
            {
                try
                {
                    var cookieData = JsonConvert.DeserializeObject<ProfileSettingsResponse>(cookieValue);

                    // 2️⃣ Find selected device (NO default)
                    var selectedDevice = cookieData?.devices?
                        .Where(d => d.isSelected == true)
                        .SingleOrDefault(); // only one should be selected


                    if (selectedDevice == null)
                    {
                        return View(context.Entity.ViewName, viewModels);
                    }

                    selectedNumber = selectedDevice.number;
                }
                catch
                {
                    return View(context.Entity.ViewName, viewModels);
                }
            }
            else
            {
                return View(context.Entity.ViewName, viewModels);
            }

            // 3️⃣ Create request
            if (!int.TryParse(selectedNumber, out var number))
                return View(context.Entity.ViewName, viewModels);






            var activeResult = await _subscriptionService.GetAllSubscribedPlansByNumber<SubscriptionResponse>(number);


            viewModels.ActivePlans = activeResult?.data?.subscribedPlans;

            //var planResult = await _subscriptionService.GetAllSubscriptionPlans<SubscriptionResponse>(number);
            //viewModels.AllPlans = planResult?.data?.AllPlans;

            return View(context.Entity.ViewName, viewModels);
        }

    }
}
