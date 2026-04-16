using System;
using System.Collections.Generic;
using VFL.Renderer.Entities.PrepayGifting;
using VFL.Renderer.Entities.PurchasePlan;

namespace VFL.Renderer.Services.PrepayGifting.Models
{
    public class PrepayGiftingResponse
    {

        public string code { get; set; }
        public List<PrepayGiftingDto> AllPlans { get; set; }
    }





    public class PrepayGiftingDto
    {

        public int PlanId { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
        public string AmountWithCurrency { get; set; }
        public string PlanType { get; set; }
        public string Network { get; set; }

        public List<PrepayGiftingCodeDto> PlanCodes { get; set; }



    }


    public class PrepayGiftingCodeDto
    {
        public string Pcode { get; set; }
        public string PaymentMethod { get; set; }
    }


    //public class SubscribeResponse
    //{
    //    public SubscribeData Data { get; set; }
    //}

    public class SubscribeData
    {
        public bool isSuccessful { get; set; }

        public bool isCodeValid { get; set; }

        public SubscribeInformation information { get; set; }
    }

    public class SubscribeInformation
    {
        

        public string GifterNumber { get; set; }

        public string OrderReference { get; set; }

        public string Amount { get; set; }

        public string Number { get; set; }

        public DateTime Date { get; set; }

        public string Email { get; set; }

        public string PlanName { get; set; }

        public string Process { get; set; }
    }


    public class GetRealMoneyResponse
    {
        public string Ammount { get; set; }
    }



}


