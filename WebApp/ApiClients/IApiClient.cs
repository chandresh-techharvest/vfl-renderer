using System;
using System.Threading.Tasks;

namespace VFL.Renderer.ApiClients
{
    public interface IApiClient
    {
        Task<T> PostAsync<T>(string endpoint, object data);
        Task<T> GetAsync<T>(string endpoint, string cacheKey, TimeSpan? cacheDuration = null);
    }
}
