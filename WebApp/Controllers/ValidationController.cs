using AngleSharp.Io;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.Services.Validation;
using VFL.Renderer.Services.Validation.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VFL.Renderer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {
        private readonly IValidationService _validationService;
        public ValidationController(IValidationService validationService)
        {
            _validationService = validationService;
        }
        // GET: api/<ValidationController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValidationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public async Task<IActionResult> CheckEmailIsRegistered([FromBody] string value)
        {
            EmailVerifyRequest request = new EmailVerifyRequest
            {
                email = value
            };
            try
            {
                var response = await _validationService.CheckEmailIsRegisteredAsync<ValidationResponse>(request);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == 0)
                {
                    response.status = 200;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = new StringContent(JsonConvert.SerializeObject(ex));
                return BadRequest(responseMessage);
            }
        }

        // POST api/<ValidationController>
        [HttpPost]
        public async Task<IActionResult> CheckNumberRegistered([FromBody] string value)
        {
            NumberVerifyRequest request = new NumberVerifyRequest
            {
                number = value
            };
            try
            {
                var response = await _validationService.CheckNumberIsRegisteredAsync<ValidationResponse>(request);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == 0)
                {
                    response.status = 200;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckNumberIsValid([FromBody] string value)
        {
            NumberVerifyRequest request = new NumberVerifyRequest
            {
                number = value
            };
            try
            {
                var response = await _validationService.CheckNumberIsValidAsync<ValidationResponse>(request);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == 0)
                {

                    response.status = 200;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }




        [HttpPost]
        public async Task<IActionResult> CheckNumberIsValid_AllowInactiveNumber([FromBody] string value)
        {
            NumberVerifyRequest request = new NumberVerifyRequest
            {
                number = value
            };
            try
            {
                var response = await _validationService.CheckNumberIsValid_AllowInactiveNumber<ValidationResponse>(request);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == 0)
                {

                    response.status = 200;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


















        [HttpPost]
        public async Task<IActionResult> CheckNumber([FromBody] string value)
        {
            NumberVerifyRequest request = new NumberVerifyRequest
            {
                number = value
            };
            try
            {
                var response = await _validationService.CheckNumberIsValidAsync<ValidationResponse>(request);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == 0)
                {
                    if (response.data.isNumberValid)
                    {
                        var resRegistered = await _validationService.CheckNumberIsRegisteredAsync<ValidationResponse>(request);
                        resRegistered.data.isNumberValid = response.data.isNumberValid;
                        if (resRegistered.StatusCode == HttpStatusCode.OK || resRegistered.StatusCode == 0)
                        {
                            resRegistered.status = 200;
                            resRegistered.StatusCode = HttpStatusCode.OK;
                            return Ok(resRegistered);
                        }
                        else
                        {
                            return Ok(resRegistered);
                        }
                    }
                    else
                    {
                        return Ok(response);
                    }
                        
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        
        // PUT api/<ValidationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<ValidationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
