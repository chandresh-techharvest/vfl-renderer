using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using VFL.Renderer.Services.TransactionHistroy;
using VFL.Renderer.Services.TransactionHistroy.Models;
using VFL.Renderer.Services.WebTopUp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VFL.Renderer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly ITransactionHistoryService _transactionHistoryService;
        private readonly ILogger<ITransactionHistoryService> _logger;

        public TransactionHistoryController(ITransactionHistoryService transactionHistoryService, ILogger<ITransactionHistoryService> logger)
        {
            _transactionHistoryService = transactionHistoryService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> GetWebTopUpHistory(GraphQLRequest filters)
        {
            try
            {
                var response = await _transactionHistoryService.GetWebTopUpHistory<GraphQLResponse>(filters);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while getting WebTopUp history");

                return BadRequest(new ApiResponse<GraphQLResponse>
                {
                    isException = false,
                    developerErrorMessage = ex.Message + " Unexpected error occurred while getting WebTopUp history"
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetPurchasePlanHistory(GraphQLRequest filters)
        {
            try
            {
                var response = await _transactionHistoryService.GetPurchasePlanHistory<GraphQLResponse>(filters);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while getting Purchase Plan History");

                return BadRequest(new ApiResponse<GraphQLResponse>
                {
                    isException = false,
                    developerErrorMessage = ex.Message + " Unexpected error occurred while getting Purchase Plan History"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDirectTopUpHistory(GraphQLRequest filters)
        {
            try
            {
                var response = await _transactionHistoryService.GetDirectTopUpHistory<GraphQLResponse>(filters);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while getting Direct TopUp History");

                return BadRequest(new ApiResponse<GraphQLResponse>
                {
                    isException = false,
                    developerErrorMessage = ex.Message + " Unexpected error occurred while getting Direct TopUp History"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPlanActivationHistory(GraphQLRequest filters)
        {
            try
            {
                var response = await _transactionHistoryService.GetPlanActivationHistory<GraphQLResponse>(filters);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while getting Plan Activation History");

                return BadRequest(new ApiResponse<GraphQLResponse>
                {
                    isException = false,
                    developerErrorMessage = ex.Message + " Unexpected error occurred while getting Plan Activation History"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPrepayGiftHistory(GraphQLRequest filters)
        {
            try
            {
                var response = await _transactionHistoryService.GetPrepayGiftHistory<GraphQLResponse>(filters);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while getting Prepay Gift History");

                return BadRequest(new ApiResponse<GraphQLResponse>
                {
                    isException = false,
                    developerErrorMessage = ex.Message + " Unexpected error occurred while getting Prepay Gift History"
                });
            }
        }

    }
}
