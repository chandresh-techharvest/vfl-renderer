using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Services.DirectTopUp.Models;

namespace VFL.Renderer.Services.DirectTopUp
{
    public interface IDirectTopUpService
    {
        Task<ApiResponse<T>> SendRequest<T>(DirectTopUpRequest request);
    }
}
