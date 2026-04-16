using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Models.MyBillLoginForm;

namespace VFL.Renderer.Services.MyBillLogin
{
    public interface IAuthServiceMyBill
    {
        Task<ApiResponse<T>> MyBillLoginAsync<T>(MyBillLoginRequest request);
        Task<ApiResponse<T>> RequestNewAccessTokenAsync<T>();
        Task<ApiResponse<T>> RefreshTokenAsync<T>();
        Task<ApiResponse<T>> ExternalLoginAsync<T>(string provider, string token);
        Task<ApiResponse<T>> RefreshTokenAsync<T>(string refreshToken);
        
        /// <summary>
        /// Login using a single-use access token (for customer support)
        /// </summary>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="token">The single-use access token</param>
        /// <returns>Login response with JWT token</returns>
        Task<ApiResponse<T>> LoginUsingSingleAccessTokenAsync<T>(string token);
    }
}
