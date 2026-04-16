using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFL.Renderer.OSApi.Config;
using VFL.Renderer.OSApi.ResetPassword.Model;

namespace VFL.Renderer.OSApi.Clients
{
    public class ResetPasswordApiClient: BaseApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _baseUrl;        
        public PasswordResetApiClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = options.Value.BaseUrl.TrimEnd('/');
        }


        public async Task<ResetPasswordResponse> SubmitRequest(ForgotPasswordRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/ForgotPassword/SubmitRequest");

            using var content = new MultipartFormDataContent();
            if (!string.IsNullOrWhiteSpace(request.Email))
                content.Add(new StringContent(request.Email), nameof(request.Email));
            if (!string.IsNullOrWhiteSpace(request.VerificationPageUrl))
                content.Add(new StringContent(request.VerificationPageUrl), nameof(request.VerificationPageUrl));

            httpRequest.Content = content;
            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                {
                    return JsonConvert.DeserializeObject<ResetPasswordResponse>(responseContent);
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

        public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/ForgotPassword/ResetPassword");

            var content = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");

            httpRequest.Content = content;
            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(responseContent))
            {
                return JsonConvert.DeserializeObject<ResetPasswordResponse>(responseContent);
            }
            else
            {
                return null;
            }
        }
    }
}
