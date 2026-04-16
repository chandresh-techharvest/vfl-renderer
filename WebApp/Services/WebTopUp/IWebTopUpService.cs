using System.Threading.Tasks;
using VFL.Renderer.Common;

using VFL.Renderer.Services.WebTopUp.Models;

namespace VFL.Renderer.Services.WebTopUp
{
    public interface IWebTopUpService
    {

        Task<ApiResponse<T>> ProcessPayment<T>(WebTopUpRequest request);

        Task<ApiResponse<T>> ProcessPaymentPublic<T>(WebTopUpRequest request);
        Task<ApiResponse<T>> ProvideUpdate<T>(ProvideUpdateRequest request);
        Task<ApiResponse<T>> ProvideUpdatePublic<T>(ProvideUpdateRequest request);

       
    }
}
