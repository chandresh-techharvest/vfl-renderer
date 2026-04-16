using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using VFL.Renderer.ApiClients;
using VFL.Renderer.Common;
using VFL.Renderer.Services.WebTopUp.Models;

namespace VFL.Renderer.Services.Plans
{
    public class PlansService : IPlansService
    {

        private readonly WebClient _client;
        private readonly ILogger<WebClient> _logger;

        public PlansService(WebClient client, ILogger<WebClient> logger)
        {
            _client = client;
            _logger = logger;
        }


        //For UnAuthenticate user purchaseplan processpayment api

        public async Task<ApiResponse<T>> ProcessPaymentPublic<T>(WebTopUpRequest request)
        {
            try
            {

                var setting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                var content = new StringContent(JsonConvert.SerializeObject(request, setting), null, "application/json");
                var response = await _client.PostAsync<ApiResponse<T>>("api/PurchasePlan/ProcessPayment", content);
                return response;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/PurchasePlan/ProcessPayment");
                throw;
            }
        }



        //For UnAuthenticate user purchaseplan provideupdate api 
        public async Task<ApiResponse<T>> ProvideUpdatePublic<T>(ProvideUpdateRequest request)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");
                var response = await _client.PostAsync<ApiResponse<T>>("api/PurchasePlan/ProvideUpdate", content);
                return response;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/PurchasePlan/ProvideUpdate");
                throw;
            }
        }

        //For Authenticate user purchaseplan processpayment api

        public async Task<ApiResponse<T>> ProcessPayment<T>(WebTopUpRequest request)
        {
            try
            {

                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/PurchasePlan/ProcessPayment", content, "application/json");
                return response;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/PurchasePlan/ProcessPayment");
                throw;
            }
        }

        //For Authenticate user purchaseplan provideupdate api 


        public async Task<ApiResponse<T>> ProvideUpdate<T>(ProvideUpdateRequest request)
        {
            try
            {
                var content = request;
                var response = await _client.PostWithTokenAsync<T>("api/PurchasePlan/ProvideUpdate", content, "application/json");
                return response;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Url}", $"api/PurchasePlan/ProvideUpdate");
                throw;
            }
        }










        public async Task<ApiResponse<T>> GetAllGiftingPlansAsync<T>(int number)
        {
            var query = $@"query {{
  allPlans(
    number: ""{number}""
    where: {{
      and: [
        {{ planType: {{ neq: ""GIFT"" }} }}
        {{
          planCodes: {{
            some: {{
              paymentMethod: {{ neq: ""REAL_MONEY"" }}
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



        //public purchase
        public async Task<ApiResponse<T>> GetAllPublicGiftingPlansAsync<T>(int number)
        {
            const string query = @"query {{
  allPlans(
    number: ""{number}""
    where: {{
      and: [
        {{ planType: {{ neq: ""GIFT"" }} }}
        {{
          planCodes: {{
            some: {{
              paymentMethod: {{ neq: ""REAL_MONEY"" }}
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


        //----------------------------------get all plan type------------------------------------------------
        public async Task<ApiResponse<T>> GetPurchasePlanTypesAsync<T>()
        {
            const string query = @"query{
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



        //----------------------------------get all purchaseplanpayment method type------------------------------------------------
        public async Task<ApiResponse<T>> GetPurchasePlanPaymentMethodsAsync<T>()
        {
            const string query = @"query{ allCategories(where:{ categoryType:{ eq:""PaymentMethods"" } and:[{ identifier:{ neq:""REAL_MONEY"" } }] }){ name identifier } }";


            
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




        //----------------------------------get all purchaseplanbyid method type------------------------------------------------
        //public async Task<ApiResponse<T>> GetPurchasePlanByIdAsync<T>(int planId)
        //{
        //    var request = @"query { allPlans(number:""8063689"", where:{ planId:{ eq:298 } }, order:[{ amount:ASC }]){ planId name summary details amountWithCurrency planType network planCodes { pcode paymentMethod } } }";


        //    var payload = new
        //    {

        //        query = request
        //    };
        //    try
        //    {
        //        var response = await _client.PostGraphQLAsync<T>("graphql/", payload);
        //        return response;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error in POST GetAllGiftingPlansAsync Service Layer");
        //        throw;
        //    }
        //}


        public async Task<ApiResponse<T>> GetPurchasePlanByIdAsync<T>(int number,int planId)
        {
            var query = $@"query {{
        allPlans(
            number:""{number}"",
            where:{{
                planId:{{ eq:{planId} }}
            }},
            order:[{{ amount:ASC }}]
        ){{
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









        public async Task<ApiResponse<T>> GetAllPurchasePlanByPlanType<T>(int number,string  planType)
        {
                           var query = $@"query {{
                                      allPlans(
                                      number: ""{number}""
                                      where: {{
                                      planType:{{ eq: ""{planType}""}}
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


        public async Task<ApiResponse<T>> GetAllPurchasePlanByPaymentMethod<T>(int number,string PaymentMethod)
        {
            var query = $@"query {{
                                          allPlans(
                                        number: ""{number}""
                                           where: {{
                                              planCodes:{{some:{{ paymentMethod:{{eq:""{PaymentMethod}""}} }}}}
                                             and:[{{planType:{{neq:""GIFT""}}}}]
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


        public async Task<ApiResponse<T>> GetAllPurchasePlanByPaymentMethodandPlanType<T>(int number ,string planType, string PaymentMethod)
        {
            var query = $@"query {{allPlans(number: ""{number}"" where: {{and: [{{ planType: {{ eq: ""{planType}"" }} }}{{planCodes: {{some: {{
              paymentMethod: {{ eq: ""{PaymentMethod}"" }}}}}}}}]}} order: [{{ amount: ASC }}]) {{  planId  name  summary
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

        public async Task<ApiResponse<T>> GetallTopUpAmountsAsync<T>()
        {
            var query = @"query{
                              allTopUpAmounts(order: [ {
                                 value: ASC
                              }]){
                                value
                                isDefault
                              }
                            }";


            try
            {
                var response = await _client.PostWithTokenGraphQLAsync<T>(query);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST GetallTopUpAmountsAsync Service Layer");
                throw;
            }
        }





        public async Task<ApiResponse<T>> GetallPublicTopUpAmountsAsync<T>()
        {
            var query = @"query{
                              allTopUpAmounts(order: [ {
                                 value: ASC
                              }]){
                                value
                                isDefault
                              }
                            }";

            try
            {
                var response = await _client.PostGraphQLAsync<T>(query);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in POST GetallTopUpAmountsAsync Service Layer");
                throw;
            }
        }




























    }
}
