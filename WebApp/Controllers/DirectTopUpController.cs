using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VFL.Renderer.Services.DirectTopUp;
using VFL.Renderer.Services.DirectTopUp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VFL.Renderer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DirectTopUpController : ControllerBase
    {
        #region Local Properties
        DirectTopUpService _directTopUpService;
        private readonly ILogger<DirectTopUpService> _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for Direct TopUp Controller
        /// </summary>
        /// <param name="directTopUpService"></param>
        /// <param name="logger"></param>
        public DirectTopUpController(DirectTopUpService directTopUpService, ILogger<DirectTopUpService> logger)
        {
            _directTopUpService = directTopUpService;
            _logger = logger;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">VFL.Renderer.Services.DirectTopUp.Models.DirectTopUpResponse</typeparam>
        /// <param name="request">VFL.Renderer.Services.DirectTopUp.Models.DirectTopUpRequest</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public async Task<IActionResult> SendRequest(DirectTopUpRequest request)
        {
            try
            {
                var response = await _directTopUpService.SendRequest<DirectTopUpResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok(response);
                }
                else
                {
                    _logger.LogError("Error in DirectTopUp SendRequest: {StatusCode} - {Message}", response.StatusCode, response.developerErrorMessage);
                    return BadRequest(response);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/DirectTopUp/SendRequest");
                return BadRequest(ex);
            }
        }
        #endregion

    }
}
