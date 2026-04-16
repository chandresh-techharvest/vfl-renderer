using Microsoft.AspNetCore.Mvc;
using Progress.Sitefinity.RestSdk.OData;
using System.Net.Http;
using System.Net;
using VFL.Renderer.Services;
using VFL.Renderer.Services.LoginService;
using System.Threading.Tasks;
using VFL.Renderer.Models.LoginForm;
using Newtonsoft.Json;
using VFL.Renderer.Extensions;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System;
using VFL.Renderer.Services.Registration;
using VFL.Renderer.Services.Registration.Models;
//using Microsoft.AspNetCore.Authentication.Google;
using VFL.Renderer.Config;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using VFL.Renderer.Common;
using Azure;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


[Route("api/[controller]/[action]")]
[ApiController]
public class LoginFormController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly SitefinityServiceClient _sitefinityClient;
    private readonly IRegistrationService _registrationService;
    private readonly ApiSettings _apiSettings;
    public LoginFormController(IAuthService authService, IODataRestClient restService, IOptions<ApiSettings> apiSettings, SitefinityServiceClient sitefinityClient, IRegistrationService registrationService)
    {
        _authService = authService;
        this.restService = restService;
        _sitefinityClient = sitefinityClient;
        _registrationService = registrationService;
        _apiSettings = apiSettings.Value;
    }

    // POST api/<LoginFormController>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await ClearSessionAsync(true);
        var response = await _authService.LoginAsync<LoginResponse>(request);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (response.data.isLoggedIn)
            {
                // Create claims
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.username),
                    new Claim("access_token", response.data.jwtToken)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                //var cookieOnlineServicesRefreshToken = CookieHelper.GetCookie(Request, "OnlineServicesRefreshToken", true);
                //var Expires = ParseCookieValue(cookieOnlineServicesRefreshToken, "expires");
                //TimeSpan expiresmin = DateTime.Parse(Expires) - DateTime.Now;
                var cookieExpiry = DateTimeOffset.UtcNow.AddMinutes(_apiSettings.RefreshTokenExpiryMinutes);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        IssuedUtc = DateTime.UtcNow,
                        ExpiresUtc = cookieExpiry
                    });
                return Ok(response);
            }
            else
            {
                return Unauthorized(response);
            }
        }
        else
        {
            return BadRequest(response);
        }
    }
    private string ParseCookieValue(string setCookieHeader, string cookieName)
    {
        string cookieValue = string.Empty;
        foreach (string item in setCookieHeader.Split(';'))
        {
            if (item.Contains(cookieName))
            {
                var parts = item.Split('=');
                cookieValue = parts[0].Trim() == cookieName ? parts[1] : null;
                break;
            }
        }
        return cookieValue;
    }
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await ClearSessionAsync(true);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/api/LoginForm/GoogleCallback")]
    public async Task<IActionResult> GoogleCallback([FromBody] GoogleTokenRequest request)
    {
        if (string.IsNullOrEmpty(request?.IdToken))
            return Ok(new { success = false, error = "No token provided" });        

        //Get Google access token
        var accessToken = request.IdToken;

        // Call your external API with the Google token
        var apiResponse = await _authService.ExternalLoginAsync<LoginResponse>("Google", accessToken);

        if (apiResponse.StatusCode != HttpStatusCode.OK || apiResponse.data == null || !apiResponse.data.isLoggedIn)
        {
            return Redirect("/login?error=external_api_failed");
        }

        // Create claims and sign in locally
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, apiResponse.data.username ?? "GoogleUser"),
            new Claim("access_token", apiResponse.data.jwtToken)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.Now.AddDays(15)
            });

        return Ok(apiResponse);
    }

    //public async Task<IActionResult> GoogleLogin(string Token)
    //{
    //    var accessToken = User.FindFirst("access_token")?.Value;
    //    if (string.IsNullOrEmpty(accessToken))
    //    {
    //        return Unauthorized();
    //    }
    //    var data = await _authService.ExternalLoginAsync("Google", accessToken);
    //    return Ok(data);
    //}

   

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
            Response.Cookies.Append("OnlineServicesRefreshToken", "", expiredOptions);
            Response.Cookies.Append("luData", "", expiredOptions);
            //Response.Cookies.Append(".MyBill.Session", "", expiredOptions);
        }
        else
        {
            Response.Cookies.Delete("luData");
            Response.Cookies.Delete("OnlineServicesRefreshToken");
        }

        // Clear session
        HttpContext.Session.Clear();
    }

    private readonly IODataRestClient restService;
}

