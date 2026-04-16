using AngleSharp.Dom;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Config;
using VFL.Renderer.Models.LoginForm;
using VFL.Renderer.Models.MyBillLoginForm;

namespace VFL.Renderer.ApiClients
{
    public class AuthApiClient
    {
        private HttpClient _httpClient;
        private readonly ILogger<AuthApiClient> _logger;
        private readonly string _baseUrl;
        private readonly string _myBillBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiSettings _apiSettings;

        public AuthApiClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger<AuthApiClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = options.Value.BaseUrl.TrimEnd('/');
            _myBillBaseUrl = options.Value.MyBillBaseUrl.TrimEnd('/');
            _httpContextAccessor = httpContextAccessor;
            _apiSettings = options.Value;
        }

        public async Task<ApiResponse<T>> LoginAsync<T>(LoginRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/Login/Standard");
            
            var cookieContainer = new CookieContainer();
            //var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            //_httpClient = new HttpClient(handler);
            //var uri = new Uri(_httpContextAccessor.HttpContext?.Request.GetDisplayUrl());



            // Retrieve cookies for the specific URI
            var uri = new Uri($"{_baseUrl}");
            
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(request.password), "Password");
            content.Add(new StringContent(request.username), "Username");
            httpRequest.Content = content;
            ApiResponse<T> apiResponse = new ApiResponse<T>();
            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                               
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var setCookieHeader = response.Headers.GetValues("Set-Cookie")?.FirstOrDefault();
                        if (!string.IsNullOrEmpty(setCookieHeader))
                        {
                            // Parse cookie value: "onlinerefreshtoken=abc123; Path=/; HttpOnly"
                            var refreshTokenValue = ParseCookieValue(setCookieHeader, "OnlineServicesRefreshToken");
                            var Expires = ParseCookieValue(setCookieHeader, "expires");
                            TimeSpan expiresmin = DateTime.Parse(Expires) - DateTime.Now;
                            CookieHelper.SetCookie(_httpContextAccessor.HttpContext.Response, "OnlineServicesRefreshToken", refreshTokenValue, true, expiresmin.Minutes, httpOnly: true, secure: true, sameSite: SameSiteMode.Lax, path: "/");
                        }
                        var responseContent = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                            apiResponse.StatusCode = response.StatusCode;
                            return apiResponse;
                        }
                        else
                        {
                            apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                        }
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                            apiResponse.StatusCode = response.StatusCode;
                            return apiResponse;
                        }
                        else
                        {
                            apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                        }
                    }
                }
                else
                {
                    apiResponse.StatusCode = response.StatusCode;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"{_httpClient.BaseAddress}api/ForgotPassword/SubmitRequest");
                throw;
            }

        }
        
        public async Task<ApiResponse<T>> ExternalLoginAsync<T>(string provider, string token)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/Login/Google");
            //httpRequest.Headers.Add("Cookie", "OnlineServicesRefreshToken=" + Guid.NewGuid());

            //var content = new MultipartFormDataContent();
            //content.Add(new StringContent(provider), "Provider"); // e.g., "Google"
            //content.Add(new StringContent(token), "Token");       // e.g., Google ID token
            var parms = new
            {
                token = token
            };
            httpRequest.Content = new StringContent(JsonConvert.SerializeObject(parms), null, "application/json");
            ApiResponse<T> apiResponse = new ApiResponse<T>();
            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                if (response != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        apiResponse.StatusCode = response.StatusCode;
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                    }
                }
                else
                {
                    apiResponse.StatusCode = response.StatusCode;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"{_httpClient.BaseAddress}/api/ForgotPassword/SubmitRequest");
                throw;
            }
        }


        public async Task<ApiResponse<T>> RefreshTokenAsync<T>(string refreshToken)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/Login/GetRefreshTokenForOpenId");
            httpRequest.Headers.Add("Cookie", $"OnlineServicesRefreshToken={refreshToken}");
            ApiResponse<T> apiResponse = new ApiResponse<T>();
            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                if (response != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        apiResponse.StatusCode = response.StatusCode;
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                    }
                }
                else
                {
                    apiResponse.StatusCode = response.StatusCode;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"{_httpClient.BaseAddress}/api/ForgotPassword/SubmitRequest");
                throw;
            }

        }

        public async Task<ApiResponse<T>> MyBillLoginAsync<T>(MyBillLoginRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_myBillBaseUrl}/api/Login");

            var req = new MyBillLoginRequest1
            {
                ban = request.username,
                password = request.password
            };

            httpRequest.Content = new StringContent(JsonConvert.SerializeObject(req), null, "application/json");
            ApiResponse<T> apiResponse = new ApiResponse<T>();
            try
            {
                var response = await _httpClient.SendAsync(httpRequest);

                if (response != null)
                {
                    // CRITICAL: Capture refresh token cookie from MyBill backend response
                    // and forward it to the browser so token refresh works when JWT expires.
                    // This mirrors how LoginAsync captures OnlineServicesRefreshToken for MyVodafone.
                    if (response.StatusCode == HttpStatusCode.OK && response.Headers.Contains("Set-Cookie"))
                    {
                        try
                        {
                            var setCookieHeaders = response.Headers.GetValues("Set-Cookie");
                            foreach (var setCookieHeader in setCookieHeaders)
                            {
                                _logger.LogDebug("MyBill Login: Set-Cookie header received: {Header}",
                                    setCookieHeader.Length > 80 ? setCookieHeader.Substring(0, 80) + "..." : setCookieHeader);

                                // Forward the Set-Cookie header directly to the browser response
                                // This preserves the cookie name, value, path, httponly, secure, etc.
                                _httpContextAccessor?.HttpContext?.Response.Headers.Append("Set-Cookie", setCookieHeader);
                            }
                        }
                        catch (Exception cookieEx)
                        {
                            _logger.LogWarning(cookieEx, "MyBill Login: Error forwarding Set-Cookie headers from backend");
                        }
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        apiResponse.StatusCode = response.StatusCode;
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                    }
                }
                else
                {
                    apiResponse.StatusCode = response.StatusCode;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"{_myBillBaseUrl}/api/Login");
                throw;
            }

        }

        /// <summary>
        /// Forwards Set-Cookie headers from a MyBill backend response to the browser.
        /// This ensures refresh token cookies set by the backend are available for
        /// subsequent token refresh requests.
        /// </summary>
        private void ForwardSetCookieHeaders(HttpResponseMessage response, string callerContext)
        {
            if (response.Headers.Contains("Set-Cookie"))
            {
                try
                {
                    var setCookieHeaders = response.Headers.GetValues("Set-Cookie");
                    foreach (var header in setCookieHeaders)
                    {
                        _logger.LogDebug("MyBill {Context}: Forwarding Set-Cookie to browser", callerContext);
                        _httpContextAccessor?.HttpContext?.Response.Headers.Append("Set-Cookie", header);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "MyBill {Context}: Error forwarding Set-Cookie headers", callerContext);
                }
            }
        }

        private string ParseCookieValue(string setCookieHeader, string cookieName)
        {
            string cookieValue = string.Empty;
            foreach (string item in setCookieHeader.Split(';'))
            {
                if (item.Contains(cookieName))
                {
                    var parts = item.Split('=');
                    cookieValue = parts[0].Trim() == cookieName ? parts[1] : null;
                    break;
                }
            }
            return cookieValue;
        }

        /// <summary>
        /// Request a new JWT access token using the existing refresh token cookie
        /// The refresh token cookie must be forwarded from the browser request to the backend API
        /// </summary>
        public async Task<ApiResponse<T>> RequestNewAccessTokenAsync<T>()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_myBillBaseUrl}/api/Login/RequestNewAccessToken");
            
            // CRITICAL: Forward the refresh token cookie from the browser request to the MyBill backend
            // The MyBill backend expects the refresh token as an HTTP-only cookie
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext != null)
            {
                // Get all cookies from the incoming request and forward relevant ones
                var cookieHeader = httpContext.Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    httpRequest.Headers.Add("Cookie", cookieHeader);
                    _logger.LogInformation("MyBill AuthApiClient: Forwarding cookies to refresh token endpoint. Cookie header length: {Length}", cookieHeader.Length);
                }
                else
                {
                    _logger.LogWarning("MyBill AuthApiClient: No cookies found in request headers to forward");
                }
            }
            else
            {
                _logger.LogWarning("MyBill AuthApiClient: No HttpContext available - cannot forward cookies");
            }
            
            ApiResponse<T> apiResponse = new ApiResponse<T>();
            try
            {
                _logger.LogInformation("MyBill AuthApiClient: Calling {Url} to refresh access token", $"{_myBillBaseUrl}/api/Login/RequestNewAccessToken");
                
                var response = await _httpClient.SendAsync(httpRequest);
                
                if (response != null)
                {
                    // Forward any rotated refresh token cookies from backend to browser
                    ForwardSetCookieHeaders(response, "RequestNewAccessToken");

                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("MyBill AuthApiClient: Token refresh response status: {StatusCode}, Content length: {Length}", 
                        response.StatusCode, responseContent?.Length ?? 0);
                    
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        // Log first 200 chars of response for debugging (be careful not to log tokens)
                        var logContent = responseContent.Length > 200 ? responseContent.Substring(0, 200) + "..." : responseContent;
                        _logger.LogDebug("MyBill AuthApiClient: Response preview: {Content}", logContent);
                        
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        apiResponse.StatusCode = response.StatusCode;
                        return apiResponse;
                    }
                    else
                    {
                        _logger.LogWarning("MyBill AuthApiClient: Empty response content from token refresh");
                        apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                    }
                }
                else
                {
                    _logger.LogWarning("MyBill AuthApiClient: Null response from token refresh");
                    apiResponse.StatusCode = response.StatusCode;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GET {Url}", $"{_myBillBaseUrl}/api/Login/RequestNewAccessToken");
                throw;
            }
        }

        /// <summary>
        /// Login using a single-use access token (for customer support)
        /// Calls the backend API: /api/Login/LoginUsingSingleAccessToken?token={token}
        /// </summary> 
        public async Task<ApiResponse<T>> MyBillLoginUsingSingleAccessTokenAsync<T>(string token)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_myBillBaseUrl}/api/Login/LoginUsingSingleAccessToken?token={Uri.EscapeDataString(token)}");

            ApiResponse<T> apiResponse = new ApiResponse<T>();
            try
            {
                _logger.LogInformation("MyBill: Calling {Url} for single access token login", $"{_myBillBaseUrl}/api/Login/LoginUsingSingleAccessToken");
                
                var response = await _httpClient.SendAsync(httpRequest);
                
                if (response != null)
                {
                    // Forward refresh token cookies from backend to browser
                    ForwardSetCookieHeaders(response, "LoginUsingSingleAccessToken");

                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("MyBill: Single access token login response status: {StatusCode}", response.StatusCode);
                    
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        apiResponse.StatusCode = response.StatusCode;
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                    }
                }
                else
                {
                    apiResponse.StatusCode = response.StatusCode;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GET {Url}", $"{_myBillBaseUrl}/api/Login/LoginUsingSingleAccessToken");
                throw;
            }
        }


        public async Task<ApiResponse<T>> LoginUsingSingleAccessTokenAsync<T>(string token)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/Login/LoginUsingSingleAccessToken?token={Uri.EscapeDataString(token)}");

            ApiResponse<T> apiResponse = new ApiResponse<T>();
            try
            {
                _logger.LogInformation("MyBill: Calling {Url} for single access token login", $"{_myBillBaseUrl}/api/Login/LoginUsingSingleAccessToken");

                var response = await _httpClient.SendAsync(httpRequest);

                if (response != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("MyBill: Single access token login response status: {StatusCode}", response.StatusCode);

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        apiResponse.StatusCode = response.StatusCode;
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                    }
                }
                else
                {
                    apiResponse.StatusCode = response.StatusCode;
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GET {Url}", $"{_myBillBaseUrl}/api/Login/LoginUsingSingleAccessToken");
                throw;
            }
        }
    }
}
