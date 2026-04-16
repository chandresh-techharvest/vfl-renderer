

using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillLoginForm;
using VFL.Renderer.ViewModels.MyBillLoginForm;

namespace VFL.Renderer.Models.MyBillLoginForm
{
    /// <summary>
    /// Interface for MyBill Login form model
    /// </summary>
    public interface ICustomMyBillLoginModel
    {
        /// <summary>
        /// Initializes the view model for MyBill login form
        /// </summary>
        /// <param name="entity">The MyBill login form entity</param>
        /// <returns>Populated MyBill login form view model</returns>
        Task<MyBillLoginFormViewModel> InitializeViewModel(MyBillLoginFormEntity entity);
    }
}

