using Progress.Sitefinity.AspNetCore.Widgets.Models.Section;
using System;
using System.Threading.Tasks;
using Vfl.Renderer.ViewModels.StaticSection;
using VFL.Renderer.Entities.StaticSection;

namespace Vfl.Renderer.Models.StaticSection
{
    public class StaticSection : IStaticSection
    {
        public virtual async Task<StaticSectionViewModel> InitializeViewModel(StaticSectionEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var viewModel = new StaticSectionViewModel();
            viewModel.CustomClasses = entity.CustomCssClass;
            return viewModel;
        }
    }
}
