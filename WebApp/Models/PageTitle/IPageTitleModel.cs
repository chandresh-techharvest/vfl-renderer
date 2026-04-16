using System.Collections.Generic;
using System.Threading.Tasks;
using VFL.Renderer.Entities.PageTitle;
using VFL.Renderer.ViewModels.PageTitle;


namespace VFL.Renderer.Models.PageTitle
{
    public interface IPageTitleModel
    {
        /// <summary>
        /// Gets the view models.
        /// </summary>
        /// <returns>The generated view models.</returns>
        Task<PageTitleViewModel> GetViewModels(PageTitleEntity entity);
    }
}
