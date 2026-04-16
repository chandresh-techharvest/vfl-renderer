using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.TransactionHistroy.Models;

namespace VFL.Renderer.Services.TransactionHistroy
{
    /// <summary>
    /// 
    /// </summary>
    public class TransactionHistoryService : ITransactionHistoryService
    {
        private readonly WebClient _client;
        private readonly ILogger<WebClient> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public TransactionHistoryService(WebClient client, ILogger<WebClient> logger)
        {
            _client = client;
            _logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<ApiResponse<T>> GetWebTopUpHistory<T>(GraphQLRequest filters)
        {
            try
            {
                var response = await _client.PostWithTokenGraphQLAsync<T>(filters.query, filters.Variables);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST GetWebTopUpHistory Service Layer");
                throw;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<ApiResponse<T>> GetPurchasePlanHistory<T>(GraphQLRequest filters)
        {           
            try
            {
                var response = await _client.PostWithTokenGraphQLAsync<T>(filters.query, filters.Variables);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST GetPurchasePlanHistory Service Layer");
                throw;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<ApiResponse<T>> GetDirectTopUpHistory<T>(GraphQLRequest filters)
        {
            try
            {
                var response = await _client.PostWithTokenGraphQLAsync<T>(filters.query, filters.Variables);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST GetDirectTopUpHistory Service Layer");
                throw;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<ApiResponse<T>> GetPlanActivationHistory<T>(GraphQLRequest filters)
        {    
            try
            {
                var response = await _client.PostWithTokenGraphQLAsync<T>(filters.query, filters.Variables);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST GetPlanActivationHistory Service Layer");
                throw;
            }

        }

        public async Task<ApiResponse<T>> GetPrepayGiftHistory<T>(GraphQLRequest filters)
        {
            try
            {
                var response = await _client.PostWithTokenGraphQLAsync<T>(filters.query, filters.Variables);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST GetPlanActivationHistory Service Layer");
                throw;
            }

        }
    }
}
