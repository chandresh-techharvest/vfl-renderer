using Progress.Sitefinity.AspNetCore.Widgets.Models.Section;
using System.Threading.Tasks;
using Vfl.Renderer.ViewModels.StaticSection;
using VFL.Renderer.Entities.StaticSection;

namespace Vfl.Renderer.Models.StaticSection
{
    public interface IStaticSection
    {
        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <param name="entity">The section entity.</param>
        /// <returns>The view model.</returns>
        Task<StaticSectionViewModel> InitializeViewModel(StaticSectionEntity entity);
    }
}
