using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Services.MyBillProfile;
using VFL.Renderer.Services.MyBillTransactionHistory;

namespace VFL.Renderer.Controllers
{
    /// <summary>
    /// API Controller for MyBill Transaction History operations
    /// Handles AJAX requests from the MyBill Transaction History widget
    /// Uses MyBillAuth authentication scheme
    /// Implements server-side cursor-based pagination via GraphQL
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "MyBillAuth")]
    public class MyBillTransactionHistoryController : ControllerBase
    {
        private readonly IMyBillTransactionHistoryService _transactionHistoryService;
        private readonly IMyBillProfileService _myBillProfileService;
        private readonly ILogger<MyBillTransactionHistoryController> _logger;

        public MyBillTransactionHistoryController(
            IMyBillTransactionHistoryService transactionHistoryService,
            IMyBillProfileService myBillProfileService,
            ILogger<MyBillTransactionHistoryController> logger)
        {
            _transactionHistoryService = transactionHistoryService;
            _myBillProfileService = myBillProfileService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the list of BAN accounts for the authenticated user
        /// Used by the transaction history filter dropdown
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBanAccounts()
        {
            try
            {
                if (!User?.Identity?.IsAuthenticated ?? true)
                {
                    return Unauthorized(new { message = "User is not authenticated", isSuccess = false, statusCode = 401 });
                }

                if (User?.Identity?.AuthenticationType != "MyBillAuth")
                {
                    return Unauthorized(new { message = "Invalid authentication scheme", isSuccess = false, statusCode = 401 });
                }

                var tokenClaim = User?.Claims?.FirstOrDefault(c => c.Type == "access_tokenMyBill");
                if (tokenClaim == null || string.IsNullOrEmpty(tokenClaim.Value))
                {
                    return Unauthorized(new { message = "Access token is missing. Please log in again.", isSuccess = false, statusCode = 401 });
                }

                var graphQLResponse = await _myBillProfileService.GetAllAccountsByPrimaryAsync<Services.MyBillProfile.Models.GraphQLAccountsResponse>();

                if (graphQLResponse?.Data?.AllAccountsByPrimary != null && graphQLResponse.Data.AllAccountsByPrimary.Length > 0)
                {
                    var primaryAccount = graphQLResponse.Data.AllAccountsByPrimary[0];
                    var banAccounts = primaryAccount.BusinessAccountNumbers?.Select(account => new
                    {
                        number = account.Number,
                        name = account.AccountName
                    }).ToArray() ?? [];

                    return Ok(new { data = banAccounts, isSuccess = true, statusCode = 200 });
                }

                return Ok(new { data = Array.Empty<object>(), isSuccess = true, statusCode = 200 });
            }
            catch (UnauthorizedAccessException uex)
            {
                _logger.LogWarning(uex, "MyBillTransactionHistory: Session expired in GetBanAccounts");
                return Unauthorized(new { isSuccess = false, statusCode = 401, message = "Your session has expired. Please log in again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBillTransactionHistory: Error in GetBanAccounts");
                return BadRequest(new { isSuccess = false, message = "Failed to retrieve BAN accounts", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets paginated transaction history
        /// Uses cursor-based pagination for efficient server-side paging
        /// </summary>
        /// <param name="request">Pagination and filter request</param>
        /// <returns>Paginated transaction history with cursor info</returns>
        [HttpPost]
        public async Task<IActionResult> GetTransactionHistory([FromBody] TransactionHistoryRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "MyBillTransactionHistory: GetTransactionHistory called. PageSize={PageSize}, After={After}, BAN={BAN}, Status={Status}, InvoiceNumber={InvoiceNumber}, DateFrom={DateFrom}, DateTo={DateTo}",
                    request?.PageSize ?? 20,
                    request?.After ?? "null",
                    request?.BanNumber ?? "null",
                    request?.Status ?? "null",
                    request?.InvoiceNumber ?? "null",
                    request?.DateFrom ?? "null",
                    request?.DateTo ?? "null");

                // Validate user is authenticated
                if (!User?.Identity?.IsAuthenticated ?? true)
                {
                    _logger.LogError("MyBillTransactionHistory: User is not authenticated");
                    return Unauthorized(new
                    {
                        message = "User is not authenticated",
                        isSuccess = false,
                        statusCode = 401
                    });
                }

                // Validate authentication scheme
                if (User?.Identity?.AuthenticationType != "MyBillAuth")
                {
                    _logger.LogError("MyBillTransactionHistory: Invalid authentication scheme (using {Scheme})",
                        User?.Identity?.AuthenticationType);
                    return Unauthorized(new
                    {
                        message = "Invalid authentication scheme",
                        isSuccess = false,
                        statusCode = 401
                    });
                }

                // Check for access token claim
                var tokenClaim = User?.Claims?.FirstOrDefault(c => c.Type == "access_tokenMyBill");
                if (tokenClaim == null || string.IsNullOrEmpty(tokenClaim.Value))
                {
                    _logger.LogError("MyBillTransactionHistory: access_tokenMyBill claim is missing or empty");
                    return Unauthorized(new
                    {
                        message = "Access token is missing. Please log in again.",
                        isSuccess = false,
                        statusCode = 401
                    });
                }

                // Parse date filters safely
                DateTime? dateFrom = null;
                DateTime? dateTo = null;
                if (!string.IsNullOrEmpty(request?.DateFrom) && DateTime.TryParse(request.DateFrom, out var parsedFrom))
                {
                    dateFrom = parsedFrom;
                }
                if (!string.IsNullOrEmpty(request?.DateTo) && DateTime.TryParse(request.DateTo, out var parsedTo))
                {
                    dateTo = parsedTo;
                }

                // Call service to get transaction history
                var pageSize = request?.PageSize > 0 ? request.PageSize : 20;
                var graphQLResponse = await _transactionHistoryService.GetTransactionHistoryAsync(
                    pageSize,
                    request?.After,
                    request?.BanNumber,
                    request?.Status,
                    request?.InvoiceNumber,
                    dateFrom,
                    dateTo);

                if (graphQLResponse?.Data?.TransactionHistoryQuery == null)
                {
                    _logger.LogWarning("MyBillTransactionHistory: No data returned from GraphQL");
                    return Ok(new
                    {
                        data = new
                        {
                            transactions = Array.Empty<object>(),
                            totalCount = 0,
                            pageInfo = new
                            {
                                endCursor = (string)null,
                                hasNextPage = false
                            }
                        },
                        isSuccess = true,
                        statusCode = 200
                    });
                }

                var result = graphQLResponse.Data.TransactionHistoryQuery;

                // Transform transactions for frontend
                var transactions = result.Edges?.Select(edge => new
                {
                    transactionId = edge.Node.TransactionId,
                    accountName = edge.Node.AccountName,
                    ban = edge.Node.Ban,
                    invoiceNumber = edge.Node.InvoiceNumber,
                    date = edge.Node.Date,
                    reference = edge.Node.Reference,
                    amount = edge.Node.Amount,
                    paymentMethod = edge.Node.PaymentMethod,
                    partialPayment = edge.Node.PartialPayment,
                    status = edge.Node.Status,
                    cursor = edge.Cursor
                }).ToArray() ?? [];

                _logger.LogInformation(
                    "MyBillTransactionHistory: Returning {Count} transactions (Total: {Total}, HasNextPage: {HasNext})",
                    transactions.Length,
                    result.TotalCount,
                    result.PageInfo?.HasNextPage ?? false);

                return Ok(new
                {
                    data = new
                    {
                        transactions = transactions,
                        totalCount = result.TotalCount,
                        pageInfo = new
                        {
                            endCursor = result.PageInfo?.EndCursor,
                            hasNextPage = result.PageInfo?.HasNextPage ?? false
                        }
                    },
                    isSuccess = true,
                    statusCode = 200
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "MyBillTransactionHistory: Session expired");
                return Unauthorized(new
                {
                    isSuccess = false,
                    statusCode = 401,
                    message = "Your session has expired. Please log in again."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBillTransactionHistory: Error in GetTransactionHistory");
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Failed to retrieve transaction history",
                    error = ex.Message
                });
            }
        }
    }

    /// <summary>
    /// Request model for transaction history API
    /// </summary>
    public class TransactionHistoryRequest
    {
        /// <summary>
        /// Number of records to fetch per page
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Cursor for pagination (null for first page, endCursor from previous response for next page)
        /// </summary>
        public string After { get; set; }

        /// <summary>
        /// Optional BAN number filter
        /// </summary>
        public string BanNumber { get; set; }

        /// <summary>
        /// Optional status filter (e.g., "Success", "Failed")
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Optional invoice number filter (contains search)
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Optional date range start filter (yyyy-MM-dd string)
        /// </summary>
        public string DateFrom { get; set; }

        /// <summary>
        /// Optional date range end filter (yyyy-MM-dd string)
        /// </summary>
        public string DateTo { get; set; }
    }
}
