using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.Validation.Models;

namespace VFL.Renderer.Services.Validation
{
    public class ValidationService : IValidationService
    {
        private readonly WebClient _client;
        private readonly ILogger<AuthApiClient> _logger;

        public ValidationService(WebClient client, ILogger<AuthApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EmailVerifyRequest"></param>
        /// <returns>Task<ValidationResponse></returns>
        public async Task<ApiResponse<T>> CheckEmailIsRegisteredAsync<T>(EmailVerifyRequest request)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");
                return await _client.PostAsync<ApiResponse<T>>("api/General/CheckEmailIsRegistered", content);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST CheckEmailIsRegisteredAsync Service Layer");
                throw;
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NumberVerifyRequest"></param>
        /// <returns>Task<ValidationResponse></returns>
        public async Task<ApiResponse<T>> CheckNumberIsRegisteredAsync<T>(NumberVerifyRequest request)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");
                return await _client.PostAsync<ApiResponse<T>>("api/General/CheckNumberIsRegistered", content);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST CheckNumberIsRegisteredAsync Service Layer");
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NumberVerifyRequest"></param>
        /// <returns>Task<ValidationResponse></returns>
        public async Task<ApiResponse<T>> CheckNumberIsValidAsync<T>(NumberVerifyRequest request)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");
                return await _client.PostAsync<ApiResponse<T>>("api/General/CheckNumberIsValid", content);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST CheckNumberIsValidAsync Service Layer");
                throw;
            }
        }

        public async Task<ApiResponse<T>> CheckNumberIsValid_AllowInactiveNumber<T>(NumberVerifyRequest request)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");
                return await _client.PostAsync<ApiResponse<T>>("api/General/CheckNumberIsValid_AllowInactiveNumber", content);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST CheckNumberIsValidAsync Service Layer");
                throw;
            }
        }





    }
}
