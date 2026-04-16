using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFL.Renderer.OSApi.Clients
{
    public interface IApiClient
    {
        Task<T?> PostAsync<T>(object data);
        Task<T?> GetAsync<T>(string cacheKey, TimeSpan? cacheDuration = null);
    }
}
