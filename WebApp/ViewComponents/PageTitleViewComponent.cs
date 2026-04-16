using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Threading.Tasks;
using VFL.Renderer.Entities.PageTitle;
using VFL.Renderer.Entities.VitiProduct;
using VFL.Renderer.Models.PageTitle;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// The view component for the registration widget.
    /// </summary>
    [SitefinityWidget(Title = "Page Title", Order = 1, Category = WidgetCategory.Content, IconName = "short-text", NotPersonalizable = true)]
    [ViewComponent(Name = "PageTitle")]
    public class PageTitleViewComponent : ViewComponent
    {
        private readonly IPageTitleModel model;

        public PageTitleViewComponent(IPageTitleModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Invokes the page title widget creation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="IViewComponentResult"/> representing the result of the operation.</returns>
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<PageTitleEntity> context)
        {
            var viewModels = await this.model.GetViewModels(context.Entity);
            return this.View(context.Entity.ViewName, viewModels);
        }
    }
}
