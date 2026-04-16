using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Services.TransactionHistroy.Models;

namespace VFL.Renderer.Services.TransactionHistroy
{
    public interface ITransactionHistoryService
    {
        Task<ApiResponse<T>> GetWebTopUpHistory<T>(GraphQLRequest filters);
        Task<ApiResponse<T>> GetPurchasePlanHistory<T>(GraphQLRequest filters);
        Task<ApiResponse<T>> GetDirectTopUpHistory<T>(GraphQLRequest filters);
        Task<ApiResponse<T>> GetPlanActivationHistory<T>(GraphQLRequest filters);
        Task<ApiResponse<T>> GetPrepayGiftHistory<T>(GraphQLRequest filters);
    }
}
