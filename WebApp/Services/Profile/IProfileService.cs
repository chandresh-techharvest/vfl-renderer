using System.Threading.Tasks;
using VFL.Renderer.Common;

namespace VFL.Renderer.Services.Profile
{
    public interface IProfileService
    {
        Task<ApiResponse<T>> GetProfileInformationAsync<T>();
    }
}
