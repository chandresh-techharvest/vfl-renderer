using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using Progress.Sitefinity.AspNetCore.Widgets.Models.LoginForm;
using System.Threading.Tasks;
using System;
using VFL.Renderer.Models.LoginForm;
using VFL.Renderer.Entities.LoginForm;
using VFL.Renderer.ViewModels.LoginForm;
using System.Net.Http;
using VFL.Renderer.Models.Profile;
using VFL.Renderer.Services.Registration;
using VFL.Renderer.Services.Registration.Models;
using Microsoft.AspNetCore.Http;
using VFL.Renderer.Services.LoginService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using VFL.Renderer.Config;
using Microsoft.Extensions.Options;
using Azure;

namespace VFL.Renderer.ViewComponents
{
    [SitefinityWidget(Title = "Custom Login form", Order = 0, Section = WidgetSection.Other, Category = WidgetCategory.Content, IconName = "login", NotPersonalizable = true)]
    [ViewComponent(Name = "CustomLoginForm")]
    public class CustomLoginFormViewComponent : ViewComponent
    {
        private ICustomLoginFormModel model;
        private readonly IRegistrationService _registrationService;
        private readonly IAuthService _authService;
        private readonly ApiSettings _apiSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;


        /// <summary>
        /// Initializes a new instance of the <see cref="LoginFormViewComponent"/> class.
        /// </summary>
        /// <param name="loginFormModel">The login form model.</param>
        public CustomLoginFormViewComponent(ICustomLoginFormModel loginFormModel, IOptions<ApiSettings> apiSettings, IRegistrationService registrationService, IAuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            this.model = loginFormModel;
            _registrationService = registrationService;
            _authService = authService;
            _apiSettings = apiSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Invokes the login form widget creation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<CustomLoginFormEntity> context)
        {
            var query = HttpContext.Request.Query;
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            CustomLoginFormViewModel viewModel = new CustomLoginFormViewModel();
            if (query.Count > 0)
            {
                var response = await _authService.SingleAccessLoginAsync<LoginResponse>(query["token"].ToString());
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK && response.data != null && response.data.isLoggedIn)
                {
                    if (response.data.isLoggedIn)
                    {
                        // Create claims
                        var claims = new[]
                        {
                    //new Claim(ClaimTypes.Name, request.username),
                    new Claim("access_token", response.data.jwtToken)
                };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        var cookieExpiry = DateTimeOffset.UtcNow.AddMinutes(_apiSettings.JwtExpiryMinutes);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                            new AuthenticationProperties
                            {
                                IsPersistent = true,
                                IssuedUtc = DateTime.UtcNow,
                                ExpiresUtc = cookieExpiry
                            });
                        viewModel = await this.model.InitializeViewModel(context.Entity);
                        _httpContextAccessor.HttpContext.Response.Redirect(viewModel.RedirectUrl);
                        return this.View(context.Entity.SfViewName, viewModel);
                    }
                    else
                    {
                        try
                        {
                            viewModel = await this.model.InitializeViewModel(context.Entity);
                        }
                        catch (HttpRequestException ex)
                        {
                            viewModel.ErrorMessage = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            viewModel.ErrorMessage = ex.Message;
                        }
                        var IsAuthenticated = HttpContext.User?.Identity?.IsAuthenticated ?? false;
                        if (IsAuthenticated)
                        {
                            HttpContext.Response.Redirect(viewModel.RedirectUrl);
                            //_httpContextAccessor.HttpContext.Response.Redirect(viewModel.RedirectUrl); 
                        }

                        return this.View(context.Entity.SfViewName, viewModel);
                    }
                }
                else
                {
                    viewModel = await this.model.InitializeViewModel(context.Entity);
                    viewModel.ErrorMessage = "Invalid or expired token. Please try logging in again.";
                    return this.View(context.Entity.SfViewName, viewModel);
                }
            }
            else
            {

                try
                {
                    viewModel = await this.model.InitializeViewModel(context.Entity);
                }
                catch (HttpRequestException ex)
                {
                    viewModel.ErrorMessage = ex.Message;
                }
                catch (Exception ex)
                {
                    viewModel.ErrorMessage = ex.Message;
                }
                var IsAuthenticated = HttpContext.User?.Identity?.IsAuthenticated ?? false;
                if (IsAuthenticated)
                {
                    bool isBackendUser = HttpContext.User.IsInRole("Administrators");
                    if (!isBackendUser)
                    {
                        HttpContext.Response.Redirect(viewModel.RedirectUrl);
                    }
                    //_httpContextAccessor.HttpContext.Response.Redirect(viewModel.RedirectUrl); 
                }

                return this.View(context.Entity.SfViewName, viewModel);
            }
        }
    }
}
