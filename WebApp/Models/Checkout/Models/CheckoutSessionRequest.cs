using System.Collections.Generic;

namespace VFL.Renderer.Models.Checkout.Models
{
    public class CheckoutSessionRequest
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PlanCode { get; set; } // for webtopup
        public List<PlanCodeDto> PlanCodes { get; set; } // for purchaseplan

        public string PlanName { get; set; }

        public decimal Amount { get; set; }
        public string PageUrl { get; set; }
        public string PageType { get; set; }
    }

    public class PlanCodeDto
    {
        public string Pcode { get; set; }
        public string PaymentMethod { get; set; }
    }
}
