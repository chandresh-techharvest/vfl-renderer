using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Services.Dashboard.Models;

namespace VFL.Renderer.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<ApiResponse<T>> AddDevice<T>(DeviceRequest request);
        Task<ApiResponse<T>> GetBalanceInformation<T>(DeviceGetRequestModel request);
        Task<ApiResponse<T>> RemoveDevice<T>(DeviceGetRequestModel request);
        Task<ApiResponse<T>> SendOTPCode<T>(DeviceGetRequestModel request);
    }
}
