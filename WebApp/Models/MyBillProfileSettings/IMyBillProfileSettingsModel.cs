using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillProfileSettings;
using VFL.Renderer.ViewModels.MyBillProfileSettings;

namespace VFL.Renderer.Models.MyBillProfileSettings
{
    /// <summary>
    /// Interface for MyBill Profile Settings model
    /// </summary>
    public interface IMyBillProfileSettingsModel
    {
        /// <summary>
        /// Gets the view model for the MyBill Profile Settings widget
        /// </summary>
        Task<MyBillProfileSettingsViewModel> GetViewModels(MyBillProfileSettingsEntity entity);
    }
}
