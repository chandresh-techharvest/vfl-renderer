using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System;
using System.Threading.Tasks;
using VFL.Renderer.Entities.VitiProduct;
using VFL.Renderer.Models.VitiProduct;

namespace VFL.Renderer.ViewComponents
{
    [SitefinityWidget]
    public class VitiProductViewComponent : ViewComponent
    {
        private readonly IVitiProductModel model;

        public VitiProductViewComponent(IVitiProductModel model)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
        }

        /// <summary>
        /// Invokes the view component.
        /// </summary>
        /// <param name="context">The view component context.</param>
        /// <returns>The view component result.</returns>
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<VitiProductEntity> context)
        {
            var viewModels = await this.model.GetViewModels(context.Entity);
            return this.View(context.Entity.ViewName, viewModels);
        }
    }
}
