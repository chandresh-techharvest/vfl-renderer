using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Models.MyBillLoginForm;

namespace VFL.Renderer.Services.MyBillLogin
{
    public class AuthServiceMyBill : IAuthServiceMyBill
    {
        private readonly AuthApiClient _authApiClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthApiClient> _logger;

        public AuthServiceMyBill(AuthApiClient authApiClient, IMemoryCache cache, ILogger<AuthApiClient> logger)
        {
            _authApiClient = authApiClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<T>> MyBillLoginAsync<T>(MyBillLoginRequest request)
        {
            try
            {
                var response = await _authApiClient.MyBillLoginAsync<T>(request);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST LoginAsync Service Layer");
                throw;
            }

        }

        /// <summary>
        /// Request a new JWT access token using the existing refresh token cookie
        /// This is called automatically when the JWT expires (after 15 minutes)
        /// </summary>
        public async Task<ApiResponse<T>> RequestNewAccessTokenAsync<T>()
        {
            try
            {
                var response = await _authApiClient.RequestNewAccessTokenAsync<T>();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in RequestNewAccessTokenAsync Service Layer");
                throw;
            }
        }

        public async Task<ApiResponse<T>> RefreshTokenAsync<T>()
        {
            try
            {
                return await _authApiClient.RefreshTokenAsync<T>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST RefreshTokenAsync Service Layer");
                throw;
            }
        }

        public async Task<ApiResponse<T>> RefreshTokenAsync<T>(string refreshToken)
        {
            try
            {
                return await _authApiClient.RefreshTokenAsync<T>(refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST RefreshTokenAsync Service Layer");
                throw;
            }
        }

        public async Task<ApiResponse<T>> ExternalLoginAsync<T>(string provider, string token)
        {
            var response = await _authApiClient.ExternalLoginAsync<T>(provider, token);
            return response;
        }

        /// <summary>
        /// Login using a single-use access token (for customer support)
        /// This allows support staff to access customer profiles using a pre-generated token
        /// </summary>
        public async Task<ApiResponse<T>> LoginUsingSingleAccessTokenAsync<T>(string token)
        {
            try
            {
                var response = await _authApiClient.MyBillLoginUsingSingleAccessTokenAsync<T>(token);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in LoginUsingSingleAccessTokenAsync Service Layer");
                throw;
            }
        }
    }
}
