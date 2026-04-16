using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Models.ResetPassword;
using VFL.Renderer.Services.ResetPasswordService.Models;

namespace VFL.Renderer.Services.ResetPasswordService
{
    public interface IResetPasswordService
    {
        Task<ApiResponse<T>> SubmitRequest<T>(ForgotPasswordRequest request);
        Task<ApiResponse<T>> ResetPassword<T>(ResetPasswordVerifyRequest request);
    }
}
