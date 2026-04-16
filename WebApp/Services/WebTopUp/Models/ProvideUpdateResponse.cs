using System;

namespace VFL.Renderer.Services.WebTopUp.Models
{
    public class ProvideUpdateResponse
    {
        public bool IsSuccessful { get; set; }
        public string OrderReference { get; set; }
        public string Amount { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }

        public string planName { get; set; }
        public  ProvideUpdateResponse provideUpdateResponse { get; internal set; }
    }
}
