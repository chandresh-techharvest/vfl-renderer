using System;

namespace VFL.Renderer.Services.DirectTopUp.Models
{
    public class DirectTopUpResponse
    {
        public string reference { get; set; }
        public DateTime date { get; set; }
        public string number { get; set; }
        public string email { get; set; }
        public bool isToppedUp { get; set; }
        public string pin { get; set; }
    }
}
