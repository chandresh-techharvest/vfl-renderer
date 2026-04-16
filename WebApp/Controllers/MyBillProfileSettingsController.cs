using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;

namespace VFL.Renderer.Controllers
{
    /// <summary>
    /// API Controller for MyBill Profile Settings operations
    /// Handles profile updates and password changes
    /// Uses MyBillAuth authentication scheme
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "MyBillAuth")]
    public class MyBillProfileSettingsController : ControllerBase
    {
        private readonly MyBillWebClient _myBillWebClient;
        private readonly ILogger<MyBillProfileSettingsController> _logger;
        private readonly VFL.Renderer.Services.MyBillProfile.IMyBillProfileService _myBillProfileService;

        public MyBillProfileSettingsController(
            MyBillWebClient myBillWebClient,
            ILogger<MyBillProfileSettingsController> logger,
            VFL.Renderer.Services.MyBillProfile.IMyBillProfileService myBillProfileService)
        {
            _myBillWebClient = myBillWebClient;
            _logger = logger;
            _myBillProfileService = myBillProfileService;
        }

        /// <summary>
        /// Gets profile information for a specific BAN to populate the form
        /// GET api/MyBillProfileSettings/GetProfileInfo/{selectedBan}?refresh=true
        /// </summary>
        [HttpGet("/api/MyBillProfileSettings/GetProfileInfo/{selectedBan}")]
        public async Task<IActionResult> GetProfileInfo(string selectedBan, [FromQuery] bool refresh = false)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedBan))
                {
                    return BadRequest(new { message = "BAN number is required" });
                }

                _logger.LogDebug("MyBill: GetProfileInfo called for BAN {BanNumber}", selectedBan);

                // Clear cache if refresh is requested
                if (refresh)
                {
                    _myBillProfileService.ClearProfileCache();
                }

                // Get all accounts from GraphQL
                var graphQLResponse = await _myBillProfileService.GetAllAccountsByPrimaryAsync<Services.MyBillProfile.Models.GraphQLAccountsResponse>();

                if (graphQLResponse?.Data?.AllAccountsByPrimary != null && graphQLResponse.Data.AllAccountsByPrimary.Length > 0)
                {
                    var primaryAccount = graphQLResponse.Data.AllAccountsByPrimary[0];

                    // Find the selected BAN account
                    var selectedBanAccount = primaryAccount.BusinessAccountNumbers?
                        .FirstOrDefault(b => b.Number == selectedBan);

                    if (selectedBanAccount == null)
                    {
                        _logger.LogWarning("MyBill: BAN {BanNumber} not found in user's accounts", selectedBan);
                        return NotFound(new { message = "Account not found" });
                    }

                    // Return profile data for the selected BAN
                    var profileData = new
                    {
                        accountName = selectedBanAccount.AccountName,
                        contactFullName = primaryAccount.ContactFullName,
                        phoneNumber = primaryAccount.PhoneNumber,
                        email = primaryAccount.Email,
                        banNumber = selectedBan
                    };

                    return Ok(new
                    {
                        isSuccess = true,
                        statusCode = 200,
                        data = profileData
                    });
                }

                _logger.LogWarning("MyBill: No primary account found for user");
                return NotFound(new { message = "Profile information not found" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "MyBill: Session expired in GetProfileInfo for BAN {BanNumber}", selectedBan);
                return Unauthorized(new
                {
                    isSuccess = false,
                    statusCode = 401,
                    message = "Your session has expired. Please log in again."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in GetProfileInfo for BAN {BanNumber}", selectedBan);
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Failed to retrieve profile information",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates user profile information for a specific BAN
        /// PUT api/MyBillProfileSettings/UpdateProfile/{selectedBan}
        /// </summary>
        [HttpPut("/api/MyBillProfileSettings/UpdateProfile/{selectedBan}")]
        public async Task<IActionResult> UpdateProfile(string selectedBan, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedBan))
                {
                    return BadRequest(new { message = "BAN number is required" });
                }

                if (request == null)
                {
                    return BadRequest(new { message = "Request body is required" });
                }

                _logger.LogDebug("MyBill: UpdateProfile called for BAN {BanNumber}", selectedBan);

                // Call backend API to update profile using PUT
                var response = await _myBillWebClient.PutWithTokenAsync<dynamic>(
                    $"api/Dashboard/UpdateProfile/{selectedBan}",
                    new
                    {
                        accountName = request.AccountName,
                        contactFullName = request.ContactFullName,
                        contactPhoneNumber = request.ContactPhoneNumber,
                        email = request.Email,
                        password = (string)null // Password not included in profile update
                    },
                    "application/json"
                );

                // Check if token refresh failed (401 returned)
                if (response?.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("MyBill: UpdateProfile received 401 - session expired for BAN {BanNumber}", selectedBan);
                    return Unauthorized(new
                    {
                        isSuccess = false,
                        statusCode = 401,
                        message = "Your session has expired. Please log in again."
                    });
                }

                // Clear the profile cache to ensure fresh data on next load
                _myBillProfileService.ClearProfileCache();

                return Ok(new
                {
                    isSuccess = true,
                    statusCode = 200,
                    message = "Profile updated successfully",
                    data = response
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "MyBill: Session expired in UpdateProfile for BAN {BanNumber}", selectedBan);
                return Unauthorized(new
                {
                    isSuccess = false,
                    statusCode = 401,
                    message = "Your session has expired. Please log in again."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in UpdateProfile for BAN {BanNumber}", selectedBan);
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Failed to update profile",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates user password for a specific BAN
        /// PUT api/MyBillProfileSettings/UpdatePassword/{selectedBan}
        /// </summary>
        [HttpPut("/api/MyBillProfileSettings/UpdatePassword/{selectedBan}")]
        public async Task<IActionResult> UpdatePassword(string selectedBan, [FromBody] UpdatePasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedBan))
                {
                    return BadRequest(new { message = "BAN number is required" });
                }

                if (request == null || string.IsNullOrEmpty(request.NewPassword))
                {
                    return BadRequest(new { message = "New password is required" });
                }

                if (request.NewPassword != request.ConfirmPassword)
                {
                    return BadRequest(new { message = "Passwords do not match" });
                }

                _logger.LogDebug("MyBill: UpdatePassword called for BAN {BanNumber}", selectedBan);

                // Get current profile information from GraphQL to include in the update
                var graphQLResponse = await _myBillProfileService.GetAllAccountsByPrimaryAsync<Services.MyBillProfile.Models.GraphQLAccountsResponse>();

                if (graphQLResponse?.Data?.AllAccountsByPrimary == null || graphQLResponse.Data.AllAccountsByPrimary.Length == 0)
                {
                    _logger.LogError("MyBill: No primary account found for password update");
                    return StatusCode(500, new { message = "Failed to retrieve profile information" });
                }

                var primaryAccount = graphQLResponse.Data.AllAccountsByPrimary[0];

                // Find the selected BAN account
                var selectedBanAccount = primaryAccount.BusinessAccountNumbers?
                    .FirstOrDefault(b => b.Number == selectedBan);

                if (selectedBanAccount == null)
                {
                    _logger.LogWarning("MyBill: BAN {BanNumber} not found in user's accounts", selectedBan);
                    return NotFound(new { message = "Account not found" });
                }

                // Call backend API to update password with complete profile data
                var response = await _myBillWebClient.PutWithTokenAsync<dynamic>(
                    $"api/Dashboard/UpdateProfile/{selectedBan}",
                    new
                    {
                        accountName = selectedBanAccount.AccountName,
                        contactFullName = primaryAccount.ContactFullName,
                        contactPhoneNumber = primaryAccount.PhoneNumber,
                        email = primaryAccount.Email,
                        password = request.NewPassword // Include new password
                    },
                    "application/json"
                );

                // Check if token refresh failed (401 returned)
                if (response?.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("MyBill: UpdatePassword received 401 - session expired for BAN {BanNumber}", selectedBan);
                    return Unauthorized(new
                    {
                        isSuccess = false,
                        statusCode = 401,
                        message = "Your session has expired. Please log in again."
                    });
                }

                return Ok(new
                {
                    isSuccess = true,
                    statusCode = 200,
                    message = "Password updated successfully",
                    data = response
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "MyBill: Session expired in UpdatePassword for BAN {BanNumber}", selectedBan);
                return Unauthorized(new
                {
                    isSuccess = false,
                    statusCode = 401,
                    message = "Your session has expired. Please log in again."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBill: Error in UpdatePassword for BAN {BanNumber}", selectedBan);
                return BadRequest(new
                {
                    isSuccess = false,
                    message = "Failed to update password",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// DTO for profile update request
        /// </summary>
        public class UpdateProfileRequest
        {
            public string AccountName { get; set; }
            public string ContactFullName { get; set; }
            public string ContactPhoneNumber { get; set; }
            public string Email { get; set; }
        }

        /// <summary>
        /// DTO for password update request
        /// </summary>
        public class UpdatePasswordRequest
        {
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }
        }
    }
}
