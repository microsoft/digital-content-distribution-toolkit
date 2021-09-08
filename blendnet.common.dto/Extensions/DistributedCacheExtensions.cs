using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.common.dto.Extensions
{
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// Retrieves the value from cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="distributedCache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key)
        {
            string valueFromCache = await distributedCache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(valueFromCache))
            {
                return JsonSerializer.Deserialize<T>(valueFromCache);
            }

            return default(T);
        }

        /// <summary>
        /// Stores the JSON Serialized Form to cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="distributedCache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="durationInHours"></param>
        /// <returns></returns>
        public static async Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, int durationInHours)
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();

            options.SetAbsoluteExpiration(TimeSpan.FromHours(durationInHours));

            string jsonToAdd = JsonSerializer.Serialize(value);

            await distributedCache.SetStringAsync(key, jsonToAdd, options);

        }
    }
}
