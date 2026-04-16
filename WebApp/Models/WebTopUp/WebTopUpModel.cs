using Microsoft.AspNetCore.Http;
using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.RestSdk;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using Progress.Sitefinity.RestSdk.OData;
using System;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Dto;
using VFL.Renderer.Entities.WebTopUp;

namespace VFL.Renderer.Models.WebTopUp
{
    public class WebTopUpModel : IWebTopUpModel
    {

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IODataRestClient restService;

        public WebTopUpModel(
            IHttpContextAccessor httpContextAccessor,
            IODataRestClient restService)
        {
            this.httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            this.restService = restService
                ?? throw new ArgumentNullException(nameof(restService));
        }


        public virtual async Task<WebTopUpViewModel> InitializeViewModel(WebTopUpEntity entity) {



            var viewModel = new WebTopUpViewModel();
            if (entity.CheckoutPage.HasSelectedItems())
            {
                viewModel.CheckoutPage = await this.restService.GetItem<PageNodeDto>(entity.CheckoutPage);
            }

            var uriBuilder = new UriBuilder
            {
                Scheme = httpContextAccessor.HttpContext.Request.Scheme,
                Host = httpContextAccessor.HttpContext.Request.Host.Host,
                Port = httpContextAccessor.HttpContext.Request.Host.Port.GetValueOrDefault(80),
            };
            if (entity.CustomerSuportPage?.ItemIdsOrdered?.Any() == true)
            {
                var item = entity.CustomerSuportPage.ItemIdsOrdered.First();

                var pageNode =
                    await restService.GetItem<PageNodeDto>(item);

                viewModel.CustomerSuportPage =
                    uriBuilder.Uri + pageNode.UrlName;
            }

            return viewModel;

        }


    }
}
