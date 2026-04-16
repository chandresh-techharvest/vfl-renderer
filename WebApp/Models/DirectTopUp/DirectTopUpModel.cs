using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.RestSdk;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using System.Threading.Tasks;
using VFL.Renderer.Entities.DirectTopUp;
using VFL.Renderer.ViewModels.DirectTopUp;

namespace VFL.Renderer.Models.DirectTopUp
{
    public class DirectTopUpModel : IDirectTopUpModel
    {
        private readonly IRestClient restService;
        public DirectTopUpModel(IRestClient restService) {
            this.restService = restService;
        }

        public async Task<DirectTopUpViewModel> GetViewModels(DirectTopUpEntity entity)
        {
            var viewModel = new DirectTopUpViewModel();

            if (entity.CustomerSupport.HasSelectedItems())
            {
                viewModel.CustomerSupport = await this.restService.GetItem<PageNodeDto>(entity.CustomerSupport);
            }

            if (entity.TermsConditionsPage.HasSelectedItems())
            {
                viewModel.TermsConditionsPage = await this.restService.GetItem<PageNodeDto>(entity.TermsConditionsPage);
            }
            return viewModel;
        }
    }
}
