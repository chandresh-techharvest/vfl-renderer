using System.Threading.Tasks;
using VFL.Renderer.Entities.Registration;

namespace VFL.Renderer.Models.Registration
{
    public interface IRegistrationModel
    {
        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <param name="entity">The registration entity.</param>
        /// <returns>The view model of the widget.</returns>
        public Task<RegistrationViewModel> InitializeViewModel(RegistrationEntity entity);
    }
}
