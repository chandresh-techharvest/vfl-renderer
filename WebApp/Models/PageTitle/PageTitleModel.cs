using System.Collections.Generic;
using System.Threading.Tasks;
using VFL.Renderer.Entities.PageTitle;
using VFL.Renderer.ViewModels.PageTitle;

namespace VFL.Renderer.Models.PageTitle
{
    public class PageTitleModel : IPageTitleModel
    {
        public async Task<PageTitleViewModel> GetViewModels(PageTitleEntity entity)
        {
            string title = entity.Title;
            var viewModel = new PageTitleViewModel
            {
                Title = title
            };
            return viewModel;
        }
    }
}
