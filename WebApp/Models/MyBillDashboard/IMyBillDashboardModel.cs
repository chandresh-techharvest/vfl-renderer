using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillDashboard;
using VFL.Renderer.ViewModels.MyBillDashboard;

namespace VFL.Renderer.Models.MyBillDashboard
{
    /// <summary>
    /// Interface for MyBill Dashboard model
    /// </summary>
    public interface IMyBillDashboardModel
    {
        /// <summary>
        /// Gets the view models for the MyBill Dashboard widget
        /// </summary>
        /// <param name="entity">The MyBill dashboard entity from Sitefinity</param>
        /// <returns>Populated MyBillDashboardViewModel</returns>
        Task<MyBillDashboardViewModel> GetViewModels(MyBillDashboardEntity entity);
    }
}
