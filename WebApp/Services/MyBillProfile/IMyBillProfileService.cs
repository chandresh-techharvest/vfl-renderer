using System.Threading.Tasks;
using VFL.Renderer.Common;

namespace VFL.Renderer.Services.MyBillProfile
{
    /// <summary>
    /// Interface for MyBill Profile Service
    /// </summary>
    public interface IMyBillProfileService
    {
        /// <summary>
        /// Gets profile information for the authenticated MyBill user
        /// </summary>
        Task<ApiResponse<T>> GetProfileInformationAsync<T>();

        /// <summary>
        /// Gets all accounts by primary user from GraphQL API
        /// Includes account numbers, names, and paperless emails
        /// </summary>
        Task<T> GetAllAccountsByPrimaryAsync<T>();

        /// <summary>
        /// Gets invoices for a specific BAN from GraphQL API
        /// Includes invoice details, amounts, dates, payment status
        /// </summary>
        Task<T> GetInvoicesByBanAsync<T>(string banNumber, bool getFiles = true);

        /// <summary>
        /// Clears the cached profile data for the current user
        /// Should be called after profile updates to ensure fresh data
        /// </summary>
        void ClearProfileCache();
    }
}
