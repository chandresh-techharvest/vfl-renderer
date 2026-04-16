using System;
using System.Threading.Tasks;
using VFL.Renderer.Services.MyBillTransactionHistory.Models;

namespace VFL.Renderer.Services.MyBillTransactionHistory
{
    /// <summary>
    /// Interface for MyBill Transaction History Service
    /// Provides paginated transaction history using GraphQL API
    /// </summary>
    public interface IMyBillTransactionHistoryService
    {
        /// <summary>
        /// Gets paginated transaction history from GraphQL API
        /// Uses cursor-based pagination for efficient server-side paging
        /// </summary>
        /// <param name="first">Number of records to fetch per page</param>
        /// <param name="after">Cursor for pagination (null for first page)</param>
        /// <param name="banNumber">Optional BAN number filter</param>
        /// <param name="status">Optional status filter</param>
        /// <param name="invoiceNumber">Optional invoice number filter (contains search)</param>
        /// <param name="dateFrom">Optional date range start filter</param>
        /// <param name="dateTo">Optional date range end filter</param>
        /// <returns>GraphQL response with transaction data and pagination info</returns>
        Task<GraphQLTransactionHistoryResponse> GetTransactionHistoryAsync(
            int first = 20, 
            string after = null, 
            string banNumber = null, 
            string status = null,
            string invoiceNumber = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null);
    }
}
