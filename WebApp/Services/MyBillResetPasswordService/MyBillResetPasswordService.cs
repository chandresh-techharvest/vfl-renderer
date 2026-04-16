using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Models.MyBillResetPassword;
using VFL.Renderer.Services.MyBillResetPasswordService.Models;

namespace VFL.Renderer.Services.MyBillResetPasswordService
{
    public class MyBillResetPasswordService : IMyBillResetPasswordService
    {
        private readonly MyBillPublicApiClient _myBillPublicApiClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MyBillResetPasswordService> _logger;

        public MyBillResetPasswordService(MyBillPublicApiClient myBillPublicApiClient, IMemoryCache cache, ILogger<MyBillResetPasswordService> logger)
        {
            _myBillPublicApiClient = myBillPublicApiClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<T>> ResetPassword<T>(MyBillResetPasswordVerifyRequest request)
        {
            try
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        password = request.password,
                        token = request.token,
                        username = request.username
                    }),
                    Encoding.UTF8,
                    "application/json");


                return await _myBillPublicApiClient.PostAsync<T>("api/ResetPassword/ResetRequest", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in MyBill POST ResetPassword Service Layer");
                throw;
            }
        }

        public async Task<ApiResponse<T>> SubmitRequest<T>(MyBillForgotPasswordRequest request)
        {
            try
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        ban = request.ban,
                        verificationPageUrl = request.verificationPageUrl
                    }),
                    Encoding.UTF8,
                    "application/json");
                
                return await _myBillPublicApiClient.PostAsync<T>("api/ResetPassword/SubmitRequest", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in MyBill POST SubmitRequest Service Layer");
                throw;
            }
        }
    }
}
