using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.PrepayGifting;
using VFL.Renderer.Services.PrepayGifting.Models;


namespace VFL.Renderer.Services.WebTopUp
{
    public class PrepayGiftingService : IPrepayGiftingService
    {
        WebClient _client;
        private readonly ILogger<WebClient> _logger;
        public PrepayGiftingService(WebClient webClient, ILogger<WebClient> logger)
        {
            _client = webClient;
            _logger = logger;
        }
        public async Task<ApiResponse<T>> SendOtpRequest<T>(PrepayGiftingRequest request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/PrepayGift/SendOtpRequest", content, "application/json");
                return response;
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, "Error calling POST {Url}", $"api/PrepayGift/SendOtpRequest");
                throw;
            }
        }





        public async Task<ApiResponse<T>> GetRealMoneyBalance<T>(string number)
        {
            try
            {
                var url = $"api/General/GetRealMoneyBalance/{number}";

                var response = await _client.GetWithoutTokenAsync<T>(url);

                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling GET GetRealMoneyBalance");
                throw;
            }
        }






        public async Task<ApiResponse<T>> PrepayGiftSubscribe<T>(SubscribeRequest request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/PrepayGift/Subscribe", content, "application/json");
                return response;
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, "Error calling POST {Url}", $"/api/PrepayGift/Subscribe");
                throw;
            }
        }






        public async Task<ApiResponse<T>> GetAllGiftingPlans<T>(int number)
        {
            var query = $@"
query {{
    allPlans(
        number: ""{number}"",
        where: {{
            planType: {{
                eq: ""GIFT""
            }}
        }},
        order: [{{
            amount: ASC
        }}]
    ) {{
        planId
        name
        summary
        details
        amountWithCurrency
        planType
        network
        planCodes {{
            pcode
            paymentMethod
        }}
    }}
}}
";            
            try
            {
                var response = await _client.PostGraphQLAsync<T>(query);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST GetAllGiftingPlansAsync Service Layer");
                throw;
            }
        }

        //--------------------------------------------------------
        public async Task<ApiResponse<T>> GetPlanByPlanId<T>(int number,int planId)
        {



            var query = $@"
query {{
  allPlans(
    number: ""{number}""
    where: {{
      planType: {{ eq: ""GIFT"" }}
      planId: {{ eq: {planId} }}
      planCodes: {{
        some: {{
          paymentMethod: {{ eq: ""REAL_MONEY"" }}
        }}
      }}
    }}
    order: [{{ amount: ASC }}]
  ) {{
    planId
    name
    summary
    details
    amountWithCurrency
    planType
    network
    planCodes {{
      pcode
      paymentMethod
    }}
  }}
}}";

            try
            {
                var response = await _client.PostGraphQLAsync<T>(query);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST GetAllGiftingPlansAsync Service Layer");
                throw;
            }
        }

    }
}
