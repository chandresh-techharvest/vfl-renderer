using AngleSharp.Dom;
using AngleSharp.Io;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.Profile;
using VFL.Renderer.Services.Profile.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VFL.Renderer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileSettingsController : ControllerBase
    {
        ProfileService _profileService;
        protected readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProfileSettingsController(ProfileService profileService,HttpClient httpClient, IHttpContextAccessor httpContextAccessor) {
            _profileService = profileService;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/<ProfileSettingsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _profileService.GetProfileInformationAsync<ProfileSettingsResponse>();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(response);
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var httpContext = _httpContextAccessor.HttpContext;
                    httpContext.Response.Redirect("/login");
                    return Unauthorized(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = new StringContent(JsonConvert.SerializeObject(ex));
                return BadRequest(responseMessage);
            }
        }       

        // POST api/<ProfileSettingsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] EditProfileRequest request)
        {
            try
            {
                var response = await _profileService.EditProfileInformationAsync<EditProfileResponse>(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.data != null)
                    {
                        var cookiedata = new
                        {
                            firstName = request.FirstName,
                            lastName = request.LastName,
                            email = request.Email                            
                        };

                        CookieHelper.SetCookie(HttpContext.Response, "luData", JsonConvert.SerializeObject(cookiedata), true);
                    }
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = new StringContent(JsonConvert.SerializeObject(ex));
                return BadRequest(responseMessage);
            }

        }

       
        // PUT api/<ProfileSettingsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<ProfileSettingsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
