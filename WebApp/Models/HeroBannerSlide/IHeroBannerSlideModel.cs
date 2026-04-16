
using System.Collections.Generic;
using System.Threading.Tasks;
using VFL.Renderer.Entities.HeroBannerSlide;
using VFL.Renderer.ViewModels.HeroBannerSlide;

namespace VFL.Renderer.Models.HeroBannerSlide
{
    public interface IHeroBannerSlideModel
    {
        /// <summary>
        /// Gets the view models.
        /// </summary>
        /// <returns>The generated view models.</returns>
        Task<IList<ItemViewModel>> GetViewModels(HeroBannerSlideEntity entity);
    }
}
