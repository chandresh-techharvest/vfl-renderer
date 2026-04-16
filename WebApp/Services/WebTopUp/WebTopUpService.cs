using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.Dashboard.Models;
using VFL.Renderer.Services.WebTopUp.Models;

namespace VFL.Renderer.Services.WebTopUp
{
    public class WebTopUpService: IWebTopUpService
    {
        WebClient _client;
        private readonly ILogger<WebClient> _logger;
        public WebTopUpService(WebClient webClient, ILogger<WebClient> logger)
        {
            _client = webClient;
            _logger = logger;
        }

        //processpayment api for Authenticated user

        public async Task<ApiResponse<T>> ProcessPaymentPublic<T>(WebTopUpRequest request)
        {
            try
            {

                var setting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                var content = new StringContent(JsonConvert.SerializeObject(request, setting), null, "application/json");
                var response = await _client.PostAsync<ApiResponse<T>>("api/WebTopUp/ProcessPayment", content);
                return response;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/PurchasePlan/ProcessPayment");
                throw;
            }
        }

        //processpayment api for Authenticated user
        public async Task<ApiResponse<T>> ProcessPayment<T>(WebTopUpRequest request)
        {
            try
            {

                var setting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/WebTopUp/ProcessPayment", content, "application/json");
                return response;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/PurchasePlan/ProcessPayment");
                throw;
            }
        }









        //provideUpdate api for UnAuthenticated user
        public async Task<ApiResponse<T>> ProvideUpdatePublic<T>(ProvideUpdateRequest request)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");
                var response = await _client.PostAsync<ApiResponse<T>>("api/WebTopUp/ProvideUpdate", content);
                return response;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/PurchasePlan/ProvideUpdate");
                throw;
            }
        }

        //provideUpdate api for Authenticated user
        public async Task<ApiResponse<T>> ProvideUpdate<T>(ProvideUpdateRequest request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/WebTopUp/ProvideUpdate", content, "application/json");
                return response;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/PurchasePlan/ProvideUpdate");
                throw;
            }
        }




    }
}
