using Progress.Sitefinity.RestSdk;
using Progress.Sitefinity.RestSdk.Client;
using Progress.Sitefinity.RestSdk.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillDashboard;
using VFL.Renderer.Services.MyBillProfile;
using VFL.Renderer.Services.MyBillProfile.Models;
using VFL.Renderer.ViewModels.MyBillDashboard;

namespace VFL.Renderer.Models.MyBillDashboard
{
    /// <summary>
    /// Model for MyBill Dashboard widget - orchestrates data from multiple sources
    /// </summary>
    public class MyBillDashboardModel : IMyBillDashboardModel
    {
        private readonly IMyBillProfileService _myBillProfileService;
        private readonly IRestClient _restClient;

        public MyBillDashboardModel(IMyBillProfileService myBillProfileService, IRestClient restClient)
        {
            _myBillProfileService = myBillProfileService;
            _restClient = restClient;
        }

        /// <summary>
        /// Gets the view models by orchestrating data from Sitefinity and MyBill backend
        /// Includes preview data for Sitefinity designer mode
        /// </summary>
        public async Task<MyBillDashboardViewModel> GetViewModels(MyBillDashboardEntity entity)
        {
            try
            {
                MyBillDashboardViewModel viewModel = new MyBillDashboardViewModel();

                // 1. Get slider images from Sitefinity media library
                if (entity.SliderImages != null && entity.SliderImages.ItemIdsOrdered != null)
                {
                    viewModel.SliderImages = new List<string>();
                    string[] ids = entity.SliderImages.ItemIdsOrdered;
                    foreach (var id in ids)
                    {
                        var img = await _restClient.GetItem<ImageDto>(id);
                        if (img != null)
                        {
                            viewModel.SliderImages.Add(img.Url);
                        }
                    }
                }

                // 2. BAN account data is loaded entirely client-side by the dashboard
                //    JavaScript (via /api/MyBillDashboard/GetAllAccountsByPrimary).
                //    During SSR we only need preview data for the Sitefinity designer;
                //    the real data will replace it once the JS initializes.
                viewModel.BanAccounts = GetPreviewBanAccounts();

                return viewModel;
            }
            catch (System.Exception ex)
            {
                // Log exception and return view model with preview data
                var viewModel = new MyBillDashboardViewModel
                {
                    BanAccounts = GetPreviewBanAccounts()
                };
                return viewModel;
            }
        }

        /// <summary>
        /// Returns preview BAN accounts for Sitefinity designer mode
        /// </summary>
        private ViewModels.MyBillDashboard.BanAccount[] GetPreviewBanAccounts()
        {
            return new[]
            {
                new ViewModels.MyBillDashboard.BanAccount
                {
                    Number = "945939845",
                    Name = "Main Corporate Account",
                    IsSelected = true
                },
                new ViewModels.MyBillDashboard.BanAccount
                {
                    Number = "987654321",
                    Name = "Branch Office Account",
                    IsSelected = false
                }
            };
        }
    }
}
