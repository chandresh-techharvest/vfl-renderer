using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using System.Collections.Generic;
using VFL.Renderer.Entities.WebTopUp;

namespace VFL.Renderer.Models.WebTopUp
{
    public class WebTopUpViewModel
    {

        public PageNodeDto CheckoutPage { get; set; } 
        public string CustomerSuportPage {  get; set; }
        public List<TopUpAmountDto> TopUpAmounts { get; set; }
    }
}
