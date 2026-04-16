using AngleSharp.Text;
using Progress.Sitefinity.RestSdk.Client;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VFL.Renderer.Dto;
using VFL.Renderer.Entities.Dashboard;
using VFL.Renderer.Services.Dashboard;
using VFL.Renderer.Services.Dashboard.Models;
using VFL.Renderer.Services.Profile;
using VFL.Renderer.Services.Profile.Models;
using VFL.Renderer.ViewModels.Dashboard;
using Progress.Sitefinity.RestSdk;
using Progress.Sitefinity.RestSdk.Dto;
using Progress.Sitefinity.AspNetCore.RestSdk;
using System.Collections.Generic;

namespace VFL.Renderer.Models.Dashboard
{
    public class DashboardModel : IDashboardModel
    {
        ProfileService _profileService;
        DashboardService _dashboardService;
        private readonly IRestClient restClient;

        public DashboardModel(ProfileService profileService, DashboardService dashboardService, IRestClient restClient) {
            _profileService = profileService;
            _dashboardService = dashboardService;
            this.restClient = restClient;
        }
        public async Task<DashboardViewModel> GetViewModels(DashboardEntity entity)
        {
            try
            {
                DashboardViewModel viewModel = new DashboardViewModel();
                if (entity.SliderImages != null)
                {
                    viewModel.SliderImages = new List<string>();
                    string[] ids = entity.SliderImages.ItemIdsOrdered;
                    foreach (var id in ids)
                    {
                        var img = await this.restClient.GetItem<ImageDto>(id);
                        if (img != null)
                        {
                            viewModel.SliderImages.Add(img.Url);
                        }
                    }
                }
                var profileInfo = await _profileService.GetProfileInformationAsync<ProfileSettingsResponse>();                
                if (profileInfo.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    viewModel.devices = profileInfo.data.devices;
                }
                return viewModel;
            }
            catch (System.Exception ex)
            {
                throw;
            }

        }
    }
}
