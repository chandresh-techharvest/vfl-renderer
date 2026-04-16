using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Services.WebTopUp.Models;

namespace VFL.Renderer.Services.Plans
{
    public interface IPlansService
    {
        Task<ApiResponse<T>> GetallTopUpAmountsAsync<T>();
        Task<ApiResponse<T>> GetallPublicTopUpAmountsAsync<T>();

        Task<ApiResponse<T>> GetAllGiftingPlansAsync<T>(int number);
        Task<ApiResponse<T>> GetAllPublicGiftingPlansAsync<T>(int number);

        Task<ApiResponse<T>> GetPurchasePlanTypesAsync<T>();
        Task<ApiResponse<T>> GetPurchasePlanPaymentMethodsAsync<T>();
       

       Task<ApiResponse<T>> GetPurchasePlanByIdAsync<T>(int number  ,int planId);
        Task<ApiResponse<T>> GetAllPurchasePlanByPlanType<T>(int number,string planType);

        Task<ApiResponse<T>> GetAllPurchasePlanByPaymentMethod<T>(int number, string PaymentMethod);

        Task<ApiResponse<T>> GetAllPurchasePlanByPaymentMethodandPlanType<T>(int number,string planType,string PaymentMethod);
        Task<ApiResponse<T>> ProcessPayment<T>(WebTopUpRequest request);

        Task<ApiResponse<T>> ProcessPaymentPublic<T>(WebTopUpRequest request);
        Task<ApiResponse<T>> ProvideUpdate<T>(ProvideUpdateRequest request);

       
       Task<ApiResponse<T>> ProvideUpdatePublic<T>(ProvideUpdateRequest request);




    }
}
