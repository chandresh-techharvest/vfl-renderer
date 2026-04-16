using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System;
using System.Threading.Tasks;
using VFL.Renderer.Entities.PricePackage;
using VFL.Renderer.Models.PricePackage;
using VFL.Renderer.ViewModels.PricePackage;

namespace VFL.Renderer.ViewComponents
{
    [SitefinityWidget]
    public class PricePackageViewComponent: ViewComponent
    {
        private readonly IPricePackageModel model;
        public PricePackageViewComponent(IPricePackageModel model)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
        }

        /// <summary>
        /// Invokes the view component.
        /// </summary>
        /// <param name="context">The view component context.</param>
        /// <returns>The view component result.</returns>
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<PricePackageEntity> context)
        {
            var viewModels = await this.model.GetViewModels(context.Entity);
            return this.View(context.Entity.ViewName, viewModels);
        }
    }
}
