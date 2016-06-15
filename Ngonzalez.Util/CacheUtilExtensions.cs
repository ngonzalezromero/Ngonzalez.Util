using System;
using System.IO;
using System.Text;
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

        public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T value)
        {
            try
            {
                value = Deserializer<T>(cache.Get(key));
                return true;
            }
            catch (Exception)
            {
                value = default(T);
                return false;
            }
        }

        private static byte[] Serializer(this object value)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.None);

            byte[] bytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write(json);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }

        private static T Deserializer<T>(this byte[] byteArray)
        {
            var json = Encoding.UTF8.GetString(byteArray);
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
