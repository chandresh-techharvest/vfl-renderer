using System.Threading.Tasks;
using Progress.Sitefinity.AspNetCore.RestSdk;
using Progress.Sitefinity.AspNetCore.Web;
using Progress.Sitefinity.RestSdk;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using VFL.Renderer.Entities.MyBillLoginStatus;
using VFL.Renderer.ViewModels.MyBillLoginStatus;

namespace VFL.Renderer.Models.MyBillLoginStatus
{
    /// <summary>
    /// Model for MyBill Login Status widget
    /// Handles fetching configured page links from Sitefinity
    /// </summary>
    public class MyBillLoginStatusModel : IMyBillLoginStatusModel
    {
        private readonly IRestClient restService;
        private readonly IRenderContext renderContext;

        public MyBillLoginStatusModel(IRestClient restService, IRenderContext renderContext)
        {
            this.restService = restService;
            this.renderContext = renderContext;
        }

        public async Task<MyBillLoginStatusViewModel> GetViewModels(MyBillLoginStatusEntity entity)
        {
            var viewModel = new MyBillLoginStatusViewModel();

            if (entity.LoginPage.HasSelectedItems())
            {
                viewModel.LoginPage = await this.restService.GetItem<PageNodeDto>(entity.LoginPage);
            }

            if (entity.ProfilePage.HasSelectedItems())
            {
                viewModel.ProfilePage = await this.restService.GetItem<PageNodeDto>(entity.ProfilePage);
            }

            if (entity.MyBillDashboard.HasSelectedItems())
            {
                viewModel.MyBillDashboard = await this.restService.GetItem<PageNodeDto>(entity.MyBillDashboard);
            }

            if (entity.TransactionHistory.HasSelectedItems())
            {
                viewModel.TransactionHistory = await this.restService.GetItem<PageNodeDto>(entity.TransactionHistory);
            }

            if (entity.LogoutRedirectPage != null && entity.LogoutRedirectPage.HasSelectedItems())
            {
                var logoutPage = await this.restService.GetItem<PageNodeDto>(entity.LogoutRedirectPage);
                if (logoutPage != null)
                {
                    viewModel.LogoutRedirectUrl = "/" + logoutPage.UrlName.TrimStart('/');
                }
            }

            return viewModel;
        }
    }
}
