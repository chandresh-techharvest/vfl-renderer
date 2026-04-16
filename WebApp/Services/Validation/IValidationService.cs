using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Services.Validation.Models;

namespace VFL.Renderer.Services.Validation
{
    public interface IValidationService 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="EmailVerifyRequest"></param>
        /// <returns>Task<ValidationResponse></returns>
        Task<ApiResponse<T>> CheckEmailIsRegisteredAsync<T>(EmailVerifyRequest request);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NumberVerifyRequest"></param>
        /// <returns>Task<ValidationResponse></returns>
        Task<ApiResponse<T>> CheckNumberIsRegisteredAsync<T>(NumberVerifyRequest request);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NumberVerifyRequest"></param>
        /// <returns>Task<ValidationResponse></returns>
        Task<ApiResponse<T>> CheckNumberIsValidAsync<T>(NumberVerifyRequest request);


        Task<ApiResponse<T>> CheckNumberIsValid_AllowInactiveNumber<T>(NumberVerifyRequest request);



    }
}
