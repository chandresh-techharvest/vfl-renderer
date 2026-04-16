using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.MyBillPayment.Models;

namespace VFL.Renderer.Services.MyBillPayment
{
    /// <summary>
    /// Service for processing MyBill payments
    /// Uses MyBillWebClient for authenticated API calls
    /// </summary>
    public class MyBillPaymentService : IMyBillPaymentService
    {
        private readonly MyBillWebClient _client;
        private readonly ILogger<MyBillPaymentService> _logger;

        public MyBillPaymentService(
            MyBillWebClient client, 
            ILogger<MyBillPaymentService> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Process a payment request and get payment gateway redirect URL
        /// </summary>
        public async Task<ApiResponse<MyBillPaymentResponse>> ProcessPaymentAsync(MyBillPaymentRequest request)
        {
            try
            {
                var response = await _client.PostWithTokenAsync<MyBillPaymentResponse>(
                    "/api/BillPayment/PaymentRequest", 
                    request, 
                    "application/json"
                );

                // Check for validation errors (400 Bad Request)
                if (response?.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    _logger.LogError(
                        "Payment request validation failed for Invoice {Invoice}.",
                        request.InvoiceNumber);
                }

                if (response?.data?.RedirectUrl == null)
                {
                    _logger.LogWarning(
                        "Payment request for Invoice {Invoice} did not return a redirect URL. Status: {Status}", 
                        request.InvoiceNumber,
                        response?.StatusCode);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error processing payment for Invoice {Invoice}, BAN {BAN}", 
                    request.InvoiceNumber, 
                    request.SelectedBAN);
                throw;
            }
        }

        /// <summary>
        /// Verify payment callback from payment gateway
        /// Uses PostWithCookiesAsync since this may be called from an anonymous endpoint
        /// after redirect from payment gateway
        /// </summary>
        public async Task<ApiResponse<MyBillProvideUpdateResponse>> ProvideUpdateAsync(MyBillProvideUpdateRequest request)
        {
            try
            {
                var response = await _client.PostWithCookiesAsync<MyBillProvideUpdateResponse>(
                    "/api/BillPayment/PaymentUpdate", 
                    request
                );

                if (response?.data?.IsSuccessful != true)
                {
                    _logger.LogWarning(
                        "Payment verification failed or returned unsuccessful. Error: {Error}", 
                        response?.data?.ErrorMessage ?? "No error message");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Exception in ProvideUpdateAsync for Invoice {Invoice}, BAN {BAN}", 
                    request.InvoiceNumber, 
                    request.SelectedBAN);
                throw;
            }
        }
    }
}
