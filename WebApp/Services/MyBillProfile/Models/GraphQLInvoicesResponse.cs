using System;

namespace VFL.Renderer.Services.MyBillProfile.Models
{
    /// <summary>
    /// GraphQL response model for invoices query
    /// </summary>
    public class GraphQLInvoicesResponse
    {
        public GraphQLInvoicesData Data { get; set; }
    }

    public class GraphQLInvoicesData
    {
        public InvoiceInfo[] Invoices { get; set; }
    }

    /// <summary>
    /// Invoice information from GraphQL API
    /// </summary>
    public class InvoiceInfo
    {
        /// <summary>
        /// Invoice number (e.g., "1123815322")
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Invoice amount in cents (divide by 100 for dollars)
        /// Example: 516056 = $5,160.56
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Billing cycle start date
        /// </summary>
        public DateTime? BillingCycleStartDate { get; set; }

        /// <summary>
        /// Billing cycle end date
        /// </summary>
        public DateTime? BillingCycleEndDate { get; set; }

        /// <summary>
        /// Payment due date
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// PDF file name (e.g., "1123815322-911068956.pdf")
        /// Null if file not available
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Payment status (true = paid, false = unpaid)
        /// </summary>
        public bool PaymentStatus { get; set; }

        /// <summary>
        /// Previous balance before this invoice in cents (divide by 100 for dollars)
        /// </summary>
        public decimal PreBalance { get; set; }
    }
}
