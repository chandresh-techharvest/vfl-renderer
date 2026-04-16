using Azure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vfl.Renderer.Entities.LoginStatus;
using Vfl.Renderer.Models.LoginStatus;
using VFL.Renderer.Services.Profile;
using VFL.Renderer.Services.Profile.Models;
using VFL.Renderer.Config;
using Microsoft.Extensions.Options;

namespace Vfl.Renderer.ViewComponents
{
    /// <summary>
    /// The view component for accessing Sitefinity data.
    /// </summary>
    [SitefinityWidget]
    public class LoginStatusViewComponent : ViewComponent
    {
        private ILoginStatusModel model;
        private IProfileService _profileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiSettings _apiSettings;


        /// <summary>
        /// Initializes a new instance of the <see cref="LoginStatusViewComponent"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public LoginStatusViewComponent(ILoginStatusModel model, IOptions<ApiSettings> options, IProfileService profileService, IHttpContextAccessor httpContextAccessor)
        {
            this.model = model;
            _httpContextAccessor = httpContextAccessor;
            _profileService = profileService;
            _apiSettings = options.Value;
        }

        /// <summary>
        /// Invokes the view component.
        /// </summary>
        /// <param name="context">The view component context.</param>
        /// <returns>The view component result.</returns>
        public async Task<IViewComponentResult> InvokeAsync(IViewComponentContext<LoginStatusEntity> context)
        {
            bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            var viewModels = await this.model.GetViewModels(context.Entity);
            if (isAuthenticated)
            {
                string authCookieValue = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                viewModels.Name = authCookieValue;
                try
                {
                    var profileViewModel = await _profileService.GetProfileInformationAsync<ProfileSettingsResponse>();
                    if (profileViewModel.data != null)
                    {
                        viewModels.ProfileImageUrl = profileViewModel.data.profileImageData;
                        viewModels.Name = profileViewModel.data.firstName + " " + profileViewModel.data.lastName;
                    }
                    else if(profileViewModel.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        var isEditMode = HttpContext.Request.Query.ContainsKey("sfaction") && HttpContext.Request.Query["sfaction"] == "edit";
                        if (isEditMode)
                        {
                            viewModels.Warning = "The user is unauthorized. Please log in to see the profile information.";
                        }
                        else
                        {
                            await ClearSessionAsync(false);

                            bool isBackendUser = HttpContext.User.IsInRole("Administrators");
                            if (!isBackendUser)
                            {

                                _httpContextAccessor.HttpContext.Response.Redirect("/login");
                            }


                        }
                    }

                    //if (HttpContext.Request.Cookies["luData"] == null)
                    //{
                    //    var cookieValue = JsonConvert.SerializeObject(profileViewModel.data);
                    //    HttpContext.Response.Cookies.Append(
                    //    "luData",
                    //    JsonConvert.SerializeObject(profileViewModel.data),
                    //    new CookieOptions
                    //    {
                    //        Expires = DateTimeOffset.UtcNow.AddDays(7), // Set expiration
                    //        HttpOnly = false,   // Cannot be accessed via client-side script
                    //        Secure = false,     // Sent over HTTPS only
                    //        SameSite = SameSiteMode.None, // Protects against CSRF
                    //        Domain = HttpContext.Request.Host.Host
                    //    }
                    //);
                    //}                   
                }
                catch (System.Exception ex)
                {

                }
                
                
            }
            return this.View(context.Entity.ViewName, viewModels);
        }

        private async Task ClearSessionAsync(bool forceExpireCookies = false)
        {
            // Clear cookies
            if (forceExpireCookies)
            {
                var expiredOptions = new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(-1)
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("OnlineServicesRefreshToken", "", expiredOptions);
                _httpContextAccessor.HttpContext.Response.Cookies.Append("luData", "", expiredOptions);
                //Response.Cookies.Append(".MyBill.Session", "", expiredOptions);
            }
            else
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete("luData");
                _httpContextAccessor.HttpContext.Response.Cookies.Delete("OnlineServicesRefreshToken");
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Clear session
            HttpContext.Session.Clear();
        }
    }
}
