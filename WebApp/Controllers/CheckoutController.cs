//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;
//using System.Threading.Tasks;
//using VFL.Renderer.Models.Checkout;
//using VFL.Renderer.Models.WebTopUp;
//using VFL.Renderer.Services.WebTopUp;
//using VFL.Renderer.Services.WebTopUp.Models;
////using VFL.Renderer.Services.WebTopUp.Models;
//namespace VFL.Renderer.Controllers
//{
//    //[Route("api/[controller]/[action]")]
//    //[ApiController]
//    public class CheckoutController : Controller
//    {
//        private readonly ICheckoutSessionStore _sessionStore;
//        private readonly WebTopUpService _webTopUpService;

//        public CheckoutController(
//            ICheckoutSessionStore sessionStore,
//            WebTopUpService webTopUpService)
//        {
//            _sessionStore = sessionStore;
//            _webTopUpService = webTopUpService;
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Submit(
//            string sid,
//            CheckoutViewModel model)
//        {
//            // 1. Load secure session
//            var session = _sessionStore.Get(sid);

//            if (session == null)
//                return BadRequest("Checkout session expired");

//            // 2. OVERWRITE all sensitive data
//            model.Amount = session.Amount;
//            model.Email = session.Email;
//            model.PhoneNumber = session.PhoneNumber;

            
//                var request = new WebTopUpRequest
//                {
//                    Email = model.Email,
//                    Number = model.PhoneNumber,
//                    Amount = session.Amount.ToString(),
//                    PaymentInformationRequest =
//                        BuildPaymentRequest(model)
//                };

//                var response =
//                    await _webTopUpService
//                        .ProcessPayment<WebTopUpResponse>(request);

//                if (response?.data?.RedirectUrl == null)
//                    return BadRequest("Payment failed");

//                // Optional: invalidate session
//                _sessionStore.Remove(sid);

//            //return Redirect(response.data.RedirectUrl);
//            return Json(new
//            {
//                redirectUrl = response.data.RedirectUrl
//            });


//            // future products here
//            // return BadRequest("Unsupported product");
//        }

//        private PaymentInformationRequest BuildPaymentRequest(CheckoutViewModel model)
//        {
//            if (model.PaymentMethod == "MPAISA")
//            {
//                return new PaymentInformationRequest
//                {
//                    MPaisaPayment = new MPaisaPayment
//                    {
//                        RedirectUrl = $"{Request.Scheme}://{Request.Host}/webtopup"
//                    }
//                };
//            }

//            return new PaymentInformationRequest
//            {
//                CreditCardPayment = new CreditCardPayment
//                {
//                    Billing_Firstname = model.FirstName,
//                    Billing_Lastname = model.LastName,
//                    Billing_Email = model.BillingEmail,
//                    Billing_PhoneNumber = model.BillingPhone,
//                    Billing_Address1 = model.Address1,
//                    Billing_Address2 = model.Address2,
//                    Billing_CountryCode = model.CountryCode,
//                    Billing_State = model.State,
//                    Billing_PostalCode = model.PostalCode,
//                    Billing_City = model.City,
//                    RedirectUrl = $"{Request.Scheme}://{Request.Host}/main/webtopup"
//                }
//            };
//        }
//    }

//}
