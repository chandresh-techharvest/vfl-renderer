//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Threading.Tasks;

//using VFL.Renderer.Services.WebTopUp;
//using VFL.Renderer.Services.WebTopUp.Models;

//namespace VFL.Renderer.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class WebTopUpController : ControllerBase
//    {

//        WebTopUpService _webTopUpService;
//        private readonly ILogger<WebTopUpController> _logger;



//        public WebTopUpController(WebTopUpService webTopUpService, ILogger<WebTopUpController> logger)
//        {
//            _webTopUpService = webTopUpService;
//            _logger = logger;
//        }

//        //[HttpPost]
//        //public async Task<IActionResult> ProcessPayment([FromBody] WebTopUpRequest request)
//        //{
//        //    if (request == null)
//        //    {
//        //        return BadRequest(new WebTopUpResponse
//        //        {
//        //            IsSuccess = false,
//        //            Message = "Invalid payment request"
//        //        });
//        //    }

//        //    try
//        //    {
//        //        var response = await _webTopUpService.ProcessPayment<WebTopUpResponse>(request);

//        //        if (response == null)
//        //        {
//        //            _logger.LogError("Null response received from payment service");

//        //            return StatusCode(500, new WebTopUpResponse
//        //            {
//        //                IsSuccess = false,
//        //                Message = "Payment service error"
//        //            });
//        //        }




//        //        return Ok(response);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _logger.LogError(ex, "Error while processing WebTopUp payment");

//        //        return StatusCode(500, new WebTopUpResponse
//        //        {
//        //            IsSuccess = false,
//        //            Message = "Unexpected error occurred while processing payment"
//        //        });
//        //    }
//        //}





//        //retrieve information

//        [HttpPost]
//        public async Task<IActionResult> ProcessPayment([FromBody] WebTopUpRequest request)
//        {
//            if (request == null)
//            {
//                return BadRequest(new WebTopUpResponse
//                {
//                    IsSuccess = false,
//                    Message = "Invalid payment request"
//                });
//            }

//            try
//            {
//                var response = await _webTopUpService.ProcessPayment<WebTopUpResponse>(request);

//                if (response == null)
//                {
//                    _logger.LogError("Null response received from payment service");

//                    return StatusCode(500, new WebTopUpResponse
//                    {
//                        IsSuccess = false,
//                        Message = "Payment service error"
//                    });
//                }



//                string redirectUrl =response.data.RedirectUrl;
//                return Ok(new
//                {
//                    isSuccess = true,
//                    data = new
//                    {
//                        url = redirectUrl,
//                        PaymentResponse = response
//                    }
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error while processing WebTopUp payment");

//                return StatusCode(500, new WebTopUpResponse
//                {
//                    IsSuccess = false,
//                    Message = "Unexpected error occurred while processing payment"
//                });
//            }
//         }



//        [HttpPost]
//        public async Task<IActionResult> ProvideUpdate([FromBody] ProvideUpdateRequest request)
//        {
//            if (request == null)
//                return BadRequest(new { isSuccess = false });

//            var result = await _webTopUpService.ProvideUpdate<ProvideUpdateResponse>(request);


//            if (result == null || !result.data.IsSuccessful)
//            {
//                return BadRequest(new
//                {
//                    isSuccess = false,
//                    message =  "Payment verification failed",
//                     data = result.data
//                });
//            }


//            return Ok(new
//            {
//                isSuccess = true,
//                message = "Payment successful",
//                data = result.data   
//            });
//        }























//    }
//}
