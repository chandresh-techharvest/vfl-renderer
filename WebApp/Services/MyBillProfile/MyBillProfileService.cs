using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;

namespace VFL.Renderer.Services.MyBillProfile
{
    /// <summary>
    /// Service for MyBill user profile operations
    /// Uses MyBillWebClient for authenticated GraphQL API calls
    /// </summary>
    public class MyBillProfileService : IMyBillProfileService
    {
        private readonly MyBillWebClient _myBillWebClient;
        private readonly ILogger<MyBillProfileService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyBillProfileService(
            MyBillWebClient myBillWebClient, 
            ILogger<MyBillProfileService> logger,
            IMemoryCache cache,
            IHttpContextAccessor httpContextAccessor)
        {
            _myBillWebClient = myBillWebClient;
            _logger = logger;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the current authenticated username from claims
        /// Returns empty string if not authenticated
        /// </summary>
        private string GetCurrentUsername()
        {
            return _httpContextAccessor?.HttpContext?.User?.Claims
                ?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Gets profile information for the authenticated MyBill user
        /// Now uses GraphQL API directly instead of REST endpoint
        /// Includes BAN accounts, company info, etc.
        /// Caches for 15 minutes to reduce backend calls while keeping data fresh
        /// </summary>
        public async Task<ApiResponse<T>> GetProfileInformationAsync<T>()
        {
            try
            {
                string username = GetCurrentUsername();
                string cacheKey = $"MyBillProfileInformation_{username}";
                
                // Check cache first
                if (_cache.TryGetValue(cacheKey, out ApiResponse<T> cachedValue))
                {
                    return cachedValue;
                }

                // Use GraphQL to get account information
                var graphQLResponse = await GetAllAccountsByPrimaryAsync<Services.MyBillProfile.Models.GraphQLAccountsResponse>();
                
                // Create a wrapper response in the expected format
                var apiResponse = new ApiResponse<T>
                {
                    data = (T)(object)graphQLResponse,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

                // Cache the result for 15 minutes (reduced from 6 hours to prevent stale data)
                var cacheDuration = System.TimeSpan.FromMinutes(15);
                _cache.Set(cacheKey, apiResponse, cacheDuration);
                
                return apiResponse;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error calling GetProfileInformationAsync");
                throw;
            }
        }

        /// <summary>
        /// Gets all accounts by primary user from GraphQL API
        /// Includes account numbers, names, and paperless emails
        /// This is the primary method for fetching account information
        /// Cached for 15 minutes to improve performance while keeping data fresh
        /// </summary>
        public async Task<T> GetAllAccountsByPrimaryAsync<T>()
        {
            try
            {
                string username = GetCurrentUsername();
                string cacheKey = $"MyBillAllAccountsByPrimary_{username}";
                
                _logger.LogDebug("MyBill: GetAllAccountsByPrimaryAsync called for user {Username}", username);
                
                // Check cache first
                if (_cache.TryGetValue(cacheKey, out T cachedValue))
                {
                    return cachedValue;
                }

                const string query = @"
                    query {
                        allAccountsByPrimary {
                            primaryAccountName
                            contactFullName
                            phoneNumber
                            email
                            businessAccountNumbers {
                                accountName
                                number
                                paperlessEmails {
                                    email
                                    id
                                }
                            }
                        }
                    }";

                try
                {
                    var response = await _myBillWebClient.PostGraphQLAsync<T>(query);
                    
                    // Cache the result for 15 minutes (reduced from 1 hour to prevent stale data)
                    var cacheDuration = System.TimeSpan.FromMinutes(15);
                    _cache.Set(cacheKey, response, cacheDuration);
                    
                    return response;
                }
                catch (System.Net.Http.HttpRequestException httpEx)
                {
                    _logger.LogError(httpEx, "MyBill: HTTP request failed in GetAllAccountsByPrimaryAsync for user {Username}. Status: {StatusCode}, Message: {Message}", 
                        username, 
                        httpEx.StatusCode, 
                        httpEx.Message);
                    throw new System.Exception($"Failed to fetch account data: {httpEx.Message}", httpEx);
                }
                catch (UnauthorizedAccessException authEx)
                {
                    _logger.LogError(authEx, "MyBill: Unauthorized access in GetAllAccountsByPrimaryAsync for user {Username}", username);
                    throw;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error calling GetAllAccountsByPrimaryAsync. Exception type: {Type}, Message: {Message}", 
                    ex.GetType().Name, 
                    ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets invoices for a specific BAN from GraphQL API
        /// Includes invoice details, amounts, dates, payment status
        /// No caching as invoice data can change frequently
        /// </summary>
        public async Task<T> GetInvoicesByBanAsync<T>(string banNumber, bool getFiles = true)
        {
            try
            {
                var query = $@"
                    query {{
                        invoices(ban: ""{banNumber}"", getFiles: {getFiles.ToString().ToLower()}) {{
                            number
                            amount
                            billingCycleStartDate
                            billingCycleEndDate
                            dueDate
                            fileName
                            paymentStatus
                            preBalance
                        }}
                    }}";

                var response = await _myBillWebClient.PostGraphQLAsync<T>(query);
                
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error calling GetInvoicesByBanAsync for BAN {BanNumber}", banNumber);
                throw;
            }
        }

        /// <summary>
        /// Clears the cached profile data for the current user
        /// Should be called after profile updates to ensure fresh data
        /// </summary>
        public void ClearProfileCache()
        {
            try
            {
                string username = GetCurrentUsername();
                
                // Clear all profile-related cache keys
                string profileInfoCacheKey = $"MyBillProfileInformation_{username}";
                string accountsCacheKey = $"MyBillAllAccountsByPrimary_{username}";
                
                _cache.Remove(profileInfoCacheKey);
                _cache.Remove(accountsCacheKey);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error clearing profile cache");
            }
        }
    }
}
