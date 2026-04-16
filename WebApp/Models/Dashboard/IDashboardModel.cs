using System.Threading.Tasks;
using VFL.Renderer.Entities.Dashboard;
using VFL.Renderer.ViewModels.Dashboard;

namespace VFL.Renderer.Models.Dashboard
{
    public interface IDashboardModel
    {
        /// <summary>
        /// Gets the view models.
        /// </summary>
        /// <returns>The generated view models.</returns>
        Task<DashboardViewModel> GetViewModels(DashboardEntity entity);
    }
}
