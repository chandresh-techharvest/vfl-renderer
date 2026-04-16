using Newtonsoft.Json;
using System;

namespace VFL.Renderer.Services.MyBillPayment.Models
{
    /// <summary>
    /// Response model from payment verification
    /// Maps backend API response fields to consistent property names
    /// </summary>
    public class MyBillProvideUpdateResponse
    {
        /// <summary>
        /// Indicates if payment was successful
        /// </summary>
        [JsonProperty("isSuccessful")]
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Transaction reference number (Order reference)
        /// </summary>
        [JsonProperty("orderReference")]
        public string OrderReference { get; set; }

        /// <summary>
        /// Transaction reference (alias for compatibility)
        /// Backend may return this instead of orderReference
        /// </summary>
        [JsonProperty("transactionReference")]
        public string TransactionReference 
        { 
            get => OrderReference;
            set => OrderReference = value;
        }

        /// <summary>
        /// Payment amount
        /// </summary>
        [JsonProperty("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Invoice number paid
        /// </summary>
        [JsonProperty("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// BAN (Billing Account Number)
        /// </summary>
        [JsonProperty("ban")]
        public string Ban { get; set; }

        /// <summary>
        /// Customer full name (backend returns fullName instead of email)
        /// </summary>
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        /// <summary>
        /// Customer email address (may be empty if backend returns fullName instead)
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Payment date/time
        /// </summary>
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Error message if verification failed
        /// </summary>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
