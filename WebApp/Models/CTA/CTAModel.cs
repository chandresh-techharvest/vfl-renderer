using Progress.Sitefinity.RestSdk.Client;
using Progress.Sitefinity.RestSdk.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using VFL.Renderer.Entities.CTA;
using VFL.Renderer.ViewModels.CTA;
using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.RestSdk;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using System.Linq;

namespace VFL.Renderer.Models.CTA
{
    public class CTAModel : ICTAModel
    {
        private readonly IRestClient restClient;
        public CTAModel(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<CTAViewModel> GetViewModels(CTAEntity entity)
        {
            CTAViewModel viewModel = new CTAViewModel();
            viewModel.PriceTitle = entity.PriceTitle;
            viewModel.TagTitle = entity.TagTitle;
            viewModel.TagTitleIconClass = entity.TagTitleIconClass;
            viewModel.MainTitle = entity.MainTitle;
            viewModel.Summary = entity.Summary;
            viewModel.CTAText = entity.CTAText;
            if (entity.CTAImage1 != null)
            {
                viewModel.CTAImage1 = new List<ImageDto>();
                string[] ids = entity.CTAImage1.ItemIdsOrdered;
                if (ids != null && ids.Count() > 0)
                {
                    foreach (string id in ids)
                    {
                        var img = await this.restClient.GetItem<ImageDto>(id);
                        if (img != null)
                        {
                            viewModel.CTAImage1.Add(img);
                        }
                    }
                }
            }
            if (entity.CTAImage2 != null)
            {
                viewModel.CTAImage2 = new List<ImageDto>();
                string[] ids = entity.CTAImage2.ItemIdsOrdered;
                if (ids != null && ids.Count() > 0)
                {
                    foreach (string id in ids)
                    {
                        var img = await this.restClient.GetItem<ImageDto>(id);
                        if (img != null)
                        {
                            viewModel.CTAImage2.Add(img);
                        }
                    }
                }
            }

            if (entity.PriceIconImage != null)
            {
                viewModel.PriceIconImage = new List<ImageDto>();
                string[] ids = entity.PriceIconImage.ItemIdsOrdered;
                if (ids != null && ids.Count() > 0)
                {
                    foreach (string id in ids)
                    {
                        var img = await this.restClient.GetItem<ImageDto>(id);
                        if (img != null)
                        {
                            viewModel.PriceIconImage.Add(img);
                        }
                    }
                }
            }
            if (entity.CTALink != null)
            {
                viewModel.CTALink = entity.CTALink;
            }

            return viewModel;
        }
    }
}
