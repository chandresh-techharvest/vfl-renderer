using Progress.Sitefinity.AspNetCore.Widgets.Models.Profile;
using System.Threading.Tasks;

namespace VFL.Renderer.Models.Profile
{
    public interface ICustomProfileModel
    {
        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <param name="entity">The Profile entity.</param>
        /// <returns>The view model of the widget.</returns>
        Task<CustomProfileViewModel> InitializeViewModel(CustomProfileEntity entity);
    }
}
