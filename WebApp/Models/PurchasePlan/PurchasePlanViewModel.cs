using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using Progress.Sitefinity.RestSdk.Dto;
using System.Collections.Generic;
using VFL.Renderer.Entities.PurchasePlan;

namespace VFL.Renderer.Models.PurchasePlan
{
    public class PurchasePlanViewModel
    {



        public PageNodeDto CheckoutPage { get; set; }
        public string CustomerSuportPage { get; set; }
        public List<PurchasePlanDto> AllPlans { get; set; }
        public List<PlanTypeDto> PlanTypes { get; set; }
        public List<PaymentMethodDto> PaymentMethods { get; set; }

    }
}
