using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Progress.Sitefinity.AspNetCore.Widgets.Models.Facets;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Config;
using VFL.Renderer.Models.LoginForm;
using VFL.Renderer.Services.TransactionHistroy.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VFL.Renderer.ApiClients
{
    public class WebClient : BaseApiClient
    {
        private bool _isRefreshing = false;
        private const string TokenClaim = "access_token";
        private readonly ApiSettings _apiSettings;
        public WebClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger logger, IMemoryCache cache, IHttpContextAccessor httpContextAccessor) : base(httpClient, options, logger, cache, httpContextAccessor)
        {
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/'));
            _apiSettings = options.Value;

        }

        /// <summary> 
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheDuration"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<ApiResponse<T>> GetWithTokenAsync<T>(string relativeUrl, string cacheKey, TimeSpan? cacheDuration = null)
        {

            //if (_cache.TryGetValue(cacheKey, out ApiResponse<T> cachedValue))
            //{
            //    _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            //    return (ApiResponse<T>)cachedValue;
            //}
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");

                var user = httpContext.User;
                if (user == null)
                    throw new InvalidOperationException("No user context found in HttpContext.");

                var token = GetBearerTokenFromContext();
                if (!string.IsNullOrEmpty(token))
                {
                    //throw new UnauthorizedAccessException("Access token missing. User not authenticated.");
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}{relativeUrl}");
                ApiResponse<T> apiResponse = new ApiResponse<T>();

                var response = await _httpClient.SendAsync(httpRequest);

                if (response.StatusCode == HttpStatusCode.Unauthorized && !_isRefreshing)
                {
                    _isRefreshing = true;
                    var refreshed = await RequestNewAccessTokenAsync();
                    _isRefreshing = false;

                    if (refreshed)
                    {
                        token = GetBearerTokenFromContext();
                        var retryRequest = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}{relativeUrl}");
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        response = await _httpClient.SendAsync(retryRequest);
                    }
                    else
                    {
                        await ClearSessionAsync(false);                        
                        if (!httpContext.User.Identity.IsAuthenticated)
                        {
                            bool isBackendUser = httpContext.User.IsInRole("Administrators");
                            if (!isBackendUser)
                            {                                
                                _httpContextAccessor.HttpContext.Response.Redirect(_apiSettings.LoginPath);
                            }
                        }
                        
                        _logger.LogWarning("Unable to refresh access token.");
                        apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                        return apiResponse;
                    }
                }

                if (response != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (apiResponse != null && cacheDuration.HasValue)
                            {
                                _cache.Set(cacheKey, apiResponse, cacheDuration.Value);
                            }
                        }
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
            catch(UnauthorizedAccessException uae)
            {
                _logger.LogError(uae, "Error calling POST Unauthorized {Url}", $"{_httpClient.BaseAddress}{relativeUrl}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"{_httpClient.BaseAddress}{relativeUrl}");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<ApiResponse<T>> PostWithTokenAsync<T>(string relativeUrl, object data,string type)
        {
            try
            {
                string url = $"{_httpClient.BaseAddress}{relativeUrl}";
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null)
                    throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");

                var user = httpContext.User;
                if (user == null)
                    throw new InvalidOperationException("No user context found in HttpContext.");

                var token = GetBearerTokenFromContext();

                if (string.IsNullOrEmpty(token))
                    throw new UnauthorizedAccessException("Access token missing. User not authenticated.");

                var httpRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, url);
                if (type == "application/json")
                {
                    httpRequest.Content = JsonContent.Create(data);
                }
                else if (type == "MultipartFormDataContent")
                {
                    httpRequest.Content = (MultipartFormDataContent)data;                    
                }
                else
                {
                    httpRequest.Content = new StringContent(JsonConvert.SerializeObject(data), null, "application/json");
                }

                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                
                ApiResponse<T> apiResponse = new ApiResponse<T>();

                var response = await _httpClient.SendAsync(httpRequest);
                // Handle expired token
                if (response.StatusCode == HttpStatusCode.Unauthorized && !_isRefreshing)
                {
                    _isRefreshing = true;
                    var refreshed = await RequestNewAccessTokenAsync();
                    _isRefreshing = false;

                    if (refreshed)
                    {
                        token = GetBearerTokenFromContext();
                        var retryRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, url);
                        if (type == "application/json")
                        {
                            retryRequest.Content = JsonContent.Create(data);
                        }
                        else if(type == "MultipartFormDataContent")
                        {
                            retryRequest.Content = (MultipartFormDataContent)data;
                        }
                        else
                        {
                            retryRequest.Content = new StringContent(JsonConvert.SerializeObject(data), null, "application/json");
                        }
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        response = await _httpClient.SendAsync(retryRequest);
                    }
                    else
                    {
                        await ClearSessionAsync(true);
                        httpContext.Response.Redirect("/login");
                        _logger.LogWarning("Unable to refresh access token.");
                        apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                        return apiResponse;
                    }
                }

                //response.EnsureSuccessStatusCode();
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
                _logger.LogError(ex, "Error calling POST {Url}", $"{_httpClient.BaseAddress}{relativeUrl}");
                throw;
            }
        }

        private async Task ClearSessionAsync(bool forceExpireCookies = false)
        {
            // Clear cookies
            if (forceExpireCookies)
            {
                var expiredOptions = new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(-1)
                };

                _httpContextAccessor.HttpContext.Response.Cookies.Append("OnlineServicesRefreshToken", "", expiredOptions);
                _httpContextAccessor.HttpContext.Response.Cookies.Append("luData", "", expiredOptions);
            }
            else
            {
                CookieHelper.DeleteCookie(_httpContextAccessor.HttpContext.Response, "luData");
                CookieHelper.DeleteCookie(_httpContextAccessor.HttpContext.Response, "OnlineServicesRefreshToken");
            }
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies", new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
            // Clear session
            _httpContextAccessor.HttpContext.Session.Clear();
        }


        // -------------------------------------------------------
        // Helper: Extract token from user claims
        // -------------------------------------------------------
        private string GetBearerTokenFromContext()
        {
            var token = _httpContextAccessor?.HttpContext?.User?
                .FindFirst(TokenClaim)?.Value;

            if (string.IsNullOrEmpty(token))
                _logger.LogWarning("No {TokenClaim} claim found in user context.", TokenClaim);

            return token ?? string.Empty;
        }

        // -------------------------------------------------------
        // Refresh Access Token and Update Cookie
        // -------------------------------------------------------
        public async Task<bool> RequestNewAccessTokenAsync()
        {
            try
            {
                var token = GetBearerTokenFromContext();
                var httpContext = _httpContextAccessor.HttpContext;
                var req = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{_httpClient.BaseAddress}api/Login/RequestNewAccessToken");
                //req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var OnlineServicesRefreshToken = CookieHelper.GetCookie(httpContext.Request, "OnlineServicesRefreshToken", true);
                if (OnlineServicesRefreshToken != null)
                {
                    var refreshTokenValue = ParseCookieValue(OnlineServicesRefreshToken, "OnlineServicesRefreshToken");
                    req.Headers.Add("Cookie", "OnlineServicesRefreshToken=" + OnlineServicesRefreshToken);
                    var response = await _httpClient.SendAsync(req);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Token refresh failed with status code {StatusCode}", response.StatusCode);
                        return false;
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var refResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (refResponse?.data == null || !refResponse.data.isLoggedIn)
                    {
                        _logger.LogWarning("Invalid or unsuccessful token refresh response.");
                        return false;
                    }


                    if (httpContext == null)
                    {
                        _logger.LogError("Error refreshing access token. No active HttpContext.");
                        return false;
                    }

                    var user = httpContext.User;
                    var claims = user.Claims.ToList();

                    // Replace access token claim
                    claims.RemoveAll(c => c.Type == "access_token");
                    claims.Add(new Claim("access_token", refResponse.data.jwtToken));

                    // Re-issue authentication cookie
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var cookieExpiry = DateTimeOffset.UtcNow.AddMinutes(_apiSettings.RefreshTokenExpiryMinutes);
                    await httpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = cookieExpiry
                        });

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing access token.");
                return false;
            }
        }
        private string ParseCookieValue(string setCookieHeader, string cookieName)
        {
            string cookieValue = string.Empty;
            if (!string.IsNullOrEmpty(setCookieHeader))
            {
                foreach (string item in setCookieHeader.Split(';'))
                {
                    if (item.Contains(cookieName))
                    {
                        var parts = item.Split('=');
                        cookieValue = parts[0].Trim() == cookieName ? parts[1] : null;
                        break;
                    }
                }
             
            }
            return cookieValue;
        }
        public async Task<ApiResponse<T>> PostWithTokenGraphQLAsync<T>(string query, object variables = null)
        {
            try
            {
                string url = $"{_httpClient.BaseAddress}graphql/";
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null)
                    throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");

                var user = httpContext.User;
                if (user == null)
                    throw new InvalidOperationException("No user context found in HttpContext.");

                var token = GetBearerTokenFromContext();
                if (string.IsNullOrEmpty(token))
                    throw new UnauthorizedAccessException("Access token missing. User not authenticated.");

                var graphQLRequest = new
                {
                    query = query,
                    variables = variables ?? new { }
                };
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                string para = JsonConvert.SerializeObject(graphQLRequest, settings);

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
                httpRequest.Content = new StringContent(para, null, "application/json"); //JsonContent.Create(graphQLRequest);


               // httpRequest.Content = new StringContent("{\"query\":\"query(\\r\\n  $first:Int!, \\r\\n  $after: String, \\r\\n  $where:PrePurchasesDtoFilterInput\\r\\n  ){\\r\\n  allDirectTopUps(first: $first, after: $after,  where: $where, order: [ {\\r\\n     serviceDate: DESC\\r\\n  }]){\\r\\n    totalCount\\r\\n      edges{\\r\\n            cursor\\r\\n            node{\\r\\n                 serviceDate\\r\\n                 number\\r\\n                 reference\\r\\n                 pin\\r\\n                 completionStatusName\\r\\n            }\\r\\n      }\\r\\n      pageInfo{\\r\\n        endCursor\\r\\n        hasNextPage\\r\\n      }\\r\\n  }\\r\\n}\",\"variables\":{\"first\":2,\"after\":null,\"where\":{\"or\":[{\"number\":{\"contains\":\"8063689\"}}]}}}", null, "application/json");
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(httpRequest);

                // Handle expired token
                if (response.StatusCode == HttpStatusCode.Unauthorized && !_isRefreshing)
                {
                    _isRefreshing = true;
                    var refreshed = await RequestNewAccessTokenAsync();
                    _isRefreshing = false;

                    if (refreshed)
                    {
                        token = GetBearerTokenFromContext();
                        var retryRequest = new HttpRequestMessage(HttpMethod.Post, url);
                        retryRequest.Content = JsonContent.Create(graphQLRequest);
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        response = await _httpClient.SendAsync(retryRequest);
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("Session expired. Please log in again.");
                    }
                }

                if (response != null && response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var result = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        return result;
                    }
                }

                var errorContent = response != null ? await response.Content.ReadAsStringAsync() : "No response";
                _logger.LogError("GraphQL request failed. Status: {StatusCode}, Content: {Content}",
                    response?.StatusCode,
                    errorContent);
                throw new HttpRequestException($"GraphQL request failed with status code {response?.StatusCode}. Response: {errorContent}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GraphQL endpoint");
                throw;
            }
        }

        

        /// <summary>
        /// GraphQL request with token authentication
        /// </summary>
        public async Task<ApiResponse<T>> PostGraphQLAsync<T>(string query, object variables = null)
        {
            try
            {
                string url = $"{_httpClient.BaseAddress}graphql/";
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null)
                    throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");
                
                var graphQLRequest = new
                {
                    query = query,
                    variables = variables ?? new { }
                };

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
                httpRequest.Content = JsonContent.Create(graphQLRequest);                

                var response = await _httpClient.SendAsync(httpRequest);

                if (response != null && response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var result = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        return result;
                    }
                }

                var errorContent = response != null ? await response.Content.ReadAsStringAsync() : "No response";
                _logger.LogError("GraphQL request failed. Status: {StatusCode}, Content: {Content}",
                    response?.StatusCode,
                    errorContent);
                throw new HttpRequestException($"GraphQL request failed with status code {response?.StatusCode}. Response: {errorContent}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GraphQL endpoint");
                throw;
            }
        }

        public async Task<ApiResponse<T>> GetWithoutTokenAsync<T>(string relativeUrl)
        {
            try
            {
                string url = $"{_httpClient.BaseAddress}{relativeUrl}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                ApiResponse<T> apiResponse = new ApiResponse<T>();

                var response = await _httpClient.SendAsync(request);

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
                        apiResponse.StatusCode = HttpStatusCode.NoContent;
                    }
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GET {Url}", relativeUrl);
                throw;
            }
        }

        //public async Task<ApiResponse<T>> PostWithTokenGraphQLAsync<T>(string query, object variables = null)
        //{
        //    try
        //    {
        //        string url = $"{_httpClient.BaseAddress}graphql/";
        //        var httpContext = _httpContextAccessor.HttpContext;

        //        _logger.LogInformation("WebClient: Starting GraphQL request to {Url}", url);

        //        if (httpContext == null)
        //        {
        //            _logger.LogError("WebClient: No active HttpContext");
        //            throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");
        //        }

        //        var user = httpContext.User;
        //        if (user == null)
        //        {
        //            _logger.LogError("WebClient: No user context found in HttpContext");
        //            throw new InvalidOperationException("No user context found in HttpContext.");
        //        }

        //        _logger.LogInformation("WebClient: User authenticated: {IsAuth}, AuthType: {AuthType}",
        //            user.Identity?.IsAuthenticated,
        //            user.Identity?.AuthenticationType);

        //        var token = GetBearerTokenFromContext();
        //        if (string.IsNullOrEmpty(token))
        //        {
        //            _logger.LogError("WebClient:  access token missing");
        //            throw new UnauthorizedAccessException(" access token missing. User not authenticated.");
        //        }

        //        _logger.LogInformation("WebClient: Token retrieved successfully, length: {Length}", token.Length);

        //        var graphQLRequest = new
        //        {
        //            query = query,
        //            variables = variables ?? new { }
        //        };
        //        JsonSerializerSettings settings = new JsonSerializerSettings();
        //        settings.NullValueHandling = NullValueHandling.Ignore;
        //        string para = JsonConvert.SerializeObject(graphQLRequest, settings);
        //        _logger.LogInformation("WebClient: GraphQL Query: {Query}", query.Replace("\r\n", " ").Replace("\n", " ").Trim());

        //        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        //        httpRequest.Content = new StringContent(para, null, "application/json"); //JsonContent.Create(graphQLRequest);


        //        // httpRequest.Content = new StringContent("{\"query\":\"query(\\r\\n  $first:Int!, \\r\\n  $after: String, \\r\\n  $where:PrePurchasesDtoFilterInput\\r\\n  ){\\r\\n  allDirectTopUps(first: $first, after: $after,  where: $where, order: [ {\\r\\n     serviceDate: DESC\\r\\n  }]){\\r\\n    totalCount\\r\\n      edges{\\r\\n            cursor\\r\\n            node{\\r\\n                 serviceDate\\r\\n                 number\\r\\n                 reference\\r\\n                 pin\\r\\n                 completionStatusName\\r\\n            }\\r\\n      }\\r\\n      pageInfo{\\r\\n        endCursor\\r\\n        hasNextPage\\r\\n      }\\r\\n  }\\r\\n}\",\"variables\":{\"first\":2,\"after\":null,\"where\":{\"or\":[{\"number\":{\"contains\":\"8063689\"}}]}}}", null, "application/json");
        //        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        _logger.LogInformation("WebClient: Sending GraphQL request...");
        //        var response = await _httpClient.SendAsync(httpRequest);
        //        _logger.LogInformation("WebClient: Received response with status code: {StatusCode}", response.StatusCode);

        //        // Handle expired token
        //        if (response.StatusCode == HttpStatusCode.Unauthorized && !_isRefreshing)
        //        {
        //            _logger.LogWarning("WebClient: Unauthorized response, attempting token refresh");
        //            _isRefreshing = true;
        //            var refreshed = await RequestNewAccessTokenAsync();
        //            _isRefreshing = false;

        //            if (refreshed)
        //            {
        //                _logger.LogInformation("WebClient: Token refreshed, retrying GraphQL request");
        //                token = GetBearerTokenFromContext();
        //                var retryRequest = new HttpRequestMessage(HttpMethod.Post, url);
        //                retryRequest.Content = JsonContent.Create(graphQLRequest);
        //                retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //                response = await _httpClient.SendAsync(retryRequest);
        //                _logger.LogInformation("WebClient: Retry response status code: {StatusCode}", response.StatusCode);
        //            }
        //            else
        //            {
        //                _logger.LogError("WebClient: Token refresh failed - refresh token likely expired");
        //                // For GraphQL, we need to throw since return type is T not ApiResponse
        //                // But controller should catch this and return proper response
        //                throw new UnauthorizedAccessException(" session expired. Please log in again.");
        //            }
        //        }

        //        if (response != null && response.IsSuccessStatusCode)
        //        {
        //            var responseContent = await response.Content.ReadAsStringAsync();
        //            _logger.LogInformation("WebClient: Response content length: {Length}", responseContent?.Length ?? 0);

        //            if (!string.IsNullOrEmpty(responseContent))
        //            {
        //                // Log first 500 chars of response for debugging
        //                var preview = responseContent.Length > 500 ? responseContent.Substring(0, 500) + "..." : responseContent;
        //                _logger.LogInformation("WebClient: Response preview: {Preview}", preview);

        //                var result = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
        //                _logger.LogInformation("WebClient: GraphQL request successful, data deserialized");
        //                return result;
        //            }
        //            else
        //            {
        //                _logger.LogError("WebClient: Response content is empty");
        //            }
        //        }

        //        var errorContent = response != null ? await response.Content.ReadAsStringAsync() : "No response";
        //        _logger.LogError("WebClient: GraphQL request failed. Status: {StatusCode}, Content: {Content}",
        //            response?.StatusCode,
        //            errorContent);
        //        throw new HttpRequestException($"GraphQL request failed with status code {response?.StatusCode}. Response: {errorContent}");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "WebClient: Error calling GraphQL endpoint: {Message}", ex.Message);
        //        throw;
        //    }
        //}



        ///// <summary>
        ///// GraphQL request with token authentication
        ///// </summary>
        //public async Task<ApiResponse<T>> PostGraphQLAsync<T>(string query, object variables = null)
        //{
        //    try
        //    {
        //        string url = $"{_httpClient.BaseAddress}graphql/";
        //        var httpContext = _httpContextAccessor.HttpContext;

        //        _logger.LogInformation("WebClient: Starting GraphQL request to {Url}", url);

        //        if (httpContext == null)
        //        {
        //            _logger.LogError("WebClient: No active HttpContext");
        //            throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");
        //        }

        //        var graphQLRequest = new
        //        {
        //            query = query,
        //            variables = variables ?? new { }
        //        };

        //        _logger.LogInformation("WebClient: GraphQL Query: {Query}", query.Replace("\r\n", " ").Replace("\n", " ").Trim());

        //        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        //        httpRequest.Content = JsonContent.Create(graphQLRequest);

        //        _logger.LogInformation("WebClient: Sending GraphQL request...");
        //        var response = await _httpClient.SendAsync(httpRequest);
        //        _logger.LogInformation("WebClient: Received response with status code: {StatusCode}", response.StatusCode);

        //        if (response != null && response.IsSuccessStatusCode)
        //        {
        //            var responseContent = await response.Content.ReadAsStringAsync();
        //            _logger.LogInformation("WebClient: Response content length: {Length}", responseContent?.Length ?? 0);

        //            if (!string.IsNullOrEmpty(responseContent))
        //            {
        //                // Log first 500 chars of response for debugging
        //                var preview = responseContent.Length > 500 ? responseContent.Substring(0, 500) + "..." : responseContent;
        //                _logger.LogInformation("WebClient: Response preview: {Preview}", preview);

        //                var result = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
        //                _logger.LogInformation("WebClient: GraphQL request successful, data deserialized");
        //                return result;
        //            }
        //            else
        //            {
        //                _logger.LogError("WebClient: Response content is empty");
        //            }
        //        }

        //        var errorContent = response != null ? await response.Content.ReadAsStringAsync() : "No response";
        //        _logger.LogError("WebClient: GraphQL request failed. Status: {StatusCode}, Content: {Content}",
        //            response?.StatusCode,
        //            errorContent);
        //        throw new HttpRequestException($"GraphQL request failed with status code {response?.StatusCode}. Response: {errorContent}");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "WebClient: Error calling GraphQL endpoint: {Message}", ex.Message);
        //        throw;
        //    }
        //}

        //public async Task<ApiResponse<T>> GetWithoutTokenAsync<T>(string relativeUrl)
        //{
        //    try
        //    {
        //        string url = $"{_httpClient.BaseAddress}{relativeUrl}";

        //        var request = new HttpRequestMessage(HttpMethod.Get, url);

        //        ApiResponse<T> apiResponse = new ApiResponse<T>();

        //        var response = await _httpClient.SendAsync(request);

        //        if (response != null)
        //        {
        //            var responseContent = await response.Content.ReadAsStringAsync();

        //            if (!string.IsNullOrEmpty(responseContent))
        //            {
        //                apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
        //                apiResponse.StatusCode = response.StatusCode;

        //                return apiResponse;
        //            }
        //            else
        //            {
        //                apiResponse.StatusCode = HttpStatusCode.NoContent;
        //            }
        //        }

        //        return apiResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error calling GET {Url}", relativeUrl);
        //        throw;
        //    }
        //}

    }
}
