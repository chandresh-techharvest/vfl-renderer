using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Config;

namespace VFL.Renderer.ApiClients
{
    /// <summary>
    /// Public (unauthenticated) API client for MyBill portal
    /// Used for operations that don't require authentication like forgot password
    /// </summary>
    public class MyBillPublicApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MyBillPublicApiClient> _logger;
        private readonly string _baseUrl;

        public MyBillPublicApiClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger<MyBillPublicApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseUrl = options.Value.MyBillBaseUrl.TrimEnd('/');
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string relativeUrl, HttpContent content)
        {
            try
            {
                string url = $"{_baseUrl}/{relativeUrl.TrimStart('/')}";
                _logger.LogInformation("MyBill Public API: POST {Url}", url);
                
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
                httpRequest.Content = content;
                
                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("MyBill Public API Response: StatusCode={StatusCode}, Content={Content}", 
                    response.StatusCode, responseContent);
                
                ApiResponse<T> apiResponse = new ApiResponse<T>();
                
                if (!string.IsNullOrEmpty(responseContent))
                {
                    try
                    {
                        // Try to deserialize as the direct response type T (the actual MyBill API response)
                        var directResponse = JsonConvert.DeserializeObject<T>(responseContent);
                        apiResponse.data = directResponse;
                        apiResponse.StatusCode = response.StatusCode;
                        apiResponse.status = (int)response.StatusCode;
                        
                        _logger.LogInformation("MyBill Public API: Successfully deserialized response. Data is null: {IsNull}", apiResponse.data == null);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "MyBill Public API: Failed to deserialize response as type {Type}. Response: {Response}", 
                            typeof(T).Name, responseContent);
                        
                        // If direct deserialization fails, try as ApiResponse<T>
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        apiResponse.StatusCode = response.StatusCode;
                    }
                }
                else
                {
                    apiResponse.StatusCode = response.StatusCode;
                    apiResponse.status = (int)response.StatusCode;
                }
                
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill Public API: Error calling POST {Url}", relativeUrl);
                throw;
            }
        }
    }
}
