using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using VFL.Renderer.Services.MyBillResetPasswordService;
using VFL.Renderer.Models.MyBillResetPassword;
using System.Linq;
using VFL.Renderer.Services.MyBillResetPasswordService.Models;

namespace VFL.Renderer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MyBillResetPasswordController : ControllerBase
    {
        private readonly IMyBillResetPasswordService _myBillResetPasswordService;
        private readonly ILogger<MyBillResetPasswordController> _logger;

        public MyBillResetPasswordController(IMyBillResetPasswordService myBillResetPasswordService, ILogger<MyBillResetPasswordController> logger)
        {
            _myBillResetPasswordService = myBillResetPasswordService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordEmail([FromBody] MyBillForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { title = "Validation Error", detail = "Model state is invalid" });
            }

            try
            {
                var response = await _myBillResetPasswordService.SubmitRequest<MyBillSubmitRequestResponse>(request);
                
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(response);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return StatusCode(401, new 
                    { 
                        title = "Unauthorized",
                        status = 401,
                        detail = "Unable to find account with this BAN",
                        data = response.data
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        title = "Request Failed",
                        status = (int)response.StatusCode,
                        detail = "Failed to send reset password email",
                        data = response.data
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendResetPasswordEmail");
                return StatusCode(500, new 
                { 
                    title = "Internal Server Error",
                    detail = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetUserPassword([FromBody] MyBillResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { title = "Validation Error", detail = "Model state is invalid" });
            }

            try
            {
                // Parse query string to get token and username
                var rawPairs = request.SecurityToken.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries)
                    .Select(part =>
                    {
                        var eqIndex = part.IndexOf('=');
                        return new
                        {
                            Key = Uri.UnescapeDataString(part[..eqIndex]),
                            Value = eqIndex > 0 ? Uri.UnescapeDataString(part[(eqIndex + 1)..]) : ""
                        };
                    })
                    .ToDictionary(x => x.Key, x => x.Value);

                string password = request.password;
                string token = rawPairs.ContainsKey("token") ? rawPairs["token"] : string.Empty;
                string username = rawPairs.ContainsKey("user") ? rawPairs["user"] : string.Empty;

                MyBillResetPasswordVerifyRequest resetPasswordVerifyRequest = new MyBillResetPasswordVerifyRequest()
                {
                    password = password,
                    token = token,
                    username = username
                };

                var response = await _myBillResetPasswordService.ResetPassword<MyBillResetPasswordResponse>(resetPasswordVerifyRequest);
                
                if (response.StatusCode == HttpStatusCode.OK && response.data != null && response.data.data?.isReset == true)
                {
                    return Ok(response);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return BadRequest(new
                    {
                        title = "Validation Error",
                        status = 400,
                        detail = "Password validation failed",
                        data = response.data
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        title = "Request Failed",
                        status = (int)response.StatusCode,
                        detail = "Failed to reset password",
                        data = response.data
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetUserPassword");
                return StatusCode(500, new 
                { 
                    title = "Internal Server Error",
                    detail = ex.Message
                });
            }
        }
    }
}
