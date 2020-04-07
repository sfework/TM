using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class MCache
    {
        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        private static readonly MemoryCacheEntryOptions CacheExpirationOptions = new MemoryCacheEntryOptions() { Priority = CacheItemPriority.NeverRemove };

        public static void Set(object Key, object Value)
        {
            Cache.Set(Key, Value, CacheExpirationOptions);
        }
        public static T Get<T>(object Key)
        {
            return Cache.Get<T>(Key);
        }


        public enum MCacheTag
        {
            AuthList
        }
    }
}
