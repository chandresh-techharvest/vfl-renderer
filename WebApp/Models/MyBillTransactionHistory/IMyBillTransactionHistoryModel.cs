using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillTransactionHistory;
using VFL.Renderer.ViewModels.MyBillTransactionHistory;

namespace VFL.Renderer.Models.MyBillTransactionHistory
{
    /// <summary>
    /// Interface for MyBill Transaction History model
    /// </summary>
    public interface IMyBillTransactionHistoryModel
    {
        /// <summary>
        /// Gets the view model for the MyBill Transaction History widget
        /// </summary>
        /// <param name="entity">The entity from Sitefinity</param>
        /// <returns>Populated MyBillTransactionHistoryViewModel</returns>
        Task<MyBillTransactionHistoryViewModel> GetViewModelAsync(MyBillTransactionHistoryEntity entity);
    }
}
