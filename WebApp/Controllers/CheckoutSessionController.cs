using Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Progress.Sitefinity.AspNetCore.Models;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Entities.Checkout;
using VFL.Renderer.Models.Checkout;
using VFL.Renderer.Models.Checkout.Models;
using VFL.Renderer.Models.WebTopUp;
using VFL.Renderer.Services.Plans;
using VFL.Renderer.Services.WebTopUp;
using VFL.Renderer.Services.WebTopUp.Models;
using VFL.Renderer.ViewModels.Checkout;
//using VFL.Renderer.Services.WebTopUp.Models;
namespace VFL.Renderer.Controllers
{
    [Route("api/checkoutsession")]
    [ApiController]
    public class CheckoutSessionController : ControllerBase
    {
        private readonly ICheckoutSessionModel _store;
        private readonly IWebTopUpService _webTopUpService;
        private readonly IPlansService _plansService;
        private readonly ILogger<CheckoutSessionController> _logger;

        public CheckoutSessionController(ICheckoutSessionModel store,WebTopUpService webTopUpService, IPlansService plansService, ILogger<CheckoutSessionController> logger)
        {
            _store = store;
            _webTopUpService = webTopUpService;
            _logger = logger;
            _plansService = plansService;
        }

        [HttpPost("create-session")]
        public IActionResult CreateSession([FromBody] CheckoutSessionRequest request)
        {
          

            var session = new CheckoutEntity
            {
                Email = request.Email,
                PhoneNumber = request.Phone,
    
                Amount = request.Amount,
                PageType = request.PageType,
                PageUrl = request.PageUrl
            };


            if (request.PageType == "purchaseplan")
            {
                
                session.PlanCodesJson = JsonConvert.SerializeObject(request.PlanCodes);
            }
            else if (request.PageType == "webtopup")
            {
               
                session.PlanCode = request.PlanCode;
            }

            _store.Save(session);

            return Ok(new { sessionId = session.Id });
        }



