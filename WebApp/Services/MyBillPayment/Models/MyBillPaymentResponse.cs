using Newtonsoft.Json;

namespace VFL.Renderer.Services.MyBillPayment.Models
{
    /// <summary>
    /// Response model from MyBill payment API
    /// Matches the actual API structure with nested payment responses
    /// </summary>
    public class MyBillPaymentResponse
    {
        /// <summary>
        /// M-PAISA payment response (if M-PAISA was selected)
        /// </summary>
        [JsonProperty("mPaisaPaymentResponse")]
        public MPaisaPaymentResponseData MPaisaPaymentResponse { get; set; }

        /// <summary>
        /// Credit card payment response (if CARD was selected)
        /// </summary>
        [JsonProperty("creditCardPaymentResponse")]
        public CreditCardPaymentResponseData CreditCardPaymentResponse { get; set; }

        /// <summary>
        /// Get the redirect URL from whichever payment method was used
        /// </summary>
        [JsonIgnore]
        public string RedirectUrl => 
            CreditCardPaymentResponse?.RedirectUrl ?? 
            MPaisaPaymentResponse?.RedirectUrl;

        /// <summary>
        /// Transaction reference number
        /// </summary>
        [JsonIgnore]
        public string TransactionReference => 
            CreditCardPaymentResponse?.TransactionReference ?? 
            MPaisaPaymentResponse?.TransactionReference;

        /// <summary>
        /// Indicates if payment request was successful
        /// </summary>
        [JsonIgnore]
        public bool IsSuccessful => 
            CreditCardPaymentResponse != null || 
            MPaisaPaymentResponse != null;

        /// <summary>
        /// Error message if any
        /// </summary>
        [JsonIgnore]
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// M-PAISA payment response data
    /// </summary>
    public class MPaisaPaymentResponseData
    {
        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonProperty("transactionReference")]
        public string TransactionReference { get; set; }
    }

    /// <summary>
    /// Credit card payment response data
    /// </summary>
    public class CreditCardPaymentResponseData
    {
        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonProperty("transactionReference")]
        public string TransactionReference { get; set; }
    }
}
