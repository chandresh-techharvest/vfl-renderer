using Microsoft.AspNetCore.Http;
using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.RestSdk;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using Progress.Sitefinity.RestSdk.OData;
using System;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Entities.PurchasePlan;
using VFL.Renderer.Entities.WebTopUp;
using VFL.Renderer.Models.WebTopUp;

namespace VFL.Renderer.Models.PurchasePlan
{
    public class PurchasePlanModel : IPurchasePlanModel
    {


        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IODataRestClient restService;


        public PurchasePlanModel(IHttpContextAccessor httpContextAccessor,IODataRestClient restService) 
        {
            this.httpContextAccessor = httpContextAccessor
                  ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            this.restService = restService
                ?? throw new ArgumentNullException(nameof(restService));

        }
        public virtual async Task<PurchasePlanViewModel> InitializeViewModel(PurchasePlanEntity entity)
        {
            var viewModel = new PurchasePlanViewModel();

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
