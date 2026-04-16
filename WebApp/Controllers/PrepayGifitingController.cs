using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Entities.PurchasePlan;
using VFL.Renderer.Services.Dashboard;
using VFL.Renderer.Services.Dashboard.Models;
using VFL.Renderer.Services.Plans;
using VFL.Renderer.Services.PrepayGifting;
using VFL.Renderer.Services.PrepayGifting.Models;
using VFL.Renderer.Services.Profile.Models;


namespace VFL.Renderer.Controllers
{


    
    [ApiController]
    [Route("PrepayGifiting")]
    public class PrepayGifitingController : ControllerBase
    {
        private readonly IPrepayGiftingService _prepayGiftingservice;
        private readonly ILogger<PrepayGifitingController> _logger;

        public PrepayGifitingController(IPrepayGiftingService prepayGiftingService)
        {
            _prepayGiftingservice = prepayGiftingService;
        }


      

        [HttpPost("SendOtpRequest")]
        public async Task<IActionResult> SendOtpRequest([FromBody] PrepayGiftingRequest request)
        {
            try
            {
                // Call external API (returns int)
                var apiResult =
                    await _prepayGiftingservice.SendOtpRequest<int>(request);

                if (apiResult == null )
                {
                    return BadRequest("OTP service failed");
                }

                // Map int → Response model
                var response = new ApiResponse<PrepayGiftingResponse>
                {
                  
                    data = new PrepayGiftingResponse
                    {
                        code = EncryptionHelper.Encrypt(
                                   apiResult.data.ToString()
                               ),
                        AllPlans = null // fill if needed
                    },
                    StatusCode = apiResult.StatusCode
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling SendOtpRequest");
                return StatusCode(500, "Internal Server Error");
            }
        }






      


        [HttpGet("GetPlanById")]
        public async Task<IActionResult> GetPlanById(int number ,int planId)
        {
           

           var result =
                await _prepayGiftingservice
                    .GetPlanByPlanId<PrepayGiftingResponse>(number, planId);

            return Ok(result?.data?.AllPlans?.FirstOrDefault());
        }




        [HttpPost("PrepayGiftSubscribe")]
        public async Task<IActionResult> PrepayGiftSubscribe(SubscribeRequest request)
        {


            try
            {
                request.oCresponse = EncryptionHelper.Decrypt(request.oCresponse);
                var result = await _prepayGiftingservice.PrepayGiftSubscribe<SubscribeData>(request);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"PrepayGifting Controller");
                throw;
            }


        }






        [HttpPost("GetAllPlans")]
        public async Task<IActionResult> GetAllPlans([FromBody] GetRealMoneyRequest request)
        {


            try
            {
                var result =
              await _prepayGiftingservice
                  .GetAllGiftingPlans<PrepayGiftingResponse>(request.Number);

                return Ok(result?.data?.AllPlans);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"PrepayGifting Controller");
                throw;
            }


        }



        [HttpGet("GetRealMoneyBalance")]
        public async Task<IActionResult> GetRealMoneyBalance(string number)
        {


            var result = await _prepayGiftingservice
                .GetRealMoneyBalance<decimal>(number);

            return Ok(result);
        }









































    }
}
