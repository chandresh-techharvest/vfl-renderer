using System.Threading.Tasks;
using VFL.Renderer.Entities.PurchasePlan;
using VFL.Renderer.Entities.Subscription;
using VFL.Renderer.Models.PurchasePlan;
using VFL.Renderer.ViewModels.Subscription;

namespace VFL.Renderer.Models.Subscription
{
    public class SubscriptionModel : ISubscriptionModel
    {

        public virtual async Task<SubscriptionViewModel> InitializeViewModel(SubscriptionEntity entity) {



            var viewModel = new SubscriptionViewModel();
            return viewModel;


        }
    }
}
