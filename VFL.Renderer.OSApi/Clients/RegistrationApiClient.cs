using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VFL.Renderer.OSApi.Config;
using VFL.Renderer.OSApi.Registration.Model;

namespace VFL.Renderer.OSApi.Clients
{
    public class RegistrationApiClient : BaseApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger _logger;
        protected readonly IMemoryCache _cache;
        protected readonly string _baseUrl;

        const string Registrationendpoint = "/api/Registration";
        const string Generalendpoint = "/api/General";

        public RegistrationApiClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger<RegistrationApiClient> logger, IMemoryCache cache)
            : base(httpClient, options, logger, cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = options.Value.BaseUrl.TrimEnd('/');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="RegistrationResponse"></typeparam>
        /// <param name="RegistrationRequest"></param>
        /// <returns></returns>
        public async Task<RegistrationResponse> PostFormAsync<RegistrationResponse>(RegistrationRequest request)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl + Registrationendpoint + "/Create");
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
                var response = await _httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                {
                    return JsonConvert.DeserializeObject<RegistrationResponse>(responseContent);
                }
                else
                {
                    return JsonConvert.DeserializeObject<RegistrationResponse>(responseContent);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST FormData {Endpoint}", Registrationendpoint);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EmailVerify"></param>
        /// <returns></returns>
        public async Task<RegistrationResponse> ConfirmEmail(EmailVerify request)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl + Registrationendpoint + "/ConfirmEmail");
                var content = new StringContent(JsonConvert.SerializeObject(request));

                httpRequest.Content = content;
                var response = await _httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                {
                    return JsonConvert.DeserializeObject<RegistrationResponse>(responseContent);
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", Registrationendpoint + "/ConfirmEmail");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RegistrationNumber"></param>
        /// <returns></returns>
        public async Task<RegistrationResponse> CheckIsRegistered(RegistrationNumber request)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl + Generalendpoint + "/Create");
                var content = new StringContent(JsonConvert.SerializeObject(request));

                httpRequest.Content = content;
                var response = await _httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                {
                    return JsonConvert.DeserializeObject<RegistrationResponse>(responseContent);
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", Generalendpoint + "/ConfirmEmail");
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RegistrationNumber"></param>
        /// <returns></returns>
        public async Task<RegistrationResponse> CheckIsValid (RegistrationNumber request)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl + Generalendpoint + "/Create");
                var content = new StringContent(JsonConvert.SerializeObject(request));

                httpRequest.Content = content;
                var response = await _httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                {
                    return JsonConvert.DeserializeObject<RegistrationResponse>(responseContent);
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", Generalendpoint + "/ConfirmEmail");
                throw;
            }
        }

    }
}
