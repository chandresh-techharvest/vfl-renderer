using Azure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Services.Dashboard;
using VFL.Renderer.Services.Dashboard.Models;
using VFL.Renderer.Services.Profile;
using VFL.Renderer.Services.Profile.Models;
using VFL.Renderer.Config;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VFL.Renderer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        DashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;
        ProfileService _profileService;
        private readonly ApiSettings _apiSettings;

        public DashboardController(DashboardService dashboardService, IOptions<ApiSettings> apiSettings, ILogger<DashboardController> logger, ProfileService profileService)
        {
            _dashboardService = dashboardService;
            _logger = logger;
            _profileService = profileService;
            _apiSettings = apiSettings.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetBalanceInformation([FromBody] string value)
        {
            try
            {                
                DeviceGetRequestModel request = new DeviceGetRequestModel();
                request.number = value;
                var response = await _dashboardService.GetBalanceInformation<BalanceResponse>(request);
                if (response.data != null)
                {
                    //if (response.data.postpayBalances != null)
                    //{
                    //    for (int i = 0; i < response.data.postpayBalances.balances.Length; i++)
                    //    {
                    //        response.data.postpayBalances.balances[i].expiry = response.data.postpayBalances.balances[i].expiry;
                    //    }
                    //}

                    //if (response.data.prepayBalance != null)
                    //{
                    //    for (int i = 0; i < response.data.prepayBalance.balances.Length; i++)
                    //    {
                    //        response.data.prepayBalance.balances[i].expiry = response.data.prepayBalance.balances[i].expiry;
                    //    }
                    //}
                }
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var balanceInfo = response.data.postpayBalances;
                }
                else
                {
                    _logger.LogError("Failed to get balance information. Status Code: {StatusCode}", response);
                }

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"GetBalanceInformation Controller");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddDevice(DeviceRequest request)
        {
            try
            {
                request.oCresponse = EncryptionHelper.Decrypt(request.oCresponse);
                var response = await _dashboardService.AddDevice<DeviceResponse>(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"AddDevice Controller");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveDevice([FromBody] string value)
        {
            try
            {
                DeviceGetRequestModel request = new DeviceGetRequestModel();
                request.number = value;
                var response = await _dashboardService.RemoveDevice<DeviceResponse>(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"RemoveDevice Controller");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendOTPCode([FromBody] string value)
        {
            try
            {
                DeviceGetRequestModel request = new DeviceGetRequestModel();
                request.number = value;
                var response = await _dashboardService.SendOTPCode<DeviceResponse>(request);
                response.data.code = EncryptionHelper.Encrypt(response.data.code.ToString());
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"SendOTPCode Controller");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProfileInformation(string selectedNumber = null)
        {
            try
            {
                var response = await _profileService.GetProfileInformationAsync<ProfileSettingsResponse>();

                if (response?.data?.devices != null && response.data.devices.Length > 0)
                {

                    var cookieValue = CookieHelper.GetCookie(Request, "luData", true);
                    string cookieSelectedNumber = null;

                    if (!string.IsNullOrEmpty(cookieValue))
                    {
                        try
                        {
                            var cookieData = JsonConvert.DeserializeObject<ProfileSettingsResponse>(cookieValue);
                            cookieSelectedNumber = cookieData?.devices?
                                .FirstOrDefault(d => d.isSelected)?.number;
                        }
                        catch
                        {
                            // ignore invalid cookie
                        }
                    }
                 
                    string finalSelectedNumber = !string.IsNullOrEmpty(selectedNumber) ? selectedNumber : cookieSelectedNumber;


                    if (!string.IsNullOrEmpty(finalSelectedNumber))
                    {
                        foreach (var d in response.data.devices)
                        {
                            d.isSelected = d.number == finalSelectedNumber;
                        }
                    }
                    else if (!response.data.devices.Any(d => d.isSelected))
                    {
                        response.data.devices[0].isSelected = true;
                    }
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    bool isBackendUser = HttpContext.User.IsInRole("Administrators");
                    if (!isBackendUser)
                    {
                        HttpContext.Response.Redirect(_apiSettings.LoginPath);
                    }
                }
                //Response.Cookies.Delete("luData");
                if (response.data != null)
                {
                    var cookiedata = new
                    {
                        firstName = response.data.firstName,
                        lastName = response.data.lastName,
                        email = response.data.email,
                        devices = response.data.devices
                    };

                    CookieHelper.SetCookie(HttpContext.Response, "luData", JsonConvert.SerializeObject(cookiedata), true);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GetProfileInformation");
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult GetLuData()
        {
            var cookie = CookieHelper.GetCookie(Request, "luData", true);
            return Ok(cookie);
        }


    }
}
