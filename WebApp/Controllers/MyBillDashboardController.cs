using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Services.MyBillProfile;
using VFL.Renderer.Services.MyBillProfile.Models;

namespace VFL.Renderer.Controllers
{
    /// <summary>
    /// API Controller for MyBill Dashboard operations
    /// Handles AJAX requests from the MyBill Dashboard widget
    /// Uses MyBillAuth authentication scheme
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "MyBillAuth")] 
    public class MyBillDashboardController : ControllerBase
    {
        private readonly MyBillWebClient _myBillWebClient;
        private readonly IMyBillProfileService _myBillProfileService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MyBillDashboardController> _logger;

        public MyBillDashboardController(
            MyBillWebClient myBillWebClient, 
            IMyBillProfileService myBillProfileService,
            IHttpClientFactory httpClientFactory,
            ILogger<MyBillDashboardController> logger)
        {
            _myBillWebClient = myBillWebClient;
            _myBillProfileService = myBillProfileService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Gets profile information including BAN accounts
        /// Used on dashboard load and BAN selection changes
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProfileInformation(string selectedBanNumber = null)
        {
            try
            {
                var response = await _myBillProfileService.GetProfileInformationAsync<MyBillProfileSettingsResponse>();

                if (response?.data?.BanAccounts != null && response.data.BanAccounts.Length > 0)
                {
                    // Check for stored selection in cookie
                    string cookieValue = Request.Cookies["mybillData"];
                    string cookieSelectedBan = null;

                    if (!string.IsNullOrEmpty(cookieValue))
                    {
                        try
                        {
                            var cookieData = JsonConvert.DeserializeObject<MyBillProfileSettingsResponse>(cookieValue);
                            cookieSelectedBan = cookieData?.BanAccounts?
                                .FirstOrDefault(b => b.IsSelected)?.Number;
                        }
                        catch
                        {
                            // Ignore invalid cookie
                        }
                    }

                    string finalSelectedBan = !string.IsNullOrEmpty(selectedBanNumber) ? selectedBanNumber : cookieSelectedBan;

                    // Set selection flag
                    if (!string.IsNullOrEmpty(finalSelectedBan))
                    {
                        foreach (var ban in response.data.BanAccounts)
                        {
                            ban.IsSelected = ban.Number == finalSelectedBan;
                        }
                    }
                    else if (!response.data.BanAccounts.Any(b => b.IsSelected))
                    {
                        // Default to first BAN if none selected
                        response.data.BanAccounts[0].IsSelected = true;
                    }
                }

                // Store in cookie for persistence
                Response.Cookies.Delete("mybillData");
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                };
                HttpContext.Response.Cookies.Append("mybillData", JsonConvert.SerializeObject(response.data), cookieOptions);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in GetProfileInformation");
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets all accounts by primary user using GraphQL API
        /// Returns account numbers, names, and paperless emails
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAccountsByPrimary(string selectedBanNumber = null)
        {
            try
            {
                _logger.LogDebug("MyBill: GetAllAccountsByPrimary called with selectedBanNumber={SelectedBan}", selectedBanNumber);

                // Check if user has required authentication
                if (!User?.Identity?.IsAuthenticated ?? true)
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated",
                        isSuccess = false,
                        statusCode = 401
                    });
                }

                if (User?.Identity?.AuthenticationType != "MyBillAuth")
                {
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
                    return Unauthorized(new
                    {
                        message = "Access token is missing. Please log in again.",
                        isSuccess = false,
                        statusCode = 401
                    });
                }

                var graphQLResponse = await _myBillProfileService.GetAllAccountsByPrimaryAsync<Services.MyBillProfile.Models.GraphQLAccountsResponse>();

                _logger.LogDebug("MyBill: GetAllAccountsByPrimary GraphQL response - HasData={HasData}, AccountCount={Count}",
                    graphQLResponse?.Data?.AllAccountsByPrimary != null,
                    graphQLResponse?.Data?.AllAccountsByPrimary?.Length ?? 0);

                if (graphQLResponse?.Data?.AllAccountsByPrimary != null && graphQLResponse.Data.AllAccountsByPrimary.Length > 0)
                {
                    // Get the first primary account (there should typically be only one)
                    var primaryAccount = graphQLResponse.Data.AllAccountsByPrimary[0];
                    
                    // Convert business account numbers to BanAccount array
                    var banAccounts = primaryAccount.BusinessAccountNumbers?.Select(account => new ViewModels.MyBillDashboard.BanAccount
                    {
                        Number = account.Number,
                        Name = account.AccountName,
                        IsSelected = false,
                        PaperlessEmails = account.PaperlessEmails?.Select(pe => new ViewModels.MyBillDashboard.PaperlessEmailInfo
                        {
                            Email = pe.Email,
                            Id = pe.Id
                        }).ToArray()
                    }).ToArray() ?? Array.Empty<ViewModels.MyBillDashboard.BanAccount>();

                    // HANDLE CASE: User is authenticated but has NO BAN accounts
                    if (banAccounts.Length == 0)
                    {
                        _logger.LogWarning("MyBill: User {Name} has no business account numbers (BANs) linked to their profile", 
                            primaryAccount.PrimaryAccountName);
                        
                        var emptyResponseData = new MyBillProfileSettingsResponse
                        {
                            BanAccounts = Array.Empty<ViewModels.MyBillDashboard.BanAccount>(),
                            CompanyName = primaryAccount.PrimaryAccountName,
                            PrimaryAccountName = primaryAccount.PrimaryAccountName,
                            ContactFullName = primaryAccount.ContactFullName,
                            PhoneNumber = primaryAccount.PhoneNumber,
                            Email = primaryAccount.Email
                        };
                        
                        return Ok(new
                        {
                            data = emptyResponseData,
                            isSuccess = true,
                            statusCode = 200,
                            message = "No billing accounts found for this user. Please contact support to link your billing accounts.",
                            hasNoBanAccounts = true
                        });
                    }

                    // Check for stored selection in cookie
                    string cookieValue = Request.Cookies["mybillData"];
                    string cookieSelectedBan = null;

                    if (!string.IsNullOrEmpty(cookieValue))
                    {
                        try
                        {
                            var cookieData = JsonConvert.DeserializeObject<MyBillProfileSettingsResponse>(cookieValue);
                            cookieSelectedBan = cookieData?.BanAccounts?
                                .FirstOrDefault(b => b.IsSelected)?.Number;
                        }
                        catch
                        {
                            // Ignore invalid cookie
                        }
                    }

                    string finalSelectedBan = null;
                    
                    if (!string.IsNullOrEmpty(selectedBanNumber))
                    {
                        finalSelectedBan = selectedBanNumber;
                    }
                    else if (!string.IsNullOrEmpty(cookieSelectedBan) && banAccounts.Any(b => b.Number == cookieSelectedBan))
                    {
                        finalSelectedBan = cookieSelectedBan;
                    }
                    else
                    {
                        finalSelectedBan = banAccounts[0].Number;
                    }

                    // Set selection flag
                    foreach (var ban in banAccounts)
                    {
                        ban.IsSelected = ban.Number == finalSelectedBan;
                    }

                    // Create response object with primary account info
                    var responseData = new MyBillProfileSettingsResponse
                    {
                        BanAccounts = banAccounts,
                        CompanyName = primaryAccount.PrimaryAccountName,
                        PrimaryAccountName = primaryAccount.PrimaryAccountName,
                        ContactFullName = primaryAccount.ContactFullName,
                        PhoneNumber = primaryAccount.PhoneNumber,
                        Email = primaryAccount.Email
                    };

                    // Store in cookie for persistence
                    Response.Cookies.Delete("mybillData");
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7),
                        HttpOnly = false,
                        Secure = false,
                        SameSite = SameSiteMode.Lax
                    };
                    HttpContext.Response.Cookies.Append("mybillData", JsonConvert.SerializeObject(responseData), cookieOptions);

                    return Ok(new
                    {
                        data = responseData,
                        isSuccess = true,
                        statusCode = 200
                    });
                }

                _logger.LogWarning("MyBill: GetAllAccountsByPrimary returned no accounts");
                return Ok(new
                {
                    data = new MyBillProfileSettingsResponse { BanAccounts = Array.Empty<ViewModels.MyBillDashboard.BanAccount>() },
                    isSuccess = true,
                    statusCode = 200
                });
            }
            catch (UnauthorizedAccessException uex)
            {
                _logger.LogWarning(uex, "MyBill: Unauthorized in GetAllAccountsByPrimary - JWT likely expired");
                return Unauthorized(new
                {
                    message = "Session expired. Please try again.",
                    isSuccess = false,
                    statusCode = 401
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in GetAllAccountsByPrimary");
                return BadRequest(new
                {
                    message = "Failed to retrieve accounts",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets account summary for the selected BAN
        /// Includes current charges, invoice details, billing information
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetAccountSummary([FromBody] string banNumber)
        {
            try
            {
                _logger.LogDebug("MyBill: GetAccountSummary called for BAN {BanNumber}", banNumber);
                
                // Call GraphQL API to get invoices for this BAN
                var graphQLResponse = await _myBillProfileService.GetInvoicesByBanAsync<Services.MyBillProfile.Models.GraphQLInvoicesResponse>(banNumber, true);

                if (graphQLResponse?.Data?.Invoices != null && graphQLResponse.Data.Invoices.Length > 0)
                {
                    // Sort invoices by date (newest first)
                    var sortedInvoices = graphQLResponse.Data.Invoices
                        .OrderByDescending(inv => inv.BillingCycleStartDate ?? inv.BillingCycleEndDate ?? inv.DueDate ?? DateTime.MinValue)
                        .ToArray();
                    
                    // Get the most recent invoice
                    var latestInvoice = sortedInvoices[0];
                    
                    _logger.LogDebug("MyBill: Found {Count} invoices for BAN {BanNumber}", sortedInvoices.Length, banNumber);

                    var responseData = new
                    {
                        invoice = new
                        {
                            number = latestInvoice.Number,
                            amount = latestInvoice.Amount / 10000m,
                            totalAmount = latestInvoice.Amount / 10000m,
                            issueDate = latestInvoice.BillingCycleEndDate,
                            dueDate = latestInvoice.DueDate,
                            fileName = latestInvoice.FileName,
                            paymentStatus = latestInvoice.PaymentStatus ? "Paid" : "Pending",
                            preBalance = latestInvoice.PreBalance / 10000m,
                            billingCycleStartDate = latestInvoice.BillingCycleStartDate,
                            billingCycleEndDate = latestInvoice.BillingCycleEndDate
                        },
                        allInvoices = sortedInvoices.Select(inv => new
                        {
                            number = inv.Number,
                            amount = inv.Amount / 10000m,
                            billingCycleStartDate = inv.BillingCycleStartDate,
                            billingCycleEndDate = inv.BillingCycleEndDate,
                            dueDate = inv.DueDate,
                            fileName = inv.FileName,
                            paymentStatus = inv.PaymentStatus,
                            preBalance = inv.PreBalance / 10000m
                        }).ToArray()
                    };

                    return Ok(new
                    {
                        data = responseData,
                        statusCode = 200
                    });
                }

                _logger.LogDebug("MyBill: No invoices found for BAN {BanNumber}", banNumber);
                return Ok(new
                {
                    data = new
                    {
                        invoice = (object)null,
                        allInvoices = Array.Empty<object>()
                    },
                    statusCode = 200
                });
            }
            catch (UnauthorizedAccessException uex)
            {
                _logger.LogWarning(uex, "MyBill: Unauthorized in GetAccountSummary for BAN {BanNumber}", banNumber);
                return Unauthorized(new
                {
                    message = "Session expired. Please try again.",
                    statusCode = 401
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in GetAccountSummary for BAN {BanNumber}", banNumber);
                return BadRequest(new
                {
                    message = "Failed to retrieve account summary",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets transaction history for the selected BAN
        /// Returns list of invoices and payments
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetTransactionHistory([FromBody] dynamic request)
        {
            try
            {
                string banNumber = request?.banNumber?.ToString();

                if (string.IsNullOrEmpty(banNumber))
                {
                    return BadRequest(new { message = "BAN number is required" });
                }

                _logger.LogDebug("MyBill: GetTransactionHistory called for BAN {BanNumber}", banNumber);

                var graphQLResponse = await _myBillProfileService.GetInvoicesByBanAsync<Services.MyBillProfile.Models.GraphQLInvoicesResponse>(banNumber, true);

                if (graphQLResponse?.Data?.Invoices != null && graphQLResponse.Data.Invoices.Length > 0)
                {
                    var sortedInvoices = graphQLResponse.Data.Invoices
                        .OrderByDescending(inv => inv.BillingCycleStartDate ?? inv.BillingCycleEndDate ?? inv.DueDate ?? DateTime.MinValue)
                        .ToArray();
                    
                    var transactions = sortedInvoices
                        .Select(invoice => new
                        {
                            date = invoice.BillingCycleEndDate ?? invoice.DueDate,
                            invoiceNumber = invoice.Number,
                            description = $"Monthly Service Charges ({(invoice.BillingCycleEndDate.HasValue ? invoice.BillingCycleEndDate.Value.ToString("MMM yyyy") : "N/A")})",
                            amount = invoice.Amount / 10000m,
                            status = invoice.PaymentStatus ? "Paid" : "Pending",
                            fileName = invoice.FileName,
                            dueDate = invoice.DueDate
                        })
                        .ToArray();

                    _logger.LogDebug("MyBill: Found {Count} transactions for BAN {BanNumber}", transactions.Length, banNumber);

                    return Ok(new
                    {
                        data = new
                        {
                            transactions = transactions
                        },
                        statusCode = 200
                    });
                }

                _logger.LogDebug("MyBill: No transactions found for BAN {BanNumber}", banNumber);
                return Ok(new
                {
                    data = new
                    {
                        transactions = Array.Empty<object>()
                    },
                    statusCode = 200
                });
            }
            catch (UnauthorizedAccessException uex)
            {
                _logger.LogWarning(uex, "MyBill: Unauthorized in GetTransactionHistory");
                return Unauthorized(new
                {
                    message = "Session expired. Please try again.",
                    statusCode = 401
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in GetTransactionHistory");
                return BadRequest(new
                {
                    message = "Failed to retrieve transaction history",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates billing email addresses (go paperless feature)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateBillingEmails([FromBody] dynamic request)
        {
            try
            {
                var response = await _myBillWebClient.PostWithTokenAsync<dynamic>(
                    "/api/MyBillBackend/UpdateBillingEmails",
                    request,
                    "application/json"
                );
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in UpdateBillingEmails");
                throw;
            }
        }

        /// <summary>
        /// Updates paperless email addresses for a specific BAN
        /// </summary>
        [HttpPut]
        [Route("/api/Dashboard/UpdatePaperless/{selectedBAN}")]
        public async Task<IActionResult> UpdatePaperless(string selectedBAN, [FromBody] PaperlessEmailUpdate[] emails)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedBAN))
                {
                    return BadRequest(new { message = "BAN number is required" });
                }

                if (emails == null || emails.Length == 0)
                {
                    return BadRequest(new { message = "At least one email is required" });
                }

                _logger.LogDebug("MyBill: UpdatePaperless called for BAN {BanNumber} with {Count} emails", selectedBAN, emails.Length);

                var response = await _myBillWebClient.PutWithTokenAsync<dynamic>(
                    $"api/Dashboard/UpdatePaperless/{selectedBAN}",
                    emails,
                    "application/json"
                );

                if (response?.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("MyBill: UpdatePaperless received 401 - session expired for BAN {BanNumber}", selectedBAN);
                    return Unauthorized(new
                    {
                        isSuccess = false,
                        statusCode = 401,
                        message = "Your session has expired. Please log in again."
                    });
                }

                _myBillProfileService.ClearProfileCache();
                _logger.LogDebug("MyBill: UpdatePaperless completed successfully for BAN {BanNumber}", selectedBAN);

                return Ok(new
                {
                    isSuccess = true,
                    statusCode = 200,
                    message = "Paperless emails updated successfully",
                    data = response
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "MyBill: Session expired in UpdatePaperless for BAN {BanNumber}", selectedBAN);
                return Unauthorized(new
                {
                    isSuccess = false,
                    statusCode = 401,
                    message = "Your session has expired. Please log in again."
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in UpdatePaperless for BAN {BanNumber}", selectedBAN);
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Failed to update paperless emails",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// DTO for paperless email update request
        /// </summary>
        public class PaperlessEmailUpdate
        {
            public string Email { get; set; }
            public int Id { get; set; }
        }

        /// <summary>
        /// Downloads invoice PDF file from MyBill backend
        /// Acts as a proxy to the protected backend API
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadInvoice(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    _logger.LogWarning("MyBill: DownloadInvoice called with empty fileName");
                    return BadRequest(new { message = "File name is required" });
                }

                _logger.LogDebug("MyBill: DownloadInvoice called for file: {FileName}", fileName);

                // Get token and create authenticated request
                var httpContext = HttpContext;
                var user = httpContext.User;
                var token = user?.FindFirst("access_tokenMyBill")?.Value;

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("MyBill: No access token found for download request");
                    return Unauthorized(new { message = "Not authenticated" });
                }

                // Construct the backend URL properly
                var baseUrl = _myBillWebClient.GetBaseUrl();
                var requestUrl = $"{baseUrl}/api/Download/Invoice?fileName={Uri.EscapeDataString(fileName)}";
                
                _logger.LogDebug("MyBill: Requesting file from backend");

                // Use IHttpClientFactory instead of creating new HttpClient
                var httpClient = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("MyBill: Backend download failed with status {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, new { message = "Failed to download invoice" });
                }

                // Get file content
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/pdf";

                _logger.LogDebug("MyBill: File downloaded successfully, size: {Size} bytes", fileBytes.Length);

                // Return file to browser
                return File(fileBytes, contentType, fileName);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in DownloadInvoice for file {FileName}", fileName);
                return StatusCode(500, new { message = "An error occurred while downloading the invoice" });
            }
        }
    }
}
