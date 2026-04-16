using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Models.ResetPassword;
using VFL.Renderer.Services.ResetPasswordService.Models;

namespace VFL.Renderer.Services.ResetPasswordService
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly WebClient _webClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthApiClient> _logger;

        public ResetPasswordService(WebClient webClient, ILogger<AuthApiClient> logger)
        {
            _webClient = webClient;
            _logger = logger;
        }

        public async Task<ApiResponse<T>> ResetPassword<T>(ResetPasswordVerifyRequest request)
        {
            try
            {
                var content = new StringContent("{\n  \"password\": \"" + request.Password + "\",\n  \"token\": \"" + request.Token + "\",\n  \"username\": \"" + request.User + "\"\n}", null, "application/json");
                return await _webClient.PostAsync<ApiResponse<T>>("api/ForgotPassword/ResetPassword", content);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST ResetPassword Service  Layer");
                throw;
            }
        }






        public async Task<ApiResponse<T>> SubmitRequest<T>(ForgotPasswordRequest request)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                if (!string.IsNullOrWhiteSpace(request.Email))
                    content.Add(new StringContent(request.Email), nameof(request.Email));
                if (!string.IsNullOrWhiteSpace(request.VerificationPageUrl))
                    content.Add(new StringContent(request.VerificationPageUrl), nameof(request.VerificationPageUrl));
                return await _webClient.PostAsync<ApiResponse<T>>("api/ForgotPassword/SubmitRequest", content);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST SubmitRequest Service Layer");
                throw;
            }
            
        }
    }
}
