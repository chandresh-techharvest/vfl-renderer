using System.Collections.Generic;

namespace VFL.Renderer.Entities.PurchasePlan
{
    public class PurchasePlanDto
    {

        public int PlanId { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
        public string AmountWithCurrency { get; set; }
        public string PlanType { get; set; }
        public string Network { get; set; }

        public List<PlanCodeDto> PlanCodes { get; set; }

    }
    public class PlanCodeDto
    {
        public string Pcode { get; set; }
        public string PaymentMethod { get; set; }
    }
    public class PlanTypeDto
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
    }


    public class PaymentMethodDto
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
    }




    public class PlanTypeResponse
    {
        public List<PlanTypeDto> AllCategories { get; set; }
    }


    public class PlanResponse
    {
        public List<PurchasePlanDto> AllPlans { get; set; }
    }

    public class PymentMethodResponse
    {
        public List<PaymentMethodDto> AllCategories { get; set; }
    }



}
