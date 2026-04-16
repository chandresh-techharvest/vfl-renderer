using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Models.LoginForm;

namespace VFL.Renderer.Services.LoginService
{
    public interface IAuthService
    {
        Task<ApiResponse<T>> LoginAsync<T>(LoginRequest request);
        Task<ApiResponse<T>> RefreshTokenAsync<T>();        
        Task<ApiResponse<T>> ExternalLoginAsync<T>(string provider, string token);        
        Task<ApiResponse<T>> SingleAccessLoginAsync<T>(string Token);
    }
}
