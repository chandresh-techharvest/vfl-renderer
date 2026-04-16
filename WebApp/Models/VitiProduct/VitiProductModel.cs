using Progress.Sitefinity.RestSdk;
using System.Collections.Generic;
using System.Threading.Tasks;
using VFL.Renderer.Entities.VitiProduct;
using VFL.Renderer.ViewModels.VitiProduct;
using VFL.Renderer.Dto;
using System;
using Newtonsoft.Json;
using Progress.Sitefinity.AspNetCore.RestSdk;
using System.Linq;

namespace VFL.Renderer.Models.VitiProduct
{
    public class VitiProductModel : IVitiProductModel
    {
        private readonly IRestClient service;
        public VitiProductModel(IRestClient service)
        {
            this.service = service;
        }


        public async Task<IList<ProductViewModel>> GetViewModels(VitiProductEntity entity)
        {
            var response = await this.service.GetItems<VitiProductItem>(entity.VitiProducts, new GetAllArgs() { Fields = new List<string>() { "Id", "Title", "Description", "ProductImage", "Brand", "Price", "BuyNowLink"} }).ConfigureAwait(true);
            return response.Items.Select(x => this.GetItemViewModel(x)).ToList();
        }


        private ProductViewModel GetItemViewModel(VitiProductItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var viewModel = new ProductViewModel()
            {
                Title = item.Title,
                Description = item.Description,
                ProductImage = (item.ProductImage != null && item.ProductImage.Count() > 0) ? item.ProductImage[0].Url : "",
                Brand = item.Brand,
                BuyNowLink = GetLinkUrl(item.BuyNowLink),
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
