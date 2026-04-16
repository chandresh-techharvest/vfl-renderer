using System.Threading.Tasks;
using VFL.Renderer.Entities.PurchasePlan;
using VFL.Renderer.Entities.WebTopUp;
using VFL.Renderer.Models.WebTopUp;

namespace VFL.Renderer.Models.PurchasePlan
{
    
        public interface IPurchasePlanModel
    {
            public Task<PurchasePlanViewModel> InitializeViewModel(PurchasePlanEntity entity);

        }
    
}