        [HttpPost("checkout-submit")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(
            [FromForm] string sid,
            [FromForm] CheckoutViewModel model)
        {
            // Load secure session
            var session = _store.Get(sid);

            if (session == null)
            {
                _logger.LogError("Null response received from payment service");

                return StatusCode(500, new WebTopUpResponse
                {
                    IsSuccess = false,
                    Message = "Checkout session expired"
                });
            }

            //  OVERWRITE all sensitive data
            model.Amount = session.Amount;
            model.Email = session.Email;
            model.PhoneNumber = session.PhoneNumber;
            model.PageUrl = session.PageUrl;
            model.PageType = session.PageType;

            if (model.PageType == "purchaseplan")
            {
                if (string.IsNullOrEmpty(session.PlanCodesJson))
                {
                    return BadRequest("PlanCodes missing in session");
                }

                var planCodes = JsonConvert.DeserializeObject<List<PlanCodeDto>>(session.PlanCodesJson);

                if (planCodes == null || !planCodes.Any())
                {
                    return BadRequest("Invalid planCodes data");
                }

                var paymentMethod = model.PaymentMethod == "CARD" ? "CREDIT" : model.PaymentMethod;

                var selectedPlan = planCodes
                    .FirstOrDefault(p => p.PaymentMethod == paymentMethod);

                if (selectedPlan == null)
                {
                    var validMethods = planCodes.Select(p => p.PaymentMethod == "CREDIT" ? "Visa or Mastercard" : "MPaisa").Distinct().ToList();
                    var validMethodDisplay = string.Join(" or ", validMethods);
                    return BadRequest($"The selected payment method is not valid for the chosen plan. Please select {validMethodDisplay} to continue.");
                }

                session.PlanCode = selectedPlan.Pcode;
            }
            else if (model.PageType == "webtopup")
            {
                // ✅ PlanCode already stored in session from CreateSession
                if (string.IsNullOrEmpty(session.PlanCode))
                {
                    return BadRequest("PlanCode missing for webtopup");
                }
            }



            var request = new WebTopUpRequest
            {
                Email = model.Email,
                Number = model.PhoneNumber,
                Amount = session.Amount.ToString(),
                planCode = session.PlanCode,
                PaymentInformationRequest =
                    BuildPaymentRequest(model)
            };
            try
            {

                bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
                if (model.PageType == "webtopup")
                {

                    //var response = await _webTopUpService.ProcessPayment<WebTopUpResponse>(request);
                    var response = isAuthenticated ? await _webTopUpService.ProcessPayment<WebTopUpResponse>(request) : await _webTopUpService.ProcessPaymentPublic<WebTopUpResponse>(request);
                    if (response == null)
                    {
                        _logger.LogError("Null response received from payment service");
                        return StatusCode(500, new WebTopUpResponse
                        {
                            IsSuccess = false,
                            Message = "Payment service error"
                        });
                    }
                    string redirectUrl = response.data.RedirectUrl;
                    return Ok(new
                    {
                        isSuccess = true,
                        data = new
                        {
                            url = redirectUrl,
                            PaymentResponse = response
                        }
                    });

                }
                else if (model.PageType == "purchaseplan"){

                    var response = isAuthenticated ? await _plansService.ProcessPayment<WebTopUpResponse>(request) : await _plansService.ProcessPaymentPublic<WebTopUpResponse>(request);
                    if (response == null)
                    {
                        _logger.LogError("Null response received from payment service");

                        return StatusCode(500, new WebTopUpResponse
                        {
                            IsSuccess = false,
                            Message = "Payment service error"
                        });
                    }
                    string redirectUrl = response.data.RedirectUrl;
                    return Ok(new
                    {
                        isSuccess = true,
                        data = new
                        {
                            url = redirectUrl,
                            PaymentResponse = response
                        }
                    });

                }
                else 
                {
                    _logger.LogWarning($"Invalid PageType received: {model.PageType}");

                    return BadRequest(new
                    {
                        isSuccess = false,
                        message = "Invalid page type"
                    });

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing WebTopUp payment");

                return StatusCode(500, new WebTopUpResponse
                {
                    IsSuccess = false,
                    Message = "Unexpected error occurred while processing payment"
                });
            }
        }













        private PaymentInformationRequest BuildPaymentRequest(CheckoutViewModel model)
        {
            if (model.PaymentMethod == "MPAISA")
            {
                return new PaymentInformationRequest
                {
                    MPaisaPayment = new MPaisaPayment
                    {
                        RedirectUrl = $"{Request.Scheme}://{Request.Host}{model.PageUrl}"
                    }
                };
            }

            return new PaymentInformationRequest
            {
                CreditCardPayment = new CreditCardPayment
                {
                    Billing_Firstname = model.FirstName,
                    Billing_Lastname = model.LastName,
                    Billing_Email = model.BillingEmail,
                    Billing_PhoneNumber = model.BillingPhone,
                    Billing_Address1 = model.Address1,
                    Billing_Address2 = model.Address2 ?? "",
                    Billing_Address3 = model.Address3 ?? "",
                    Billing_CountryCode = model.CountryCode,
                    Billing_State = model.State,
                    Billing_PostalCode = model.PostalCode,
                    Billing_City = model.City,
                    RedirectUrl = $"{Request.Scheme}://{Request.Host}{model.PageUrl}"
                }
            };
        }















        [HttpPost("provide-update")]
        public async Task<IActionResult> ProvideUpdate([FromBody] ProvideUpdateRequest request)
        {
            if (request == null)
                return BadRequest(new { isSuccess = false });
            bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            var result = isAuthenticated ? await _plansService.ProvideUpdate<ProvideUpdateResponse>(request) : await _plansService.ProvideUpdatePublic<ProvideUpdateResponse>(request);
            if (result == null || (result?.data?.IsSuccessful != true))
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Payment verification failed",
                    data = result.data
                });
            }
            return Ok(new
            {
                isSuccess = true,
                message = "Payment successful",
                data = result.data
            });
        }
    }







}








