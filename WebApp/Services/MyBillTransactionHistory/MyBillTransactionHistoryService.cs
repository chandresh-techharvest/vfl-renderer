using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Services.MyBillTransactionHistory.Models;

namespace VFL.Renderer.Services.MyBillTransactionHistory
{
    /// <summary>
    /// Service for MyBill Transaction History operations
    /// Uses MyBillWebClient for authenticated GraphQL API calls
    /// Implements server-side cursor-based pagination
    /// </summary>
    public class MyBillTransactionHistoryService : IMyBillTransactionHistoryService
    {
        private readonly MyBillWebClient _myBillWebClient;
        private readonly ILogger<MyBillTransactionHistoryService> _logger;

        public MyBillTransactionHistoryService(
            MyBillWebClient myBillWebClient,
            ILogger<MyBillTransactionHistoryService> logger)
        {
            _myBillWebClient = myBillWebClient;
            _logger = logger;
        }

        /// <summary>
        /// Gets paginated transaction history from GraphQL API
        /// Uses cursor-based pagination for efficient server-side paging
        /// </summary>
        public async Task<GraphQLTransactionHistoryResponse> GetTransactionHistoryAsync(
            int first = 20,
            string after = null,
            string banNumber = null,
            string status = null,
            string invoiceNumber = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            try
            {
                _logger.LogInformation(
                    "MyBillTransactionHistory: Fetching transactions. First={First}, After={After}, BAN={BAN}, Status={Status}, Invoice={Invoice}, DateFrom={DateFrom}, DateTo={DateTo}",
                    first, after ?? "null", banNumber ?? "null", status ?? "null",
                    invoiceNumber ?? "null", dateFrom?.ToString("o") ?? "null", dateTo?.ToString("o") ?? "null");

                // Build the where filter if needed
                var whereClause = BuildWhereClause(banNumber, status, invoiceNumber, dateFrom, dateTo);

                // Build GraphQL query with variables
                var query = @"
                    query($first: Int!, $after: String, $where: TransactionHistoryDtoFilterInput) {
                        transactionHistoryQuery(first: $first, after: $after, where: $where, order: [{ date: DESC }]) {
                            totalCount
                            edges {
                                cursor
                                node {
                                    transactionId
                                    accountName
                                    ban
                                    invoiceNumber
                                    date
                                    reference
                                    amount
                                    paymentMethod
                                    partialPayment
                                    status
                                }
                            }
                            pageInfo {
                                endCursor
                                hasNextPage
                            }
                        }
                    }";

                var variables = new
                {
                    first = first,
                    after = after,
                    where = whereClause
                };

                _logger.LogInformation("MyBillTransactionHistory: Executing GraphQL query with variables");

                var response = await _myBillWebClient.PostGraphQLAsync<GraphQLTransactionHistoryResponse>(query, variables);

                if (response?.Data?.TransactionHistoryQuery != null)
                {
                    var result = response.Data.TransactionHistoryQuery;
                    _logger.LogInformation(
                        "MyBillTransactionHistory: Retrieved {Count} transactions (Total: {Total}, HasNextPage: {HasNext})",
                        result.Edges?.Length ?? 0,
                        result.TotalCount,
                        result.PageInfo?.HasNextPage ?? false);
                }
                else
                {
                    _logger.LogWarning("MyBillTransactionHistory: Response or data is null");
                }

             
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBillTransactionHistory: Error fetching transaction history");
                throw;
            }
        }

        /// <summary>
        /// Builds the where clause object for GraphQL query filtering
        /// Date filters use the "and" array format required by the GraphQL API
        /// </summary>
        private object BuildWhereClause(string banNumber, string status, string invoiceNumber, DateTime? dateFrom, DateTime? dateTo)
        {
            var filter = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(banNumber))
            {
                filter["ban"] = new { eq = banNumber };
            }

            if (!string.IsNullOrEmpty(status))
            {
                filter["status"] = new { eq = status };
            }

            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                filter["invoiceNumber"] = new { contains = invoiceNumber };
            }

            if (dateFrom.HasValue || dateTo.HasValue)
            {
                var andConditions = new List<object>();

                if (dateFrom.HasValue)
                {
                    andConditions.Add(new Dictionary<string, object>
                    {
                        ["date"] = new { gte = dateFrom.Value.Date.ToString("yyyy-MM-ddT00:00:00.000Z") }
                    });
                }
                if (dateTo.HasValue)
                {
                    andConditions.Add(new Dictionary<string, object>
                    {
                        ["date"] = new { lte = dateTo.Value.Date.ToString("yyyy-MM-ddT00:00:00.000Z") }
                    });
                }

                filter["and"] = andConditions;
            }

            return filter.Count > 0 ? filter : null;
        }
    }
}

