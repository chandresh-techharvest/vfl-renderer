using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.Registration.Models;

namespace VFL.Renderer.Services.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly RegistrationApiClient _apiClient;
        private readonly ILogger<AuthApiClient> _logger;

        public RegistrationService(RegistrationApiClient apiClient, ILogger<AuthApiClient> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<ApiResponse<T>> RegisterAsync<T>(RegistrationRequest request)
        {
            try
            {
                return await _apiClient.PostFormAsync<T>(request);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST RegisterAsync Service Layer");
                throw;
            }
            
        }

        public async Task<ApiResponse<T>> ConfirmEmailAsync<T>(EmailVerify request)
        {
            try
            {
                return await _apiClient.ConfirmEmail<T>(request);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST ConfirmEmailAsync Service Layer");
                throw;
            }
        }

        public async Task<ApiResponse<T>> ResendConfirmationEmailAsync<T>(ResendEmailVerify request)
        {
            try
            {
                return await _apiClient.ResendConfirmationEmail<T>(request);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST ResendConfirmationEmailAsync Service Layer");
                throw;
            }
        }

    }
}
