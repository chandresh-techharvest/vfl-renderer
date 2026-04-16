using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
using VFL.Renderer.Models.MyBillLoginForm;

namespace VFL.Renderer.ApiClients
{
    /// <summary>
    /// Authenticated API client for MyBill portal
    /// Uses MyBillBaseUrl and MyBillAuth authentication scheme
    /// </summary>
    public class MyBillWebClient : MyBillBaseApiClient
    {
        private static readonly object _refreshLock = new object();
        private static volatile bool _isRefreshing = false;
        private const string MyBillAuthScheme = "MyBillAuth";
        private const string MyBillTokenClaim = "access_tokenMyBill";
        private readonly ApiSettings _apiSettings;
        
        // Store the refreshed token temporarily until the next request cycle
        private string _refreshedToken = null;

        public MyBillWebClient(HttpClient httpClient, IOptions<ApiSettings> options, ILogger<MyBillWebClient> logger, IMemoryCache cache, IHttpContextAccessor httpContextAccessor) 
            : base(httpClient, options, logger, cache, httpContextAccessor)
        {
            _httpClient.BaseAddress = new Uri(options.Value.MyBillBaseUrl.TrimEnd('/'));
            _apiSettings = options.Value;
        }

        /// <summary>
        /// GET request with MyBill token authentication
        /// </summary>
        public async Task<ApiResponse<T>> GetWithTokenAsync<T>(string relativeUrl, string cacheKey, TimeSpan? cacheDuration = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");

                var user = httpContext.User;
                if (user == null)
                    throw new InvalidOperationException("No user context found in HttpContext.");

                var token = GetMyBillTokenFromContext();
                if (!string.IsNullOrEmpty(token))
                    // throw new UnauthorizedAccessException("MyBill access token missing. User not authenticated.");
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string url = $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/{relativeUrl.TrimStart('/')}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                ApiResponse<T> apiResponse = new ApiResponse<T>();

                var response = await _httpClient.SendAsync(httpRequest);

                if (response.StatusCode == HttpStatusCode.Unauthorized && !_isRefreshing)
                {
                    _isRefreshing = true;
                    var refreshed = await RequestNewAccessTokenAsync();
                    _isRefreshing = false;

                    if (refreshed)
                    {
                        token = GetMyBillTokenFromContext();
                        var retryRequest = new HttpRequestMessage(HttpMethod.Get, url);
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        response = await _httpClient.SendAsync(retryRequest);
                    }
                    else
                    {
                        _logger.LogWarning("MyBillWebClient: Token refresh failed - refresh token likely expired");
                        // Return 401 response instead of throwing - let client-side handle redirect
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
                        if (response.StatusCode == HttpStatusCode.OK)
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
                        apiResponse.StatusCode = HttpStatusCode.NoContent;
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
                _logger.LogError(ex, "MyBill: Error calling GET {Url}", $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/{relativeUrl.TrimStart('/')}");
                throw;
            }
        }

        /// <summary>
        /// POST request with MyBill token authentication
        /// </summary>
        public async Task<ApiResponse<T>> PostWithTokenAsync<T>(string relativeUrl, object data, string type)
        {
            try
            {
                string url = $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/{relativeUrl.TrimStart('/')}";
                var httpContext = _httpContextAccessor.HttpContext;
                
                if (httpContext == null)
                    throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");

                var user = httpContext.User;
                
                if (user == null)
                    throw new InvalidOperationException("No user context found in HttpContext.");

                var token = GetMyBillTokenFromContext();
                
                if (string.IsNullOrEmpty(token))
                    throw new UnauthorizedAccessException("MyBill access token missing. User not authenticated.");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
                if (type == "application/json")
                {
                    httpRequest.Content = JsonContent.Create(data);
                }
                else
                {
                    httpRequest.Content = (MultipartFormDataContent)data;
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
                        token = GetMyBillTokenFromContext();
                        var retryRequest = new HttpRequestMessage(HttpMethod.Post, url);
                        if (type == "application/json")
                        {
                            retryRequest.Content = JsonContent.Create(data);
                        }
                        else
                        {
                            retryRequest.Content = (MultipartFormDataContent)data;
                        }
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        response = await _httpClient.SendAsync(retryRequest);
                    }
                    else
                    {
                        _logger.LogWarning("MyBillWebClient: Token refresh failed - refresh token likely expired");
                        // Return 401 response instead of throwing - let client-side handle redirect
                        apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                        return apiResponse;
                    }
                }

                if (response != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    // Log error responses with full details for debugging
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("MyBillWebClient POST to {Url} failed with status {StatusCode}. Full Response: {Response}", 
                            url, response.StatusCode, responseContent);
                    }
                    
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        // Try to deserialize as ApiResponse first
                        try
                        {
                            apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
                        }
                        catch (Newtonsoft.Json.JsonException jsonEx)
                        {
                            // If deserialization fails, create a response with the raw error
                            _logger.LogWarning(jsonEx, "MyBillWebClient: Could not deserialize response as ApiResponse<T>. Raw: {Raw}", 
                                responseContent?.Substring(0, Math.Min(1000, responseContent?.Length ?? 0)));
                            apiResponse = new ApiResponse<T>();
                        }
                        
                        apiResponse.StatusCode = response.StatusCode;
                        
                        // For 400 Bad Request, log validation errors prominently
                        if (response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            _logger.LogError("MyBillWebClient: VALIDATION ERROR from {Url}. Status: 400. Details: {Response}", 
                                url, responseContent);
                        }
                        
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.StatusCode = HttpStatusCode.NoContent;
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
                _logger.LogError(ex, "MyBill: Error calling POST {Url}", $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/{relativeUrl.TrimStart('/')}");
                throw;
            }
        }

        /// <summary>
        /// PUT request with MyBill token authentication
        /// </summary>
        public async Task<ApiResponse<T>> PutWithTokenAsync<T>(string relativeUrl, object data, string type)
        {
            try
            {
                string url = $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/{relativeUrl.TrimStart('/')}";
                var httpContext = _httpContextAccessor.HttpContext;
                
                if (httpContext == null)
                    throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");

                var user = httpContext.User;
                if (user == null)
                    throw new InvalidOperationException("No user context found in HttpContext.");

                var token = GetMyBillTokenFromContext();
                
                if (string.IsNullOrEmpty(token))
                    throw new UnauthorizedAccessException("MyBill access token missing. User not authenticated.");

                var httpRequest = new HttpRequestMessage(HttpMethod.Put, url);
                if (type == "application/json")
                {
                    httpRequest.Content = JsonContent.Create(data);
                }
                else
                {
                    httpRequest.Content = (MultipartFormDataContent)data;
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
                        token = GetMyBillTokenFromContext();
                        var retryRequest = new HttpRequestMessage(HttpMethod.Put, url);
                        if (type == "application/json")
                        {
                            retryRequest.Content = JsonContent.Create(data);
                        }
                        else
                        {
                            retryRequest.Content = (MultipartFormDataContent)data;
                        }
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        response = await _httpClient.SendAsync(retryRequest);
                    }
                    else
                    {
                        _logger.LogWarning("MyBillWebClient: Token refresh failed - refresh token likely expired");
                        // Return 401 response instead of throwing - let client-side handle redirect
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
                        apiResponse.StatusCode = response.StatusCode;
                        return apiResponse;
                    }
                    else
                    {
                        apiResponse.StatusCode = HttpStatusCode.NoContent;
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
                _logger.LogError(ex, "MyBill: Error calling PUT {Url}", $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/{relativeUrl.TrimStart('/')}");
                throw;
            }
        }

        /// <summary>
        /// GraphQL request with MyBill token authentication
        /// </summary>
        public async Task<T> PostGraphQLAsync<T>(string query, object variables = null)
        {
            try
            {
                string url = $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/graphql/";
                var httpContext = _httpContextAccessor.HttpContext;
                
                if (httpContext == null)
                    throw new InvalidOperationException("No active HttpContext. This method requires an HTTP request context.");

                var user = httpContext.User;
                if (user == null)
                    throw new InvalidOperationException("No user context found in HttpContext.");

                var token = GetMyBillTokenFromContext();
                if (string.IsNullOrEmpty(token))
                    throw new UnauthorizedAccessException("MyBill access token missing. User not authenticated.");

                var graphQLRequest = new
                {
                    query = query,
                    variables = variables ?? new { }
                };

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
                httpRequest.Content = JsonContent.Create(graphQLRequest);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(httpRequest);

                // Handle expired token - attempt refresh
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    
                    bool shouldRefresh = false;
                    lock (_refreshLock)
                    {
                        if (!_isRefreshing)
                        {
                            _isRefreshing = true;
                            shouldRefresh = true;
                        }
                    }

                    if (shouldRefresh)
                    {
                        try
                        {
                            var refreshed = await RequestNewAccessTokenAsync();
                            
                            if (refreshed)
                            {
                                token = GetMyBillTokenFromContext();
                                
                                var retryRequest = new HttpRequestMessage(HttpMethod.Post, url);
                                retryRequest.Content = JsonContent.Create(graphQLRequest);
                                retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                response = await _httpClient.SendAsync(retryRequest);
                            }
                            else
                            {
                                _logger.LogWarning("MyBillWebClient: Token refresh failed - session expired");
                                throw new UnauthorizedAccessException("MyBill session expired. Please log in again.");
                            }
                        }
                        finally
                        {
                            lock (_refreshLock)
                            {
                                _isRefreshing = false;
                            }
                        }
                    }
                    else
                    {
                        // Another thread is refreshing, wait a bit and retry
                        await Task.Delay(1000);
                        
                        token = GetMyBillTokenFromContext();
                        var retryRequest = new HttpRequestMessage(HttpMethod.Post, url);
                        retryRequest.Content = JsonContent.Create(graphQLRequest);
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        response = await _httpClient.SendAsync(retryRequest);
                    }
                }

                if (response != null && response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var result = JsonConvert.DeserializeObject<T>(responseContent);
                        return result;
                    }
                    else
                    {
                        _logger.LogError("MyBillWebClient: Response content is empty");
                    }
                }

                var errorContent = response != null ? await response.Content.ReadAsStringAsync() : "No response";
                _logger.LogError("MyBillWebClient: GraphQL request failed. Status: {StatusCode}, Error: {Error}", 
                    response?.StatusCode, errorContent?.Substring(0, Math.Min(500, errorContent?.Length ?? 0)));
                throw new HttpRequestException($"GraphQL request failed with status code {response?.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error calling GraphQL endpoint");
                throw;
            }
        }

        /// <summary>
        /// Extract MyBill token from user claims or use refreshed token if available
        /// </summary>
        private string GetMyBillTokenFromContext()
        {
            // First check if we have a freshly refreshed token (from token refresh in same request)
            if (!string.IsNullOrEmpty(_refreshedToken))
            {
                return _refreshedToken;
            }
            
            var token = _httpContextAccessor?.HttpContext?.User?
                .FindFirst(MyBillTokenClaim)?.Value;

            return token ?? string.Empty;
        }

        /// <summary>
        /// Get the base URL for MyBill backend API
        /// </summary>
        public string GetBaseUrl()
        {
            return _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
        }

        /// <summary>
        /// Validate if the current MyBill token is still valid
        /// Returns true if token is valid and user is authenticated
        /// </summary>
        public async Task<bool> ValidateTokenAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
                    return false;

                // Check if using MyBillAuth scheme
                if (httpContext.User.Identity.AuthenticationType != "MyBillAuth")
                    return false;

                var token = GetMyBillTokenFromContext();
                if (string.IsNullOrEmpty(token))
                    return false;

                // Make a lightweight API call to validate token
                const string validationQuery = @"query { allAccountsByPrimary { primaryAccountName } }";
                
                string url = $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/graphql/";
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
                var graphQLRequest = new { query = validationQuery, variables = new { } };
                httpRequest.Content = JsonContent.Create(graphQLRequest);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var cts = new System.Threading.CancellationTokenSource();
                cts.CancelAfter(5000);
                
                try
                {
                    var response = await _httpClient.SendAsync(httpRequest, cts.Token);
                    return response.IsSuccessStatusCode;
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("MyBillWebClient: Token validation timed out");
                    return false;
                }
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "MyBillWebClient: Token validation timed out");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBillWebClient: Error validating token");
                return false;
            }
        }

