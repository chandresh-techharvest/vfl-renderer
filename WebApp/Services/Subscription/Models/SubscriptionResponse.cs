using System;
using System.Collections.Generic;
using VFL.Renderer.Services.PrepayGifting.Models;

namespace VFL.Renderer.Services.Subscription.Models
{
    public class SubscriptionResponse
    {
        public string code { get; set; }
        public List<SubscriptionDto> AllPlans { get; set; }
        public List<SubscriptionDto> subscribedPlans { get; set; }
        public List<SubscriptionDto> ActivePlans { get;  set; }
        public List<PlanTypeDto> AllCategories { get;  set; }
    }

    public class SubscriptionDto
    {

        public int PlanId { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
        public string AmountWithCurrency { get; set; }
        public string PlanType { get; set; }
        public string Network { get; set; }

        public List<SubscriptionCodeDto> PlanCodes { get; set; }



    }



    public class SubscriptionCodeDto
    {
        public string Pcode { get; set; }
        public string PaymentMethod { get; set; }
    }
    public class SubscribeData
    {
        public bool isSuccessful { get; set; }

        public bool isCodeValid { get; set; }

        public SubscribeInformation information { get; set; }
    }

    public class PlanTypeDto
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
    }




    public class SubscribeInformation
    {


  

        public string OrderReference { get; set; }

        public string Amount { get; set; }

        public string Number { get; set; }

        public DateTime Date { get; set; }

        public string Email { get; set; }

        public string PlanName { get; set; }

        public string Process { get; set; }
    }

    public class GetRealMoneyRequest
    {
        public string Number { get; set; }
    }


    public class PlanTypeResponse
    {
        public List<PlanTypeDto> AllCategories { get; set; }
    }

}
