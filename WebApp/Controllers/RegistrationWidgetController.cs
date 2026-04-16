using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Progress.Sitefinity.AspNetCore.ViewComponents;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using Progress.Sitefinity.RestSdk;
using System.Threading.Tasks;
using VFL.Renderer.Entities.Registration;
using VFL.Renderer.Models.Registration;
using Progress.Sitefinity.RestSdk.OData;
using Progress.Sitefinity.AspNetCore.RestSdk;
using System;
using System.Net;
using System.Net.Http;
using System.Diagnostics.Eventing.Reader;
using VFL.Renderer.Services.Registration;
using VFL.Renderer.Services.Registration.Models;

[Route("api/[controller]/[action]")]
[ApiController]
public class RegistrationWidgetController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public RegistrationWidgetController(IRegistrationService registrationService, IODataRestClient restService)
    {
        _registrationService = registrationService;
        this.restService = restService;
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
    {
        if (request == null) return BadRequest(HttpStatusCode.NoContent);

        if (!ModelState.IsValid)
        {
            throw new Exception("Model state is invalid");
        }
        try { 
        var response = await _registrationService.RegisterAsync<RegistrationResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
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

    public string GetPageNodeUrl(Progress.Sitefinity.Renderer.Entities.Content.MixedContentContext context)
    {
        if (context?.Content?[0]?.Variations?.Length != 0)
        {
            var pageNodes = this.restService.GetItems<PageNodeDto>(context, new GetAllArgs() { Fields = new[] { nameof(PageNodeDto.ViewUrl) } }).Result;

            var items = pageNodes.Items;

            if (items.Count == 1)
            {
                return items[0].ViewUrl;
            }
        }

        return string.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmEmailAsync(EmailVerify request)
    {
        if (request == null) return BadRequest(HttpStatusCode.NoContent);
        try
        {
            var response = await _registrationService.ConfirmEmailAsync<RegistrationResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
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

    [HttpPost]
    public async Task<IActionResult> ResendConfirmationEmail(ResendEmailVerify request)
    {
        if (request == null) return BadRequest(HttpStatusCode.NoContent);
        try
        {
            var response = await _registrationService.ResendConfirmationEmailAsync<RegistrationResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
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


    private readonly IODataRestClient restService;
}
