using System.Threading.Tasks;
using VFL.Renderer.Entities.Subscription;
using VFL.Renderer.Models.PurchasePlan;
using VFL.Renderer.ViewModels.Subscription;

namespace VFL.Renderer.Models.Subscription
{
    public interface ISubscriptionModel
    {

      public Task<SubscriptionViewModel> InitializeViewModel(SubscriptionEntity entity);
    
    }

  
}
