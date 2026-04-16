namespace VFL.Renderer.Services.WebTopUp.Models
{
    public class ProvideUpdateRequest
    {
        public MPaisaPaymentCallback mPaisaPayment { get; set; }
        public CreditCardPaymentCallback creditCardPayment { get; set; }
    }




    public class MPaisaPaymentCallback
    {
        public string rCode { get; set; }
        public string redirectUrl { get; set; }
        public string requestId { get; set; }
        public string tokenv2 { get; set; }
        public string transactionId { get; set; }
        public string customerPhoneNumber { get; set; }
        public string token { get; set; }
    }

    public class CreditCardPaymentCallback
    {
        public string sessionId { get; set; }
    }


}
