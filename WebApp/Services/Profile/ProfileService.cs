using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.Profile.Models;
using static Progress.Sitefinity.AspNetCore.Constants;

namespace VFL.Renderer.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly WebClient _webClient;
        private readonly ILogger<AuthApiClient> _logger;

        public ProfileService(WebClient webClient, ILogger<AuthApiClient> logger)
        {
            _webClient = webClient;
            _logger = logger;
        }

        public async Task<ApiResponse<T>> GetProfileInformationAsync<T>()
        {
            try
            {
                string cacheKey = "ProfileInformation";
                var cacheDuration = System.TimeSpan.FromHours(6);
                return await _webClient.GetWithTokenAsync<T>("api/MyVodafoneDashboard/GetProfileInformation", cacheKey, cacheDuration);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/MyVodafoneDashboard/GetProfileInformation");
                throw;
            }
            
        }

        public async Task<ApiResponse<T>> EditProfileInformationAsync<T>(EditProfileRequest request)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                if (!string.IsNullOrWhiteSpace(request.Email))
                    content.Add(new StringContent(request.Email), nameof(request.Email));
                if (!string.IsNullOrWhiteSpace(request.FirstName))
                    content.Add(new StringContent(request.FirstName), nameof(request.FirstName));
                if (!string.IsNullOrWhiteSpace(request.LastName))
                    content.Add(new StringContent(request.LastName), nameof(request.LastName));
                if (!string.IsNullOrWhiteSpace(request.Password))
                    content.Add(new StringContent(request.Password), nameof(request.Password));
                if (!string.IsNullOrWhiteSpace(request.MiddleName))
                    content.Add(new StringContent(request.MiddleName), nameof(request.MiddleName));

                if (request.ProfileImage != null && request.ProfileImage.Length > 0)
                {
                    var streamContent = new StreamContent(request.ProfileImage.OpenReadStream());
                    streamContent.Headers.Add("Content-Type", request.ProfileImage.ContentType);
                    content.Add(streamContent, "ProfileImage", request.ProfileImage.FileName);
                }

                if (!string.IsNullOrWhiteSpace(request.Password))
                    content.Add(new StringContent(request.Password), nameof(request.Password));

                return await _webClient.PostWithTokenAsync<T>("api/EditProfile/UpdateUser", content, "MultipartFormDataContent");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/EditProfile/UpdateUser");
                throw;
            }            
            
        }

    }
}
