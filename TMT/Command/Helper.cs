using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMT.Command
{
    public class Helper
    {
        //Set后必须刷新页面才能Get
        public class CookieHelper
        {
            private readonly IJSRuntime JSRuntime;
            private readonly HttpContext Context;
            private readonly bool IsWebSocketRequest;
            public CookieHelper(IJSRuntime _JSRuntime, HttpContext _Context)
            {
                JSRuntime = _JSRuntime;
                Context = _Context;
                IsWebSocketRequest = Context.WebSockets.IsWebSocketRequest;
            }
            public async Task Set(string Key, string Value, int Exp = 60)
            {
                if (IsWebSocketRequest)
                {
                    await JSRuntime.InvokeVoidAsync("app.cookie.set", Key, Value, Exp);
                }
                else
                {
                    Context.Response.Cookies.Append(Key, Value);
                }
            }
            public async Task<string> Get(string Key)
            {
                if (IsWebSocketRequest)
                {
                    return await JSRuntime.InvokeAsync<string>("app.cookie.get", Key);
                }
                else
                {
                    if (!Context.Request.Cookies.TryGetValue(Key, out string Value))
                    {
                        return string.Empty;
                    }
                    return Value;
                }
            }
            public async Task Delete(string Key)
            {
                if (IsWebSocketRequest)
                {
                    await JSRuntime.InvokeVoidAsync("app.cookie.del", Key);
                }
                else
                {
                    Context.Response.Cookies.Delete(Key);
                }
            }
        }

        public class LocalStorageHelper
        {
            private readonly IJSRuntime JSRuntime;
            public LocalStorageHelper(IJSRuntime _JSRuntime)
            {
                JSRuntime = _JSRuntime;
            }
            public async Task Set(string Key, string Valye)
            {
                await JSRuntime.InvokeVoidAsync("app.storage.set", Key, Valye);
            }
            public async Task<string> Get(string Key)
            {
                return await JSRuntime.InvokeAsync<string>("app.storage.get", Key);
            }  
            public async Task Delete(string Key)
            {
                await JSRuntime.InvokeVoidAsync("app.storage.del", Key);
            }
            public async Task Clear()
            {
                await JSRuntime.InvokeVoidAsync("app.storage.clear");
            }
        }
        public class CacheHelper
        {
            private readonly MemoryCache _Cache = new MemoryCache(new MemoryCacheOptions());
            public void Set(string Key, object Value)
            {
                _Cache.Set(Key, Value);
            }
            public T Get<T>(string Key)
            {
                if (_Cache.TryGetValue(Key, out T Value))
                {
                    return Value;
                }
                return default;
            }
            public void Delete(string Key)
            {
                _Cache.Remove(Key);
            }
            public bool ContainKey(string Key)
            {
                return _Cache.TryGetValue(Key, out _);
            }
        }
    }
}
