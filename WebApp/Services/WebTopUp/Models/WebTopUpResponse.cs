using System;

namespace VFL.Renderer.Services.WebTopUp.Models
{
    public class WebTopUpResponse
    {
        public bool IsSuccess { get; internal set; }
        public string Message { get; internal set; }

       

        public MPaisaPaymentResponse mPaisaPaymentResponse { get; set; }
        public CreditCardPaymentResponse creditCardPaymentResponse { get; set; }

        public string RedirectUrl =>
            mPaisaPaymentResponse?.redirectUrl
            ?? creditCardPaymentResponse?.redirectUrl;
            
    }

    public class MPaisaPaymentResponse
    {
        public string redirectUrl { get; set; }
    }

    public class CreditCardPaymentResponse
    {
        public string redirectUrl { get; set; }
    }

  
}
