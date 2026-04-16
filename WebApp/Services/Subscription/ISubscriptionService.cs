using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Services.PrepayGifting.Models;
using VFL.Renderer.Services.Subscription.Models;

namespace VFL.Renderer.Services.Subscription
{
    public interface ISubscriptionService
    {


        Task<ApiResponse<T>> GetAllSubscriptionPlans<T>(int number);
        Task<ApiResponse<T>> GetAllSubscriptionPlansByPlanType<T>(int number, string planType);
        Task<ApiResponse<T>> GetAllSubscribedPlansByNumber<T>(int number);
        Task<ApiResponse<T>> GetPlanByPlanId<T>(int number, int planId);
        Task<ApiResponse<T>> SendOtpRequest<T>(SendOtpRequest request);
        Task<ApiResponse<T>> Subscribe<T>(SubscriptionRequest request);


        Task<ApiResponse<T>> Resubscribe<T>(SubscriptionRequest request);

        Task<ApiResponse<T>> Unsubscribe<T>(SubscriptionRequest request);


        Task<ApiResponse<T>> GetPurchasePlanTypesAsync<T>();
    }
}
