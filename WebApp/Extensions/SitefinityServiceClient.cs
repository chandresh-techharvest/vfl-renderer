using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace VFL.Renderer.Extensions
{
    public class SitefinityServiceClient
    {
        private readonly HttpClient _httpClient;

        public SitefinityServiceClient(HttpClient httpClient)
        { 
            _httpClient = httpClient;
        }

        public async Task<string> GetProtectedDataAsync(string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync("https://localhost:1473/api/default");

            //response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}
