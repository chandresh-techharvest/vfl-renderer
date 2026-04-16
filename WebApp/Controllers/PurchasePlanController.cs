using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VFL.Renderer.Entities.PurchasePlan;
using VFL.Renderer.Services.Plans;

namespace VFL.Renderer.Controllers
{

    [ApiController]
    [Route("PurchasePlan")]
    public class PurchasePlanController : Controller
    {

        private readonly IPlansService _plansService;
        public PurchasePlanController(IPlansService plansService)
        {
            _plansService = plansService;
        }
        [HttpGet("GetPlanById")]
        public async Task<IActionResult> GetPlanById(int number,int planId)
        {
            var result =
                await _plansService.GetPurchasePlanByIdAsync<PlanResponse>(number,planId);

            return Ok(result?.data?.AllPlans?.FirstOrDefault());
        }




        [HttpGet("GetPlansByType")]
        public async Task<IActionResult> GetPlansByType(int number,string planType)
        {
            var result =
                await _plansService.GetAllPurchasePlanByPlanType<PlanResponse>(number,planType);

            return Ok(result?.data?.AllPlans);
        }







        [HttpGet("GetPlansByPaymentMethod")]
        public async Task<IActionResult> GetPlansByPaymentMethod(int number,string paymentMethod)
        {
            var result =
                await _plansService.GetAllPurchasePlanByPaymentMethod<PlanResponse>(number,paymentMethod);

            return Ok(result?.data?.AllPlans);
        }




        [HttpGet("GetPlansByTypeAndPayment")]
        public async Task<IActionResult> GetPlansByTypeAndPayment(int number,string planType, string paymentMethod)
        {
            var result = await _plansService.GetAllPurchasePlanByPaymentMethodandPlanType<PlanResponse>( number,planType,paymentMethod);
            return Ok(result.data.AllPlans);
        }





        [HttpGet("GetAllPlans")]
        public async Task<IActionResult> GetAllPlans(int number)
        {
            var result = await _plansService.GetAllGiftingPlansAsync<PlanResponse>(number);

            return Json(result?.data?.AllPlans);
        }

       

    }




}
