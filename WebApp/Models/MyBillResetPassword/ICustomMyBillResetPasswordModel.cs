using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillResetPassword;
using VFL.Renderer.ViewModels.MyBillResetPassword;

namespace VFL.Renderer.Models.MyBillResetPassword
{
    public interface ICustomMyBillResetPasswordModel
    {
        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <param name="entity">The MyBill reset password entity.</param>
        /// <returns>The view model of the widget.</returns>
        Task<CustomMyBillResetPasswordViewModel> InitializeViewModel(CustomMyBillResetPasswordEntity entity);
    }
}
