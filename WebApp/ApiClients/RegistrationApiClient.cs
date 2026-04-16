using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Config;
using VFL.Renderer.Entities.Registration;
using VFL.Renderer.Models.Registration;
using VFL.Renderer.Services.Registration.Models;
using static Progress.Sitefinity.AspNetCore.Constants;

namespace VFL.Renderer.ApiClients
{
    public class RegistrationApiClient : BaseApiClient
    {
        

        public RegistrationApiClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger<RegistrationApiClient> logger, IMemoryCache cache)
            : base(httpClient, options, logger, cache)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RegistrationRequest"></param>
        /// <returns></returns>
        public async Task<ApiResponse<T>> PostFormAsync<T>(RegistrationRequest request)
        {

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}api/Registration/Create");
            using var content = new MultipartFormDataContent();

            if (!string.IsNullOrWhiteSpace(request.Email))
                content.Add(new StringContent(request.Email), nameof(request.Email));
            if (!string.IsNullOrWhiteSpace(request.FirstName))
                content.Add(new StringContent(request.FirstName), nameof(request.FirstName));
            if (!string.IsNullOrWhiteSpace(request.LastName))
                content.Add(new StringContent(request.LastName), nameof(request.LastName));
            if (!string.IsNullOrWhiteSpace(request.Password))
                content.Add(new StringContent(request.Password), nameof(request.Password));
            // Only add optional fields if they are not empty
            if (!string.IsNullOrWhiteSpace(request.MiddleName))
                content.Add(new StringContent(request.MiddleName), nameof(request.MiddleName));
            if (!string.IsNullOrWhiteSpace(request.Phone))
                content.Add(new StringContent(request.Phone), nameof(request.Phone));
            if (!string.IsNullOrWhiteSpace(request.VerificationPageUrl))
                content.Add(new StringContent(request.VerificationPageUrl), nameof(request.VerificationPageUrl));

            httpRequest.Content = content;
            ApiResponse<T> apiResponse = new ApiResponse<T>();
            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                if (response != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        apiResponse.StatusCode = response.StatusCode;
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                    }
                }
                else
                {
                    apiResponse.StatusCode = response.StatusCode;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"{_httpClient.BaseAddress}api/Registration/Create");
                throw;
            }
        }
        public async Task<ApiResponse<T>> ConfirmEmail<T>(EmailVerify request)
        {

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}api/Registration/ConfirmEmail");
            var content = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");

            httpRequest.Content = content;
            ApiResponse<T> apiResponse = new ApiResponse<T>();
            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                if (response != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        apiResponse.StatusCode = response.StatusCode;
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                    }
                }
                else
                {
                    apiResponse.StatusCode = response.StatusCode;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"{_httpClient.BaseAddress}api/Registration/ConfirmEmail");
                throw;
            }
        }
        public async Task<ApiResponse<T>> ResendConfirmationEmail<T>(ResendEmailVerify request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}api/Registration/ResendVerificationEmail");
            var content = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");

            httpRequest.Content = content;
            ApiResponse<T> apiResponse = new ApiResponse<T>();
            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                if (response != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        apiResponse.StatusCode = response.StatusCode;
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                    }
                }
                else
                {
                    apiResponse.StatusCode = response.StatusCode;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"{_httpClient.BaseAddress}api/Registration/ResendVerificationEmail");
                throw;
            }
        }
    }
}
