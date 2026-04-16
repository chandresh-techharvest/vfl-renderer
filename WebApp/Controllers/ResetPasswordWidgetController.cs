using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Progress.Sitefinity.RestSdk.OData;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using VFL.Renderer.Services.ResetPasswordService;
using VFL.Renderer.Models.ResetPassword;
using AngleSharp.Io;
using System.Linq;
using VFL.Renderer.Services.ResetPasswordService.Models;

namespace VFL.Renderer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ResetPasswordWidgetController : ControllerBase
    {
        private readonly IResetPasswordService _resetPasswordService;
        public ResetPasswordWidgetController(IResetPasswordService resetPasswordService, IODataRestClient restService)
        {
            _resetPasswordService = resetPasswordService;

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SendResetPasswordEmail([FromBody] ForgotPasswordRequest request)
        {

            if (!ModelState.IsValid)
            {
                throw new Exception("Model state is invalid");
                //return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            try
            {
                var response = await _resetPasswordService.SubmitRequest<ResetPasswordResponse>(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
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


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetUserPassword([FromBody] ResetPasswordRequest request)
        {

            if (!ModelState.IsValid)
            {
                throw new Exception("Model state is invalid");
                //return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            try
            {
                var rawPairs = request.SecurityToken.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries).Select(part => { var eqIndex = part.IndexOf('='); return new { Key = Uri.UnescapeDataString(part[..eqIndex]), Value = eqIndex > 0 ? Uri.UnescapeDataString(part[(eqIndex + 1)..]) : "" }; }).ToDictionary(x => x.Key, x => x.Value);
        
                string password = request.password;
                string token = rawPairs["token"];
                string user= rawPairs["user"];
           

                ResetPasswordVerifyRequest resetPasswordVerifyRequest = new ResetPasswordVerifyRequest()
                {
                    Password = password,
                    Token = token,
                    User = user

                };


                var response = await _resetPasswordService.ResetPassword<ResetPasswordResponse>(resetPasswordVerifyRequest);
                if (response.StatusCode == 0)
                {
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


        
    }

   
}



