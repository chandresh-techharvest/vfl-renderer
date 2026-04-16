using Microsoft.Extensions.Logging;

using System.Numerics;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.PrepayGifting.Models;
using VFL.Renderer.Services.Subscription.Models;

namespace VFL.Renderer.Services.Subscription
{
    public class SubscriptionService : ISubscriptionService
    {

        private readonly WebClient _client;
        private readonly ILogger<WebClient> _logger;

        public SubscriptionService(WebClient client, ILogger<WebClient> logger) {
            _client = client;
            _logger = logger;


        }





        public async Task<ApiResponse<T>> SendOtpRequest<T>(SendOtpRequest request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/PlanActivation/SendOtpRequest", content, "application/json");
                return response;
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, "Error calling POST {Url}", $"api/PlanActivation/SendOtpRequest");
                throw;
            }
        }



        public async Task<ApiResponse<T>> Subscribe<T>(SubscriptionRequest request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/PlanActivation/Subscribe", content, "application/json");
                return response;
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, "Error calling POST {Url}", $"/api/PlanActivation/Subscribe");
                throw;
            }
        }

        public async Task<ApiResponse<T>> Resubscribe<T>(SubscriptionRequest request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/PlanActivation/Resubscribe", content, "application/json");
                return response;
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, "Error calling POST {Url}", $"/api/PlanActivation/Resubscribe");
                throw;
            }
        }




        public async Task<ApiResponse<T>> Unsubscribe<T>(SubscriptionRequest request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/PlanActivation/Unsubscribe", content, "application/json");
                return response;
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, "Error calling POST {Url}", $"/api/PlanActivation/Unsubscribe");
                throw;
            }
        }

        public async Task<ApiResponse<T>> GetAllSubscribedPlansByNumber<T>(int number)
        {
            var query = $@"query{{
  subscribedPlans(number: ""{number}""){{
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
                var response = await _client.PostWithTokenGraphQLAsync<T>(query);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST GetAllGiftingPlansAsync Service Layer");
                throw;
            }
        }

        public async Task<ApiResponse<T>> GetAllSubscriptionPlans<T>(int number)
        {
            var query = $@"query {{
    allPlans(number: ""{number}"", where: {{
        planCodes: {{ some:  {{  paymentMethod: {{eq: ""REAL_MONEY""}}}}}} and: [ {{
           planType:  {{
              neq: ""GIFT""
           }}
        }}]
    }}, order: [ {{
       amount: ASC
    }}]){{
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

        public async Task<ApiResponse<T>> GetAllSubscriptionPlansByPlanType<T>(int number, string planType)
        {
            var query = $@"query {{
  allPlans(
    number: ""{number}""
    where: {{
      and: [
        {{ planType: {{ eq: ""{planType}"" }} }}
        {{ planType: {{ neq: ""GIFT"" }} }}
        {{
          planCodes: {{
            some: {{
              paymentMethod: {{ eq: ""REAL_MONEY"" }}
            }}
          }}
        }}
      ]
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



        public async Task<ApiResponse<T>> GetPurchasePlanTypesAsync<T>()
        {
            var query = @"query{
                             allCategories(where:  {
                             categoryType:  {
                                              eq: ""PlanTypes""
                                            }
                                           and: [ {
                                         identifier:  {
                                                     neq: ""GIFT""
                                                       }
                                                  }]
                                             }){
                                            name
                                           identifier
                                           }
                                           }";


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






        public async Task<ApiResponse<T>> GetPlanByPlanId<T>(int number, int planId)
        {
            var query = $@"query {{
    allPlans(number: ""{number}"", where: {{
        planId:  {{
           eq: { planId}
        }}
    }}, order: [ {{
       amount: ASC
    }}]){{
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
