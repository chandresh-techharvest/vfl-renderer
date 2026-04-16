using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Services.Registration.Models;

namespace VFL.Renderer.Services.Registration
{
    public interface IRegistrationService
    {
        Task<ApiResponse<T>> RegisterAsync<T>(RegistrationRequest request);
        Task<ApiResponse<T>> ConfirmEmailAsync<T>(EmailVerify request);
        Task<ApiResponse<T>> ResendConfirmationEmailAsync<T>(ResendEmailVerify request);
    }
}
