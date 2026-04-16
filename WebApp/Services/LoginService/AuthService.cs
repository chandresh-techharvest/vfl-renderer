using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Models.LoginForm;

namespace VFL.Renderer.Services.LoginService
{
    public class AuthService : IAuthService
    {
        private readonly AuthApiClient _authApiClient;
        private readonly IMemoryCache _cache;
        private const string RefreshTokenKey = "Auth.RefreshToken";
        private readonly ILogger<AuthApiClient> _logger;

        public AuthService(AuthApiClient authApiClient, IMemoryCache cache, ILogger<AuthApiClient> logger)
        {
            _authApiClient = authApiClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<T>> LoginAsync<T>(LoginRequest request)
        {
            try
            {
                var response = await _authApiClient.LoginAsync<T>(request);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST LoginAsync Service Layer"); 
                throw;
            }
           
        }



        public async Task<ApiResponse<T>> RefreshTokenAsync<T>()
        {
            if (_cache.TryGetValue(RefreshTokenKey, out string? refreshToken) && refreshToken != null)
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
            else
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
            return null;
        }

        public async Task<ApiResponse<T>> ExternalLoginAsync<T>(string provider, string token)
        {
            var response = await _authApiClient.ExternalLoginAsync<T>(provider,token);
            //if (response?.RefreshToken != null)
            //{
            //    _cache.Set(RefreshTokenKey, response.RefreshToken, TimeSpan.FromHours(6));
            //}
            return response;
        }
        

        public Task<ApiResponse<T>> SingleAccessLoginAsync<T>(string Token)
        {
            try
            {
                var response = _authApiClient.LoginUsingSingleAccessTokenAsync<T>(Token);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST LoginAsync Service Layer");
                throw;
            }
        }
    }
}
