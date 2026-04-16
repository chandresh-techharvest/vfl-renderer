using Newtonsoft.Json;
using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.RestSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Dto;
using VFL.Renderer.Entities.PricePackage;
using VFL.Renderer.ViewModels.PricePackage;
using VFL.Renderer.ViewModels.VitiProduct;

namespace VFL.Renderer.Models.PricePackage
{
    public class PricePackageModel : IPricePackageModel
    {
        private readonly IRestClient service;
        public PricePackageModel(IRestClient service)
        {
            this.service = service;
        }

        public async Task<IList<PricePackageViewModel>> GetViewModels(PricePackageEntity entity)
        {
            var response = await this.service.GetItems<PricePackageItem>(entity.PricePackages, new GetAllArgs() { Fields = new List<string>() { "Id", "Title", "Features", "Category", "IconClass", "Price", "PackageLink" } }).ConfigureAwait(true);
            return response.Items.Select(x => this.GetItemViewModel(x)).ToList();
        }

        private PricePackageViewModel GetItemViewModel(PricePackageItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var viewModel = new PricePackageViewModel()
            {
                Title = item.Title,
                Features = item.Features,
                Category = item.Category,
                IconClass = item.IconClass,
                PackageLink = GetLinkUrl(item.PackageLink),
                Price = item.Price
            };
            return viewModel;
        }

        private string GetLinkUrl(string linkString)
        {
            if (!string.IsNullOrEmpty(linkString))
            {
                LinkItemModel[] lnks = JsonConvert.DeserializeObject<LinkItemModel[]>(linkString);
                return lnks[0].href;
            }
            else
            {
                return null;
            }
        }
    }
}
