using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VFL.Renderer.OSApi.Config;
using VFL.Renderer.OSApi.LoginForm.Model;

namespace VFL.Renderer.OSApi.Clients
{
    public class AuthApiClient : BaseApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _baseUrl;
        protected readonly IMemoryCache _cache;

        public AuthApiClient(HttpClient httpClient,
                             IOptions<ApiSettings> options,
                             ILogger logger,
                             IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = options.Value.BaseUrl.TrimEnd('/');
            _cache = cache;

        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var httprequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/Login/Standard");
            httprequest.Headers.Add("Cookie", "OnlineServicesRefreshToken=" + Guid.NewGuid());
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(request.password), "Password");
            content.Add(new StringContent(request.username), "Username");
            httprequest.Content = content;
            try
            {
                var response = await _httpClient.SendAsync(httprequest);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                LoginResponse loginResponse = new LoginResponse();
                loginResponse.Message = ex.Message;
                return loginResponse;
            }


            //using var form = new MultipartFormDataContent();
            //form.Add(new StringContent(request.Password), "Password");
            //form.Add(new StringContent(request.Username), "Username");

            //var response = await _httpClient.PostAsync($"{_baseUrl}/api/Login/Standard", form);




        }

        public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/Login/GetRefreshTokenForOpenId");
            req.Headers.Add("Cookie", $"OnlineServicesRefreshToken={refreshToken}");

            var response = await _httpClient.SendAsync(req);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


    }
}
