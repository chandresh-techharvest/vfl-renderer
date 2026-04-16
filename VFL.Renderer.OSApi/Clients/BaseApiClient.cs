using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VFL.Renderer.OSApi.Config;

namespace VFL.Renderer.OSApi.Clients
{
    public abstract class BaseApiClient : IApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger _logger;
        protected readonly IMemoryCache _cache;
        protected readonly string _baseUrl;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        /// <param name="cache"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected BaseApiClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger logger, IMemoryCache cache)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(object data)
        {
            string url = _baseUrl;
            try
            {                
                var response = await _httpClient.PostAsJsonAsync(url, data); // converts object to JSON
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", url);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheDuration"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>( string cacheKey, TimeSpan? cacheDuration = null)
        {
            if (_cache.TryGetValue(cacheKey, out T cachedValue))
            {
                _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
                return cachedValue;
            }
            string url = _baseUrl;
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(content);

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

        /// <summary>
        /// For updating an existing item over a web api using PUT
        /// </summary>
        /// <param name="apiUrl">API Url</param>
        /// <param name="putObject">The object to be edited</param>
        public async Task<T> PutAsync<T>(object putObject)
        {
            string url = _baseUrl;
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, putObject);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", url);
                throw;
            }
        }

    }
}
