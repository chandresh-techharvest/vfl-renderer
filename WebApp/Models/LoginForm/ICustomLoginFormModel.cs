
using System.Threading.Tasks;
using VFL.Renderer.Entities.LoginForm;
using VFL.Renderer.ViewModels.LoginForm;

namespace VFL.Renderer.Models.LoginForm
{
    public interface ICustomLoginFormModel
    {
        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <param name="entity">The login form entity.</param>
        /// <returns>The view model of the widget.</returns> 
        Task<CustomLoginFormViewModel> InitializeViewModel(CustomLoginFormEntity entity);
    }
}
