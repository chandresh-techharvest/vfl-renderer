using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Models.MyBillCheckout;
using VFL.Renderer.Services.MyBillPayment;
using VFL.Renderer.Services.MyBillPayment.Models;
using VFL.Renderer.Services.WebTopUp.Models;
using VFL.Renderer.ViewModels.MyBillCheckout;

namespace VFL.Renderer.Controllers
{
    /// <summary>
    /// API controller for MyBill checkout and payment operations
    /// All endpoints require MyBillAuth authentication
    /// </summary>
    [Route("api/mybill-checkout")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "MyBillAuth")]
    public class MyBillCheckoutController : ControllerBase
    {
        private readonly ICheckoutSessionStore _store;
        private readonly IMyBillPaymentService _myBillPaymentService;
        private readonly ILogger<MyBillCheckoutController> _logger;

        public MyBillCheckoutController(
            ICheckoutSessionStore store,
            IMyBillPaymentService myBillPaymentService,
            ILogger<MyBillCheckoutController> logger)
        {
            _store = store;
            _myBillPaymentService = myBillPaymentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new checkout session for MyBill payment
        /// </summary>
        [HttpPost("create-session")]
        public IActionResult CreateSession([FromBody] CreateMyBillCheckoutSessionRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("CreateSession called with null request");
                    return BadRequest(new { isSuccess = false, message = "Invalid request" });
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { isSuccess = false, message = "Email is required" });
                }

                if (string.IsNullOrEmpty(request.BanNumber))
                {
                    return BadRequest(new { isSuccess = false, message = "BAN number is required" });
                }

                if (string.IsNullOrEmpty(request.InvoiceNumber))
                {
                    return BadRequest(new { isSuccess = false, message = "Invoice number is required" });
                }

                if (request.PaymentAmount <= 0)
                {
                    return BadRequest(new { isSuccess = false, message = "Payment amount must be greater than 0" });
                }

                if (request.PaymentAmount > request.FullAmount)
                {
                    return BadRequest(new { isSuccess = false, message = "Payment amount cannot exceed total amount" });
                }

                // Validate partial payment minimum ($10)
                if (request.IsPartialPayment && request.PaymentAmount < 10)
                {
                    return BadRequest(new { isSuccess = false, message = "Minimum partial payment is $10" });
                }

                _logger.LogInformation(
                    "Creating checkout session for Invoice {Invoice}, BAN {BAN}, Amount {Amount}", 
                    request.InvoiceNumber, 
                    request.BanNumber,
                    request.PaymentAmount);

                // Create secure session
                var session = new MyBillCheckoutSession
                {
                    Email = request.Email,
                    BanNumber = request.BanNumber,
                    InvoiceNumber = request.InvoiceNumber,
                    FullAmount = request.FullAmount,
                    PaymentAmount = request.PaymentAmount,
                    IsPartialPayment = request.IsPartialPayment,
                    PageUrl = request.PageUrl,
                    PageType = "MyBillPayment"
                };

                _store.Save(session);

                _logger.LogInformation("Checkout session created with ID: {SessionId}", session.Id);

                return Ok(new { sessionId = session.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkout session");
                return StatusCode(500, new { isSuccess = false, message = "Error creating checkout session" });
            }
        }

        /// <summary>
        /// Submit payment for processing
        /// </summary>
        [HttpPost("submit")]
        public async Task<IActionResult> Submit(
            [FromForm] string sid, 
            [FromForm] MyBillCheckoutViewModel model,
            [FromForm] decimal? PaymentAmount,
            [FromForm] bool? IsPartialPayment)
        {
            try
            {
                // Load secure session
                var session = _store.Get(sid) as MyBillCheckoutSession;

                if (session == null)
                {
                    _logger.LogWarning("Submit called with invalid or expired session ID: {SessionId}", sid);
                    return StatusCode(500, new 
                    { 
                        isSuccess = false, 
                        message = "Checkout session expired. Please try again." 
                    });
                }

                _logger.LogInformation(
                    "Processing payment submission for Invoice {Invoice}, BAN {BAN}", 
                    session.InvoiceNumber, 
                    session.BanNumber);

                // Validate payment method
                if (string.IsNullOrEmpty(model.PaymentMethod))
                {
                    return BadRequest(new { isSuccess = false, message = "Payment method is required" });
                }

                if (model.PaymentMethod != "MPAISA" && model.PaymentMethod != "CARD")
                {
                    return BadRequest(new { isSuccess = false, message = "Invalid payment method" });
                }

                // Determine the actual payment amount
                // If partial payment is selected on the checkout page, use the form value
                var isPartial = IsPartialPayment ?? session.IsPartialPayment;
                var actualPaymentAmount = session.PaymentAmount;

                if (isPartial && PaymentAmount.HasValue)
                {
                    // Validate the partial payment amount
                    if (PaymentAmount.Value < 10)
                    {
                        return BadRequest(new { isSuccess = false, message = "Minimum partial payment is $10" });
                    }

                    if (PaymentAmount.Value > session.FullAmount)
                    {
                        return BadRequest(new { isSuccess = false, message = "Payment amount cannot exceed total invoice amount" });
                    }

                    actualPaymentAmount = PaymentAmount.Value;
                    _logger.LogInformation(
                        "Using partial payment amount: {Amount} (original session amount: {SessionAmount})",
                        actualPaymentAmount,
                        session.PaymentAmount);
                }
                else if (!isPartial)
                {
                    // Full payment - use the full amount from session
                    actualPaymentAmount = session.FullAmount;
                }

                // Build payment request with validated amount
                var request = new MyBillPaymentRequest
                {
                    InvoiceNumber = session.InvoiceNumber,
                    SelectedBAN = session.BanNumber,
                    Amount = actualPaymentAmount,
                    IsPartialPayment = isPartial,
                    PaymentInformationRequest = BuildPaymentRequest(model, session.PageUrl, session.InvoiceNumber, session.BanNumber, session.Email, actualPaymentAmount)
                };

                // Call payment service
                var response = await _myBillPaymentService.ProcessPaymentAsync(request);

                // Check for backend validation errors (400 Bad Request)
                if (response?.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    _logger.LogWarning(
                        "Payment request validation failed for Invoice {Invoice}. Backend returned 400.", 
                        session.InvoiceNumber);
                    
                    return BadRequest(new 
                    { 
                        isSuccess = false, 
                        message = "Payment request validation failed. Please check all billing fields are filled correctly." 
                    });
                }

                // Check for other error status codes
                if (response?.StatusCode != null && (int)response.StatusCode >= 400)
                {
                    _logger.LogError(
                        "Payment service returned error status {StatusCode} for Invoice {Invoice}", 
                        response.StatusCode, session.InvoiceNumber);
                    
                    return StatusCode((int)response.StatusCode, new 
                    { 
                        isSuccess = false, 
                        message = $"Payment service error ({response.StatusCode}). Please try again." 
                    });
                }

                if (response?.data?.RedirectUrl != null)
                {
                    _logger.LogInformation(
                        "Payment request successful for Invoice {Invoice}. Redirecting to payment gateway.", 
                        session.InvoiceNumber);

                    return Ok(new 
                    { 
                        isSuccess = true, 
                        data = new 
                        { 
                            url = response.data.RedirectUrl 
                        } 
                    });
                }

                _logger.LogError("Payment service did not return a redirect URL for Invoice {Invoice}", session.InvoiceNumber);
                return StatusCode(500, new 
                { 
                    isSuccess = false, 
                    message = "Payment service error. Please try again." 
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access during payment submission");
                return Unauthorized(new 
                { 
                    isSuccess = false, 
                    message = "Session expired. Please log in again." 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment submission");
                return StatusCode(500, new 
                { 
                    isSuccess = false, 
                    message = "An error occurred while processing payment. Please try again." 
                });
            }
        }

        /// <summary>
        /// Verify payment callback from payment gateway
        /// Note: AllowAnonymous is required because this is called after redirect from payment gateway
        /// where the authentication token may have expired or not be available in the AJAX request
        /// </summary>
        [HttpPost("provide-update")]
        [AllowAnonymous]
        public async Task<IActionResult> ProvideUpdate([FromBody] MyBillProvideUpdateRequest request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("ProvideUpdate called with null request");
                    return BadRequest(new { isSuccess = false, message = "Invalid request - request body is null" });
                }

                _logger.LogDebug("ProvideUpdate request received for Invoice: {Invoice}, BAN: {BAN}", 
                    request.InvoiceNumber, request.SelectedBAN);

                // Extract transaction info from callback data
                string transactionId = null;
                if (request.creditCardPayment != null)
                {
                    transactionId = request.creditCardPayment.sessionId;
                    _logger.LogDebug("Processing CARD payment callback with sessionId: {SessionId}", transactionId);
                }
                else if (request.mPaisaPayment != null)
                {
                    transactionId = request.mPaisaPayment.transactionId;
                    _logger.LogDebug("Processing M-PAISA payment callback with transactionId: {TransactionId}", transactionId);
                }

                if (string.IsNullOrEmpty(transactionId))
                {
                    _logger.LogWarning("No transaction ID found in callback data");
                    return BadRequest(new { isSuccess = false, message = "Missing transaction ID in callback" });
                }

                // Update request with extracted transaction ID
                request.TransactionId = transactionId;

                var result = await _myBillPaymentService.ProvideUpdateAsync(request);
                
                _logger.LogDebug("Payment verification result - HasData: {HasData}, IsSuccessful: {IsSuccessful}", 
                    result?.data != null, result?.data?.IsSuccessful);

                if (result?.data == null)
                {
                    _logger.LogWarning("Backend API returned null data for TransactionId {TransactionId}", transactionId);
                    return BadRequest(new
                    {
                        isSuccess = false,
                        message = "Payment verification failed",
                        data = result?.data
                    });
                }

                if (result.data.IsSuccessful)
                {
                    _logger.LogDebug("Payment verified successfully for Invoice {Invoice}", result.data.InvoiceNumber);
                    return Ok(new
                    {
                        isSuccess = true,
                        message = "Payment successful",
                        data = result.data
                    });
                }

                _logger.LogWarning("Payment verification returned unsuccessful. Error: {Error}", 
                    result.data.ErrorMessage ?? "Unknown error");
                return Ok(new
                {
                    isSuccess = false,
                    message = "Payment verification failed",
                    data = result.data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment verification");
                return StatusCode(500, new 
                { 
                    isSuccess = false, 
                    message = "An error occurred while verifying payment"
                });
            }
        }

        /// <summary>
        /// Build payment information request based on payment method
        /// </summary>
        private PaymentInformationRequest BuildPaymentRequest(
            MyBillCheckoutViewModel model, 
            string pageUrl,
            string invoiceNumber,
            string banNumber,
            string email,
            decimal amount)
        {
            // Use clean callback URL without query parameters (payment gateway friendly)
            var callbackUrl = $"{Request.Scheme}://{Request.Host}{pageUrl}";
            
            // Store invoice/BAN/email/amount in encrypted cookie for retrieval after payment callback
            StorePaymentContext(invoiceNumber, banNumber, email, amount);
            
            if (model.PaymentMethod == "MPAISA")
            {
                return new PaymentInformationRequest
                {
                    MPaisaPayment = new MPaisaPayment
                    {
                        RedirectUrl = callbackUrl
                    }
                };
            }

            // Card payment
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
                    RedirectUrl = callbackUrl
                }
            };
        }

        /// <summary>
        /// Store payment context in encrypted cookie for retrieval after payment gateway callback
        /// Includes email and amount for display on success/failure pages
        /// </summary>
        private void StorePaymentContext(string invoiceNumber, string banNumber, string email, decimal amount)
        {
            var paymentContext = $"{invoiceNumber}|{banNumber}|{email}|{amount}";
            
            CookieHelper.SetCookie(
                Response,
                "mybill_payment_context",
                paymentContext,
                encrypt: true,
                expireMins: null,  // Session cookie - expires when browser closes
                httpOnly: true,
                secure: Request.IsHttps,
                sameSite: SameSiteMode.Lax
            );
        }

        /// <summary>
        /// Get payment context from encrypted cookie (called by JavaScript after payment callback)
        /// Note: AllowAnonymous is required because this is called after redirect from payment gateway
        /// </summary>
        [HttpGet("get-payment-context")]
        [AllowAnonymous]
        public IActionResult GetPaymentContext()
        {
            try
            {
                var paymentContext = CookieHelper.GetCookie(Request, "mybill_payment_context", decrypt: true);
                
                if (string.IsNullOrEmpty(paymentContext))
                {
                    _logger.LogWarning("GetPaymentContext: No payment context cookie found");
                    return Ok(new { isSuccess = false, message = "No payment context found" });
                }

                var parts = paymentContext.Split('|');
                if (parts.Length < 2)
                {
                    _logger.LogWarning("GetPaymentContext: Invalid payment context format");
                    return Ok(new { isSuccess = false, message = "Invalid payment context format" });
                }

                var invoiceNumber = parts[0];
                var banNumber = parts[1];
                var email = parts.Length > 2 ? parts[2] : null;
                var amount = parts.Length > 3 ? parts[3] : null;

                _logger.LogDebug("GetPaymentContext: Retrieved context for Invoice {Invoice}, BAN {BAN}, Email {Email}, Amount {Amount}", 
                    invoiceNumber, banNumber, email, amount);

                // Clear the cookie after retrieval (one-time use)
                CookieHelper.DeleteCookie(Response, "mybill_payment_context");

                return Ok(new 
                { 
                    isSuccess = true, 
                    invoiceNumber = invoiceNumber,
                    banNumber = banNumber,
                    email = email,
                    amount = amount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment context");
                return Ok(new { isSuccess = false, message = "Error retrieving payment context" });
            }
        }
    }
}
