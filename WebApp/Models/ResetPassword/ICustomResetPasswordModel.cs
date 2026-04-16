using Progress.Sitefinity.AspNetCore.Widgets.Models.ResetPassword;
using System.Threading.Tasks;
using VFL.Renderer.Entities.ResetPassword;
using VFL.Renderer.ViewModels.ResetPassword;

namespace VFL.Renderer.Models.ResetPassword
{
    public interface ICustomResetPasswordModel
    {
        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <param name="entity">The forgotten password entity.</param>
        /// <returns>The view model of the widget.</returns>
        Task<CustomResetPasswordViewModel> InitializeViewModel(CustomResetPasswordEntity entity);
    }
}