        /// <summary>
        /// Refresh MyBill access token and update cookie
        /// </summary>
        public async Task<bool> RequestNewAccessTokenAsync()
        {
            try
            {
                string url = $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/api/Login/RequestNewAccessToken";
                var req = new HttpRequestMessage(HttpMethod.Get, url);
                
                // CRITICAL: Forward the refresh token cookie from the browser request to the MyBill backend
                var httpContext = _httpContextAccessor?.HttpContext;
                if (httpContext != null)
                {
                    var cookieHeader = httpContext.Request.Headers["Cookie"].ToString();
                    if (!string.IsNullOrEmpty(cookieHeader))
                    {
                        req.Headers.Add("Cookie", cookieHeader);
                    }
                    else
                    {
                        _logger.LogWarning("MyBillWebClient: No cookies found to forward for token refresh");
                    }
                }
                else
                {
                    _logger.LogWarning("MyBillWebClient: No HttpContext available for token refresh");
                }

                var response = await _httpClient.SendAsync(req);
                
                // Forward any rotated refresh token cookies from backend to browser
                if (response.Headers.Contains("Set-Cookie") && httpContext != null)
                {
                    try
                    {
                        var setCookieHeaders = response.Headers.GetValues("Set-Cookie");
                        foreach (var header in setCookieHeaders)
                        {
                            httpContext.Response.Headers.Append("Set-Cookie", header);
                        }
                    }
                    catch (Exception cookieEx)
                    {
                        _logger.LogWarning(cookieEx, "MyBillWebClient: Error forwarding Set-Cookie headers during token refresh");
                    }
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("MyBill token refresh failed with status code {StatusCode}. Response: {Response}", 
                        response.StatusCode, responseContent?.Substring(0, Math.Min(500, responseContent?.Length ?? 0)));
                    return false;
                }

                var refResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<MyBillLoginResponse>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (refResponse?.data == null || !refResponse.data.isLoggedIn || string.IsNullOrEmpty(refResponse.data.jwtToken))
                {
                    _logger.LogWarning("Invalid or unsuccessful MyBill token refresh response. isLoggedIn: {IsLoggedIn}, hasToken: {HasToken}", 
                        refResponse?.data?.isLoggedIn, !string.IsNullOrEmpty(refResponse?.data?.jwtToken));
                    return false;
                }

                if (httpContext == null)
                {
                    _logger.LogError("Error refreshing MyBill access token. No active HttpContext.");
                    return false;
                }

                // CRITICAL: Store the new token for immediate use in the current request
                // This is needed because the cookie won't be available until the next request
                _refreshedToken = refResponse.data.jwtToken;

                var user = httpContext.User;
                var claims = user.Claims.ToList();

                // Replace MyBill access token claim
                claims.RemoveAll(c => c.Type == MyBillTokenClaim);
                claims.Add(new Claim(MyBillTokenClaim, refResponse.data.jwtToken));

                // Re-issue MyBill authentication cookie
                var identity = new ClaimsIdentity(claims, MyBillAuthScheme);
                var principal = new ClaimsPrincipal(identity);

                // IMPORTANT: Cookie expiry MUST match the backend refresh token expiry
                var cookieExpiry = DateTimeOffset.UtcNow.AddMinutes(_apiSettings.MyBillRefreshTokenExpiryMinutes);

                await httpContext.SignInAsync(
                    MyBillAuthScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = cookieExpiry
                    });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing MyBill access token.");
                return false;
            }
        }

        /// <summary>
        /// POST request that forwards cookies from browser for authentication
        /// Used for payment verification callbacks where user context may not be available
        /// but cookies (including refresh token) are still present in the browser
        /// </summary>
        public async Task<ApiResponse<T>> PostWithCookiesAsync<T>(string relativeUrl, object data)
        {
            try
            {
                string url = $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/{relativeUrl.TrimStart('/')}";
                var httpContext = _httpContextAccessor?.HttpContext;

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
                httpRequest.Content = JsonContent.Create(data);

                // Try to get token from user context first (if user is authenticated)
                var token = GetMyBillTokenFromContext();
                
                // If no token from context, try to extract from the MyBillAuth cookie directly
                if (string.IsNullOrEmpty(token) && httpContext != null)
                {
                    // The MyBillAuth cookie contains encrypted authentication data
                    // We need to let the authentication middleware handle this
                    // For now, try authenticating the user using the existing cookie
                    try
                    {
                        var authenticateResult = await httpContext.AuthenticateAsync("MyBillAuth");
                        if (authenticateResult.Succeeded && authenticateResult.Principal != null)
                        {
                            token = authenticateResult.Principal.FindFirst(MyBillTokenClaim)?.Value;
                        }
                    }
                    catch (Exception authEx)
                    {
                        _logger.LogWarning(authEx, "MyBillWebClient: Error authenticating from cookie");
                    }
                }

                if (!string.IsNullOrEmpty(token))
                {
                    httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    _logger.LogWarning("MyBillWebClient: No token available for payment verification request");
                    // Still try to forward cookies as fallback
                    if (httpContext != null)
                    {
                        var cookieHeader = httpContext.Request.Headers["Cookie"].ToString();
                        if (!string.IsNullOrEmpty(cookieHeader))
                        {
                            httpRequest.Headers.Add("Cookie", cookieHeader);
                        }
                    }
                }

                ApiResponse<T> apiResponse = new ApiResponse<T>();
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
                        apiResponse.StatusCode = HttpStatusCode.NoContent;
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
                _logger.LogError(ex, "MyBill: Error calling POST with cookies {Url}", relativeUrl);
                throw;
            }
        }
    }
}
