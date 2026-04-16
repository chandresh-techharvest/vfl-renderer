using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using VFL.Renderer.Common;
using Microsoft.Extensions.Options;
using VFL.Renderer.Config;

namespace VFL.Renderer.ApiClients
{
    public class ApiClient : BaseApiClient
    {
        public ApiClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger<ApiClient> logger, IMemoryCache cache)
            : base(httpClient, options, logger, cache)
        {
        }

    }
}
