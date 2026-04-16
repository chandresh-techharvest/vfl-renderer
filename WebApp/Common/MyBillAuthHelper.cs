using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VFL.Renderer.Common
{
    /// <summary>
    /// Helper class for consistent MyBill authentication checks across all components
    /// </summary>
    public static class MyBillAuthHelper
    {
        public const string MyBillAuthScheme = "MyBillAuth";
        public const string MyBillTokenClaim = "access_tokenMyBill";

        /// <summary>
        /// Checks if the user is authenticated with MyBill authentication scheme.
        /// First checks HttpContext.User (set by middleware), then falls back to
        /// explicitly authenticating against the MyBillAuth cookie scheme.
        /// This fallback is needed because Sitefinity's rendering pipeline can
        /// overwrite HttpContext.User with its own principal.
        /// </summary>
        /// <param name="httpContext">The current HTTP context</param>
        /// <returns>True if user is authenticated with MyBill</returns>
        public static async Task<bool> IsMyBillAuthenticatedAsync(HttpContext httpContext)
        {
            if (httpContext == null)
                return false;

            // Fast path: check HttpContext.User (set by our middleware)
            var user = httpContext.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var tokenClaim = user.FindFirst(MyBillTokenClaim);
                if (tokenClaim != null && !string.IsNullOrEmpty(tokenClaim.Value))
                    return true;
            }

            // Fallback: Sitefinity may have overwritten HttpContext.User.
            // Explicitly authenticate against the MyBillAuth cookie scheme.
            if (httpContext.Request.Cookies.ContainsKey("MyBillAuthCookie"))
            {
                try
                {
                    var authResult = await httpContext.AuthenticateAsync(MyBillAuthScheme);
                    if (authResult.Succeeded && authResult.Principal != null)
                    {
                        var tokenClaim = authResult.Principal.FindFirst(MyBillTokenClaim);
                        if (tokenClaim != null && !string.IsNullOrEmpty(tokenClaim.Value))
                        {
                            // Restore the principal so downstream code can use it
                            httpContext.User = authResult.Principal;
                            return true;
                        }
                    }
                }
                catch
                {
                    // Authentication failed — cookie is invalid or expired
                }
            }

            return false;
        }

        /// <summary>
        /// Synchronous check — only inspects HttpContext.User.
        /// Use IsMyBillAuthenticatedAsync when possible for reliable results.
        /// </summary>
        public static bool IsMyBillAuthenticated(HttpContext httpContext)
        {
            if (httpContext == null)
                return false;

            var user = httpContext.User;
            if (user == null)
                return false;

            if (user.Identity?.IsAuthenticated != true)
                return false;

            var tokenClaim = user.FindFirst(MyBillTokenClaim);
            if (tokenClaim == null || string.IsNullOrEmpty(tokenClaim.Value))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the user is strictly authenticated with MyBillAuth scheme
        /// Use this when you need to ensure the user is ONLY authenticated via MyBill
        /// </summary>
        /// <param name="httpContext">The current HTTP context</param>
        /// <returns>True if user is strictly authenticated with MyBillAuth scheme</returns>
        public static bool IsStrictlyMyBillAuthenticated(HttpContext httpContext)
        {
            if (httpContext == null)
                return false;

            var user = httpContext.User;
            if (user == null)
                return false;

            // Check 1: User must be authenticated
            if (user.Identity?.IsAuthenticated != true)
                return false;

            // Check 2: Authentication type must be MyBillAuth
            if (user.Identity.AuthenticationType != MyBillAuthScheme)
                return false;

            // Check 3: User must have a valid access token claim
            var tokenClaim = user.FindFirst(MyBillTokenClaim);
            if (tokenClaim == null || string.IsNullOrEmpty(tokenClaim.Value))
                return false;

            return true;
        }

        /// <summary>
        /// Gets the MyBill access token from the user's claims
        /// </summary>
        /// <param name="httpContext">The current HTTP context</param>
        /// <returns>The access token or empty string if not found</returns>
        public static string GetAccessToken(HttpContext httpContext)
        {
            if (httpContext == null)
                return string.Empty;

            var tokenClaim = httpContext.User?.FindFirst(MyBillTokenClaim);
            return tokenClaim?.Value ?? string.Empty;
        }

        /// <summary>
        /// Gets the username from the user's claims
        /// </summary>
        /// <param name="httpContext">The current HTTP context</param>
        /// <returns>The username or empty string if not found</returns>
        public static string GetUsername(HttpContext httpContext)
        {
            if (httpContext == null)
                return string.Empty;

            var nameClaim = httpContext.User?.FindFirst(ClaimTypes.Name);
            return nameClaim?.Value ?? string.Empty;
        }
    }
}
