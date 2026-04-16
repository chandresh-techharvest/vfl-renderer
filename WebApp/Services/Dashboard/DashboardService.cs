using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.Dashboard.Models;

namespace VFL.Renderer.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        WebClient _client;
        private readonly ILogger<WebClient> _logger;
        public DashboardService(WebClient webClient, ILogger<WebClient> logger) { 
            _client = webClient;
            _logger = logger;
        }

        public async Task<ApiResponse<T>> AddDevice<T>(DeviceRequest request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/MyVodafoneDashboard/AddDevice", content, "application/json");
                return response;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/MyVodafoneDashboard/AddDevice");
                throw;
            }
        }

        public async Task<ApiResponse<T>> GetBalanceInformation<T>(DeviceGetRequestModel request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/MyVodafoneDashboard/GetBalanceInformation", content, "application/json");
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/MyVodafoneDashboard/GetBalanceInformation");
                throw;
            }
        }

        public async Task<ApiResponse<T>> RemoveDevice<T>(DeviceGetRequestModel request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/MyVodafoneDashboard/RemoveDevice", content, "application/json");
                return response;
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, "Error calling POST {Url}", $"api/MyVodafoneDashboard/RemoveDevice");
                throw;
            }
        }

        public async Task<ApiResponse<T>> SendOTPCode<T>(DeviceGetRequestModel request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/MyVodafoneDashboard/SendOTPCode", content, "application/json");
                return response;
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, "Error calling POST {Url}", $"api/MyVodafoneDashboard/GetProfileInformation");
                throw;
            }
        }
    }
}
