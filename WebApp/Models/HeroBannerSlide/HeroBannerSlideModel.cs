using Newtonsoft.Json;
using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.Renderer.Entities.Content;
using Progress.Sitefinity.RestSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Dto;
using VFL.Renderer.Entities.HeroBannerSlide;
using VFL.Renderer.ViewModels.HeroBannerSlide;

namespace VFL.Renderer.Models.HeroBannerSlide
{
    public class HeroBannerSlideModel : IHeroBannerSlideModel   
    {
        private readonly IRestClient service;

        public HeroBannerSlideModel(IRestClient service)
        {
            this.service = service;
        }

        public async Task<IList<ItemViewModel>> GetViewModels(Entities.HeroBannerSlide.HeroBannerSlideEntity entity)
        {
            var response = await this.service.GetItems<HeroBannerSlideItem>(entity.HeroBannerSlides, new GetAllArgs() { Fields = new List<string>() { "Id", "Title", "SubTitle", "Description", "SlideImage", "SlideMobileImage", "ButtonText1", "ButtonLink1", "ButtonText2", "ButtonLink2", "VideoLink", "IsTextLight" } }).ConfigureAwait(true);
            return response.Items.Select(x => this.GetItemViewModel(x)).ToList();
            
        }

        private ItemViewModel GetItemViewModel(HeroBannerSlideItem item)
        {
            var viewModel = new ItemViewModel()
            {
                Title = item.Title,
                Description = item.Description,
                SlideImage = item.SlideImage != null && item.SlideImage.Count() > 0 ? item.SlideImage[0].Url : string.Empty,
                SlideMobileImage = item.SlideMobileImage != null && item.SlideMobileImage.Count() > 0 ? item.SlideMobileImage[0].Url : string.Empty,
                ButtonText1 = item.ButtonText1,
                ButtonLink1 = GetLinkUrl(item.ButtonLink1),
                ButtonText2 = item.ButtonText2,
                ButtonLink2 = GetLinkUrl(item.ButtonLink2),
                VideoLink = GetLinkUrl(item.VideoLink),
                SubTitle = item.SubTitle,
                IsTextLight = item.IsTextLight == "True" ? true : false,
            };



            return viewModel;
        }

        private string GetLinkUrl(string linkString)
        {
            if(!string.IsNullOrEmpty(linkString))
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
