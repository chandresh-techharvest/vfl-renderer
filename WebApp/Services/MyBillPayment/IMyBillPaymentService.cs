using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Services.MyBillPayment.Models;

namespace VFL.Renderer.Services.MyBillPayment
{
    /// <summary>
    /// Service interface for MyBill payment operations
    /// </summary>
    public interface IMyBillPaymentService
    {
        /// <summary>
        /// Process a payment request and get payment gateway redirect URL
        /// </summary>
        Task<ApiResponse<MyBillPaymentResponse>> ProcessPaymentAsync(MyBillPaymentRequest request);

        /// <summary>
        /// Verify payment callback from payment gateway
        /// </summary>
        Task<ApiResponse<MyBillProvideUpdateResponse>> ProvideUpdateAsync(MyBillProvideUpdateRequest request);
    }
}
