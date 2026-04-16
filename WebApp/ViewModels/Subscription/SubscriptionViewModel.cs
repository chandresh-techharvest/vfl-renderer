using System.Collections.Generic;

using VFL.Renderer.Services.Subscription.Models;

namespace VFL.Renderer.ViewModels.Subscription
{
    public class SubscriptionViewModel
    {
        public List<SubscriptionDto> AllPlans { get; set; }
        public List<SubscriptionDto> subscribedPlans { get; set; }
        public List<SubscriptionDto> ActivePlans { get; set; }

        public List<PlanTypeDto> PlanTypes { get; set; }
    }
}
