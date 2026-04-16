namespace VFL.Renderer.Services.Dashboard.Models
{
    public class DeviceGetRequestModel
    {
        public string number { get; set; }
    }

    public class DeviceRequest : DeviceGetRequestModel
    {
        public string name { get; set; }
        public string otpCode { get; set; }

        public string oCresponse { get; set; }

    }
}
