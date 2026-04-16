using AngleSharp.Io;
using System.Threading.Tasks;
using VFL.Renderer.Common;

using VFL.Renderer.Services.PrepayGifting.Models;

namespace VFL.Renderer.Services.PrepayGifting
{
    public interface IPrepayGiftingService
    {
        Task<ApiResponse<T>> SendOtpRequest<T>(PrepayGiftingRequest request);
        Task<ApiResponse<T>> GetAllGiftingPlans<T>(int number);
        Task<ApiResponse<T>> GetPlanByPlanId<T>(int number,int planId);

        Task<ApiResponse<T>> PrepayGiftSubscribe<T>(SubscribeRequest request);

        Task<ApiResponse<T>> GetRealMoneyBalance<T>(string number);
    }
}
