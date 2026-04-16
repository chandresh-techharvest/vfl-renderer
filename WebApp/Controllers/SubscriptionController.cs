using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Common;

using VFL.Renderer.Services.Subscription;
using VFL.Renderer.Services.Subscription.Models;

namespace VFL.Renderer.Controllers
{


    [ApiController]
    [Route("Subscription")]
    public class SubscriptionController : ControllerBase
    {
        private  readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(ISubscriptionService subscriptionService) { 
        
        _subscriptionService = subscriptionService;
        }



        [HttpPost("SendOtpRequest")]
        public async Task<IActionResult> SendOtpRequest([FromBody] SendOtpRequest request)
        {
            try
            {
                // Call external API (returns int)
                var apiResult =
                    await _subscriptionService.SendOtpRequest<int>(request);

                if (apiResult == null)
                {
                    return BadRequest("OTP service failed");
                }

                // Map int → Response model
                var response = new ApiResponse<SubscriptionResponse>
                {

                    data = new SubscriptionResponse
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




        [HttpPost("Subscribe")]
        public async Task<IActionResult> Subscribe(SubscriptionRequest request)
        {


            try
            {
                if (!string.IsNullOrEmpty(request.oCresponse))
                {
                    request.oCresponse = EncryptionHelper.Decrypt(request.oCresponse);
                }
                var result = await _subscriptionService.Subscribe<SubscribeData>(request);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"Subscription Controller");
                throw;
            }


        }



      















        [HttpPost("Resubscribe")]
        public async Task<IActionResult> Resubscribe(SubscriptionRequest request)
        {


            try
            {
                request.oCresponse = EncryptionHelper.Decrypt(request.oCresponse);
                var result = await _subscriptionService.Resubscribe<SubscribeData>(request);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"Subscription Controller");
                throw;
            }


        }



        [HttpPost("Unsubscribe")]
        public async Task<IActionResult> Unsubscribe(SubscriptionRequest request)
        {


            try
            {
                request.oCresponse = EncryptionHelper.Decrypt(request.oCresponse);
                var result = await _subscriptionService.Unsubscribe<SubscribeData>(request);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"Subscription Controller");
                throw;
            }


        }






        [HttpGet("GetPlanById")]
        public async Task<IActionResult> GetPlanById(int number, int planId)
        {
            var result =
                await _subscriptionService.GetPlanByPlanId<SubscriptionResponse>( number,  planId); ;

            return Ok(result?.data?.AllPlans?.FirstOrDefault());
        }








        [HttpGet("GetPlansByType")]
        public async Task<IActionResult> GetPlansByType(int number, string planType)
        {
            var result =
                await _subscriptionService.GetAllSubscriptionPlansByPlanType<SubscriptionResponse>(number, planType);

            return Ok(result?.data?.AllPlans);
        }




        [HttpGet("GetAllPlans")]
        public async Task<IActionResult> GetAllPlans(int number)
        {
            var result = await _subscriptionService.GetAllSubscriptionPlans<SubscriptionResponse>(number);

            return Ok(result?.data?.AllPlans);
        }














    }
}
