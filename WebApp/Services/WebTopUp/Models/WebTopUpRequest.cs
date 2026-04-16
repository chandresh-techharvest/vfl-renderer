namespace VFL.Renderer.Services.WebTopUp.Models
{
    public class WebTopUpRequest
    {
        public string Email { get; set; }
        public string Number { get; set; }
        public PaymentInformationRequest PaymentInformationRequest { get; set; }
        public string Amount { get; set; }

        public string planCode { get; set; }


    }
    public class PaymentInformationRequest
    {
        public MPaisaPayment MPaisaPayment { get; set; }
        public CreditCardPayment CreditCardPayment { get; set; }
    }

    public class MPaisaPayment
    {
        public string RedirectUrl { get; set; }
    }

    public class CreditCardPayment
    {
        public string Billing_Address1 { get; set; }
        public string Billing_City { get; set; }
        public string Billing_CountryCode { get; set; }
        public string Billing_Email { get; set; }
        public string Billing_Firstname { get; set; }
        public string Billing_Lastname { get; set; }
        public string Billing_PhoneNumber { get; set; }
        public string Billing_PostalCode { get; set; }
        public string Billing_State { get; set; }
        public string RedirectUrl { get; set; }
        public string Billing_Address2 { get; set; }
        public string Billing_Address3 { get; set; }
    }





}
