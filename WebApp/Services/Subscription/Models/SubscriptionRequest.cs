namespace VFL.Renderer.Services.Subscription.Models
{
    public class SendOtpRequest
    {

        public string Number { get; set; }
        public int Option { get; set; }

        public string PCode { get; set; }

      

        public int processId { get; set; }


    }
    

    public class SubscriptionRequest
    {

        public string Number { get; set; }
        
        public string PCode { get; set; }


        public string oCresponse { get; set; }
        public string  OtpCode { get; set; }


    }





}
