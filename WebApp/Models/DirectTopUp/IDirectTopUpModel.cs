using System.Threading.Tasks;
using VFL.Renderer.Entities.DirectTopUp;
using VFL.Renderer.ViewModels.DirectTopUp;

namespace VFL.Renderer.Models.DirectTopUp
{
    public interface IDirectTopUpModel
    {
        /// <summary>
        /// Gets the view models.
        /// </summary>
        /// <returns>The generated view models.</returns>
        Task<DirectTopUpViewModel> GetViewModels(DirectTopUpEntity entity);
    }
}
