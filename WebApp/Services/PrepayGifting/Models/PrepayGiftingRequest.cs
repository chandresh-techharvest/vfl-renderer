namespace VFL.Renderer.Services.PrepayGifting.Models
{
    public class PrepayGiftingRequest
    {

        public int Option { get; set; }

        public string PCode { get; set; }

        public string ReceiverNumber { get; set; }

        public string SenderNumber { get; set; }


    }

    public class SubscribeRequest
    {
        public string OtpCode { get; set; }

        public string PCode { get; set; }

        public string ReceiverNumber { get; set; }


        public string oCresponse { get; set; }

        public string SenderNumber { get; set; }
    }

    public class GetRealMoneyRequest
    {
        public int Number { get; set; }
    }
    

}
