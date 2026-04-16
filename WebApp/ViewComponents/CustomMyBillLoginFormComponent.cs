using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillLoginForm;
using VFL.Renderer.Models.MyBillLoginForm;
using VFL.Renderer.ViewModels.MyBillLoginForm;
using Microsoft.Extensions.Logging;

namespace VFL.Renderer.ViewComponents
{
    /// <summary>
    /// View component for MyBill Login form widget
    /// Separate from MyVodafone login form
    /// </summary>
    [SitefinityWidget(
        Title = "MyBill Login form", 
        Order = 0, 
        Section = WidgetSection.Other, 
        Category = WidgetCategory.Content, 
        IconName = "login", 
        NotPersonalizable = true)]
    [ViewComponent(Name = "CustomMyBillLoginForm")]
    public class CustomMyBillLoginFormComponent : ViewComponent
    {
        private readonly ICustomMyBillLoginModel model;
        private readonly ILogger<CustomMyBillLoginFormComponent> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMyBillLoginFormComponent"/> class.
        /// </summary>
        /// <param name="loginFormModel">The MyBill login form model.</param>
        /// <param name="logger">The logger.</param>
        public CustomMyBillLoginFormComponent(
            ICustomMyBillLoginModel loginFormModel,
            ILogger<CustomMyBillLoginFormComponent> logger)
        {
            this.model = loginFormModel;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the MyBill login form widget creation.
        /// </summary>
        /// <param name="context">The context containing the entity configuration.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<MyBillLoginFormEntity> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // NOTE: Redirect for already-authenticated users is handled by the middleware in Program.cs.
            // The middleware checks for a valid MyBill session and redirects to the dashboard
            // before Sitefinity's rendering pipeline even processes the page.

            // Show login form
            MyBillLoginFormViewModel loginViewModel = new MyBillLoginFormViewModel();

            try
            {
                loginViewModel = await this.model.InitializeViewModel(context.Entity);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "MyBill LoginForm: HTTP error initializing view model");
                loginViewModel.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill LoginForm: Error initializing view model");
                loginViewModel.ErrorMessage = ex.Message;
            }

            return this.View(context.Entity.SfViewName, loginViewModel);
        }
    }
}

