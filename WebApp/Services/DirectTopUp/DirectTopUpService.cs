using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.DirectTopUp.Models;

namespace VFL.Renderer.Services.DirectTopUp
{
    public class DirectTopUpService : IDirectTopUpService
    {

        WebClient _client;
        private readonly ILogger<WebClient> _logger;

        public DirectTopUpService(WebClient webClient, ILogger<WebClient> logger)
        {
            _client = webClient;
            _logger = logger;
        }


        public async Task<ApiResponse<T>> SendRequest<T>(DirectTopUpRequest request)
        {
            try
            {   
                //var setting = new JsonSerializerSettings
                //{
                //    NullValueHandling = NullValueHandling.Ignore
                //};
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/DirectTopUp/SendRequest", content, "StreamContent");
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/DirectTopUp/SendRequest");
                throw;
            }
        }
    }
}
