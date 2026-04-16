using System.Threading.Tasks;
using VFL.Renderer.Entities.WebTopUp;

namespace VFL.Renderer.Models.WebTopUp
{
    public interface IWebTopUpModel
    {
        public Task<WebTopUpViewModel> InitializeViewModel(WebTopUpEntity entity);

    }
}
