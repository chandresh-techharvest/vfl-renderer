using System;

namespace VFL.Renderer.ViewModels.MyBillTransactionHistory
{
    /// <summary>
    /// View model for MyBill Transaction History widget
    /// Contains data to be rendered in the Razor view
    /// </summary>
    public class MyBillTransactionHistoryViewModel
    {
        /// <summary>
        /// Widget title displayed in the card header
        /// </summary>
        public string WidgetTitle { get; set; } = "Payment Transaction History";

        /// <summary>
        /// Number of records per page for pagination
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Show/hide the search filter
        /// </summary>
        public bool ShowSearch { get; set; } = true;

        /// <summary>
        /// Show/hide the status filter dropdown
        /// </summary>
        public bool ShowStatusFilter { get; set; } = true;

        /// <summary>
        /// Indicates if the user is authenticated with MyBill
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Indicates if the widget is being viewed in Sitefinity designer mode
        /// </summary>
        public bool IsDesignerMode { get; set; }

        /// <summary>
        /// Redirect URL for unauthenticated users
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Preview transactions for designer mode
        /// </summary>
        public TransactionHistoryItem[] PreviewTransactions { get; set; }
    }

    /// <summary>
    /// Transaction history item for display
    /// </summary>
    public class TransactionHistoryItem
    {
        /// <summary>
        /// Unique transaction identifier
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Account name
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Billing Account Number
        /// </summary>
        public string Ban { get; set; }

        /// <summary>
        /// Invoice number
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Transaction date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Transaction reference code
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Payment method
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Partial payment indicator
        /// </summary>
        public string PartialPayment { get; set; }

        /// <summary>
        /// Transaction status
        /// </summary>
        public string Status { get; set; }
    }
}
