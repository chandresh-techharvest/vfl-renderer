using System;
using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillTransactionHistory;
using VFL.Renderer.ViewModels.MyBillTransactionHistory;

namespace VFL.Renderer.Models.MyBillTransactionHistory
{
    /// <summary>
    /// Model for MyBill Transaction History widget - prepares view model data
    /// </summary>
    public class MyBillTransactionHistoryModel : IMyBillTransactionHistoryModel
    {
        public MyBillTransactionHistoryModel()
        {
        }

        /// <summary>
        /// Gets the view model by mapping entity settings to view model
        /// Includes preview data for Sitefinity designer mode
        /// </summary>
        public Task<MyBillTransactionHistoryViewModel> GetViewModelAsync(MyBillTransactionHistoryEntity entity)
        {
            var viewModel = new MyBillTransactionHistoryViewModel
            {
                WidgetTitle = entity.WidgetTitle ?? "Payment Transaction History",
                PageSize = entity.PageSize > 0 ? entity.PageSize : 20,
                ShowSearch = entity.ShowSearch,
                ShowStatusFilter = entity.ShowStatusFilter,
                PreviewTransactions = GetPreviewTransactions()
            };

            return Task.FromResult(viewModel);
        }

        /// <summary>
        /// Returns preview transactions for Sitefinity designer mode
        /// </summary>
        private TransactionHistoryItem[] GetPreviewTransactions()
        {
            return
            [
                new TransactionHistoryItem
                {
                    TransactionId = 1,
                    AccountName = "GPH",
                    Ban = "922671653",
                    InvoiceNumber = "INV-111111",
                    Date = DateTime.Now.AddDays(-1),
                    Reference = "53b705e7",
                    Amount = 150.00m,
                    PaymentMethod = "Credit Card",
                    PartialPayment = "No",
                    Status = "Success"
                },
                new TransactionHistoryItem
                {
                    TransactionId = 2,
                    AccountName = "GPH",
                    Ban = "922671653",
                    InvoiceNumber = "INV-111112",
                    Date = DateTime.Now.AddDays(-5),
                    Reference = "039b32f4",
                    Amount = 75.50m,
                    PaymentMethod = "MPaisa",
                    PartialPayment = "Yes",
                    Status = "In Progress"
                },
                new TransactionHistoryItem
                {
                    TransactionId = 3,
                    AccountName = "GPH",
                    Ban = "922671653",
                    InvoiceNumber = "INV-111113",
                    Date = DateTime.Now.AddDays(-10),
                    Reference = "1ac5124f",
                    Amount = 200.00m,
                    PaymentMethod = "Credit Card",
                    PartialPayment = "No",
                    Status = "Failed"
                }
            ];
        }
    }
}
