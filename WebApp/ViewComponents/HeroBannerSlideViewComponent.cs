using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using Progress.Sitefinity.AspNetCore.Widgets.Models.ContentList;
using System.Threading.Tasks;
using VFL.Renderer.Entities.HeroBannerSlide;
using VFL.Renderer.Models.HeroBannerSlide;

namespace VFL.Renderer.ViewComponents
{
    [SitefinityWidget]
    public class HeroBannerSlideViewComponent : ViewComponent
    {
        private IHeroBannerSlideModel model;
        /// <summary>
        /// Initializes a new instance of the <see cref="HeroBannerSlideViewComponent"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public HeroBannerSlideViewComponent(IHeroBannerSlideModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Invokes the view component.
        /// </summary>
        /// <param name="context">The view component context.</param>
        /// <returns>The view component result.</returns>
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<HeroBannerSlideEntity> context)
        {
            var viewModels = await this.model.GetViewModels(context.Entity);            
            return this.View(context.Entity.ViewName, viewModels);
        }
    }
}
