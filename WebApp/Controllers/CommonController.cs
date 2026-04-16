using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VFL.Renderer.Common;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Azure;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VFL.Renderer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private HttpContext _httpContext;

        public CommonController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        [HttpGet]
        public async Task<string> GetCookie()
        {
            string key = "luData";
            return CookieHelper.GetCookie(HttpContext.Request, key, true);
        }
    }
}
