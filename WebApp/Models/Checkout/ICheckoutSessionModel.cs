using System.Threading.Tasks;
using VFL.Renderer.Entities.Checkout;
using VFL.Renderer.ViewModels.Checkout;

namespace VFL.Renderer.Models.Checkout
{
    public interface ICheckoutSessionModel
    {
        void Save(CheckoutEntity session);
        CheckoutEntity Get(string id);
        void Remove(string id);
        Task<CheckoutViewModel> InitializeViewModel(CheckoutEntity entity);
    }
}
