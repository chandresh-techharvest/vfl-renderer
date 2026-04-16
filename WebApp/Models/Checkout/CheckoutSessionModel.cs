using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Progress.Sitefinity.RestSdk.OData;
using System;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Entities.Checkout;
using VFL.Renderer.ViewModels.Checkout;
using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.RestSdk;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;


namespace VFL.Renderer.Models.Checkout
{
    public class CheckoutSessionModel : ICheckoutSessionModel
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IODataRestClient restService;

        public CheckoutSessionModel(IHttpContextAccessor httpContextAccessor, IMemoryCache cache,IODataRestClient restService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.restService = restService;
            _cache = cache;
        }

        public virtual async Task<CheckoutViewModel> InitializeViewModel(CheckoutEntity entity)
        {
            CheckoutViewModel viewModel = new CheckoutViewModel();
            var uriBuilder = new UriBuilder
            {
                Scheme = httpContextAccessor.HttpContext.Request.Scheme,
                Host = httpContextAccessor.HttpContext.Request.Host.Host,
                Port = httpContextAccessor.HttpContext.Request.Host.Port.GetValueOrDefault(80),
            };
            Uri location = uriBuilder.Uri;
            if (entity.CustomerSuportPage.ItemIdsOrdered != null)
            {
                var item = entity.CustomerSuportPage.ItemIdsOrdered.FirstOrDefault();
                var pageNode = await this.restService.GetItem<PageNodeDto>(item);

                string relativeUrl = uriBuilder.Uri + pageNode.UrlName;


                viewModel.CustomerSuportPageUrl = relativeUrl;
            }

            if (entity.HomePage != null)
            {
                var item = entity.HomePage.ItemIdsOrdered.FirstOrDefault();
                var pageNode = await this.restService.GetItem<PageNodeDto>(item);
                viewModel.HomePageUrl = pageNode.ViewUrl;

            }

            if (entity.TermsConditionsPage != null)
            {
                var item = entity.TermsConditionsPage.ItemIdsOrdered.FirstOrDefault();
                viewModel.TermsConditionsPage = await this.restService.GetItem<PageNodeDto>(item);                
            }

            return viewModel;
        }

        public void Save(CheckoutEntity session)
        {
            _cache.Set(
                session.Id,
                session,
                TimeSpan.FromMinutes(15) // expiry
            );
        }

        public CheckoutEntity Get(string id)
        {
            _cache.TryGetValue(id, out CheckoutEntity session);
            return session;
        }

        public void Remove(string id)
        {
            _cache.Remove(id);
        }
    }
}
