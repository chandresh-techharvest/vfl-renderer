using System.Threading.Tasks;
using Vfl.Renderer.Entities.LoginStatus;
using Vfl.Renderer.ViewModels.LoginStatus;

namespace Vfl.Renderer.Models.LoginStatus
{
    /// <summary>
    /// The model interface.
    /// </summary>
    public interface ILoginStatusModel
    {
        /// <summary>
        /// Gets the view models.
        /// </summary>
        /// <returns>The generated view models.</returns>
        Task<LoginStatusViewModel> GetViewModels(LoginStatusEntity entity);
    }
}
