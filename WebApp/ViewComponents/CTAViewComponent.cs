using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Threading.Tasks;
using VFL.Renderer.Entities.CTA;
using VFL.Renderer.Models.CTA;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// The view component for the CTA widget.
    /// </summary>
    [SitefinityWidget(Title = "CTA", Order = 1, Category = WidgetCategory.Content, IconName = "content-section", NotPersonalizable = true)]
    [ViewComponent(Name = "CTA")]
    public class CTAViewComponent : ViewComponent
    {
        private readonly ICTAModel model;

        public CTAViewComponent(ICTAModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Invokes the view component.
        /// </summary>
        /// <param name="context">The view component context.</param>
        /// <returns>The view component result.</returns>
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<CTAEntity> context)
        {
            var viewModels = await this.model.GetViewModels(context.Entity);
            return this.View(context.Entity.ViewName, viewModels);
        }
    }
}
