using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Ngonzalez.Util
{
    public static class CacheUtilExtensions
    {
        public static object SetWithAbsolute(this IMemoryCache cache, string key, object value, TimeSpan time)
        {
            return cache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(time));
        }

        public static void SetWithAbsolute(this IDistributedCache cache, string key, object value, TimeSpan time)
        {
            if (value != null)
            {
                cache.Set(key, value.Serializer(), new DistributedCacheEntryOptions().SetAbsoluteExpiration(time));
            }
        }

        public async static Task<T> GetValueAsync<T>(this IDistributedCache cache, string key) where T : class
        {
            var temp = await cache.GetAsync(key).ConfigureAwait(false);
            if (temp != null)
            {
                return Deserializer<T>(temp);
            }
            return null;
        }

        public static Task SetWithAbsoluteAsync(this IDistributedCache cache, string key, object value, TimeSpan time)
        {
            return cache.SetAsync(key, Serializer(value), new DistributedCacheEntryOptions().SetAbsoluteExpiration(time));
        }

        public static byte[] Serializer(this object value)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Encoding.UTF8.GetBytes(json);
        }

        public static T Deserializer<T>(this byte[] byteArray)
        {
            var json = Encoding.UTF8.GetString(byteArray);
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
