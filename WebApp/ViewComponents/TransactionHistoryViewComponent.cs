using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System.Threading.Tasks;
using VFL.Renderer.Config;
using VFL.Renderer.Entities.TransactionHistory;


namespace VFL.Renderer.ViewComponents
{
    [SitefinityWidget(Title = "Transaction History", Order = 1, Category = WidgetCategory.Content, IconName = "content-section", NotPersonalizable = true)]
    [ViewComponent(Name = "TransactionHistory")]
    public class TransactionHistoryViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiSettings _apiSettings;
        public TransactionHistoryViewComponent(IOptions<ApiSettings> options, IHttpContextAccessor httpContextAccessor) {
            this._httpContextAccessor = httpContextAccessor;
            _apiSettings = options.Value;
        }
        public virtual async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<TransactionHistoryEntity> context)
        {
            if (!User.Identity.IsAuthenticated)
                _httpContextAccessor.HttpContext.Response.Redirect(_apiSettings.LoginPath);


            //var viewModels = await this.model.InitializeViewModel(context.Entity);
            return View(context.Entity.ViewName);
        }
    }
}
