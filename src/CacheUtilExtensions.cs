using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

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
            if(value != null)
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
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, value);
                bytes = memoryStream.ToArray();

            }
            return bytes;
        }

        private static T Deserializer<T>(this byte[] byteArray)
        {
            using (var memoryStream = new MemoryStream(byteArray))
            {
                return (T)new BinaryFormatter().Deserialize(memoryStream);
            }
        }
    }
}
