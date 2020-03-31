using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Command
{
    public class SessionHelper
    {
        private readonly IHttpContextAccessor Context;
        public SessionHelper(IHttpContextAccessor _Context)
        {
            Context = _Context;
        }
        public void Add(string Key, object Value)
        {
            if (Value != null)
            {
                Context.HttpContext.Session.Set(Key, ByteFormat.Serialize(Value));
            }
        }
        public T Get<T>(string Key) where T : class
        {
            if (Context.HttpContext.Session.TryGetValue(Key, out byte[] Value))
            {
                return ByteFormat.Deserialize<T>(Value);
            }
            return null;
        }
        public void Remove(string Key)
        {
            Context.HttpContext.Session.Remove(Key);
        }
        public void Clear()
        {
            Context.HttpContext.Session.Clear();
        }
        public IEnumerable<string> Keys => Context.HttpContext.Session.Keys;
    }
}