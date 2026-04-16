using System;

namespace VFL.Renderer.Services.MyBillTransactionHistory.Models
{
    /// <summary>
    /// GraphQL response model for transactionHistoryQuery query
    /// Supports cursor-based pagination
    /// </summary>
    public class GraphQLTransactionHistoryResponse
    {
        public GraphQLTransactionHistoryData Data { get; set; }
    }

    public class GraphQLTransactionHistoryData
    {
        public TransactionHistoryQueryResult TransactionHistoryQuery { get; set; }
    }

    /// <summary>
    /// Transaction history query result with pagination info
    /// </summary>
    public class TransactionHistoryQueryResult
    {
        /// <summary>
        /// Total count of all matching records
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Array of transaction edges (nodes with cursors)
        /// </summary>
        public TransactionEdge[] Edges { get; set; }

        /// <summary>
        /// Pagination information for cursor-based pagination
        /// </summary>
        public PageInfo PageInfo { get; set; }
    }

    /// <summary>
    /// Transaction edge containing cursor and node
    /// </summary>
    public class TransactionEdge
    {
        /// <summary>
        /// Cursor for this edge (used for pagination)
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// The transaction data
        /// </summary>
        public TransactionNode Node { get; set; }
    }

    /// <summary>
    /// Transaction data from GraphQL API
    /// </summary>
    public class TransactionNode
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
        /// Invoice number associated with this transaction
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
        /// Transaction amount in dollars
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Payment method (e.g., "Credit Card", "MPaisa")
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Indicates if this is a partial payment ("Yes" or "No")
        /// </summary>
        public string PartialPayment { get; set; }

        /// <summary>
        /// Transaction status (e.g., "Success", "In Progress", "Failed")
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// Pagination information for cursor-based pagination
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// Cursor of the last item in the current page
        /// Use this as 'after' parameter to get next page
        /// </summary>
        public string EndCursor { get; set; }

        /// <summary>
        /// Indicates if there are more pages available
        /// </summary>
        public bool HasNextPage { get; set; }
    }
}
