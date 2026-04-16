using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Models.MyBillResetPassword;
using VFL.Renderer.Services.MyBillResetPasswordService.Models;

namespace VFL.Renderer.Services.MyBillResetPasswordService
{
    public interface IMyBillResetPasswordService
    {
        Task<ApiResponse<T>> SubmitRequest<T>(MyBillForgotPasswordRequest request);
        Task<ApiResponse<T>> ResetPassword<T>(MyBillResetPasswordVerifyRequest request);
    }
}
