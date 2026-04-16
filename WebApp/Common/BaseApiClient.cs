using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly.Caching;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Config;

namespace VFL.Renderer.Common
{
    public abstract class BaseApiClient : IApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger _logger;
        protected readonly IMemoryCache _cache;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseApiClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger logger, IMemoryCache cache)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/'));

        }
        protected BaseApiClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger logger, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            //if (_httpContextAccessor.HttpContext.Session != null)
            //{
            //    var token = _httpContextAccessor.HttpContext.Session.GetString("SitefinityJwtAuth");
            //    if (!string.IsNullOrEmpty(token))
            //    {
            //        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //    }
            //}
        }

        public async Task<T> PostAsync<T>(string url, object data)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}{url}");
                httpRequest.Content = (HttpContent)data;
                var response = await _httpClient.SendAsync(httpRequest);
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", url);
                throw;
            }
        }
        public async Task<T> GetAsync<T>(string url, string cacheKey, TimeSpan? cacheDuration = null)
        {
            if (_cache.TryGetValue(cacheKey, out T cachedValue))
            {
                _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
                return cachedValue;
            }

            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}{url}");

                var response = await _httpClient.SendAsync(httpRequest);
                if (response != null)
                {
                    response.EnsureSuccessStatusCode();
                }
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(content);

                if (result != null && cacheDuration.HasValue)
                {
                    _cache.Set(cacheKey, result, cacheDuration.Value);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GET {Url}", url);
                throw;
            }
        }

    }
}
