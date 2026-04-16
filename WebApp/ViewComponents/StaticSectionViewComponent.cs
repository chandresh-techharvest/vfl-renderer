using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using Progress.Sitefinity.AspNetCore.Widgets.Models.Section;
using System;
using System.Linq;
using Vfl.Renderer.ViewModels.StaticSection;
using VFL.Renderer.Entities.StaticSection;
using VFL.Renderer.ViewModels;
using Vfl.Renderer.Models.StaticSection;
using System.Threading.Tasks;

namespace VFL.Renderer.ViewComponents
{
    [SitefinityWidget(Category = WidgetCategory.Layout, Title = "Static section")]
    public class StaticSectionViewComponent : ViewComponent
    {
        private IStaticSection model;

        public StaticSectionViewComponent(IStaticSection model)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
        }
        public async Task<IViewComponentResult> InvokeAsync(ICompositeViewComponentContext<StaticSectionEntity> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            StaticSectionViewModel viewModel = await this.model.InitializeViewModel(context.Entity);
            foreach (var child in context.ChildComponents)
            {
                child.Properties.Add("FromParent", "Val from parent");
            }
            
            viewModel.Context = context;

            return this.View(context.Entity.ViewType ?? "Container", viewModel);
        }
    }
}
