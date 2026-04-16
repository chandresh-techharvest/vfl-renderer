using Microsoft.Extensions.Caching.Memory;
using System;

namespace VFL.Renderer.Models.MyBillCheckout
{
    /// <summary>
    /// Memory-based implementation of MyBill checkout session store
    /// </summary>
    public class CheckoutSessionStore : ICheckoutSessionStore
    {
        private readonly IMemoryCache _cache;

        public CheckoutSessionStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Save a checkout session with 15-minute expiry
        /// </summary>
        public void Save(CheckoutSession session)
        {
            _cache.Set(
                session.Id,
                session,
                TimeSpan.FromMinutes(15)
            );
        }

        /// <summary>
        /// Retrieve a checkout session by ID
        /// </summary>
        public CheckoutSession Get(string id)
        {
            _cache.TryGetValue(id, out CheckoutSession session);
            return session;
        }

        /// <summary>
        /// Remove a checkout session by ID
        /// </summary>
        public void Remove(string id)
        {
            _cache.Remove(id);
        }
    }
}
