using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMT
{
    public static class ErrorHelperExpansion
    {
        public static IApplicationBuilder UseErrorHelper(this IApplicationBuilder app)
        {
            var Context = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            var LoggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            LoggerFactory.AddProvider(new ErrorHelper(Context));
            return app;
        }
    }
    public class ErrorHelper : ILoggerProvider
    {
        private readonly IHttpContextAccessor Context;
        public ErrorHelper(IHttpContextAccessor _Context)
        {
            Context = _Context;
        }
        public ILogger CreateLogger(string CategoryName)
        {
            return new ErrorHandle(Context);
        }

        public void Dispose()
        {

        }
    }
    public class ErrorHandle : ILogger
    {
        private readonly IHttpContextAccessor Context;
        public ErrorHandle(IHttpContextAccessor _Context)
        {
            Context = _Context;
        }

        public LogLevel MinLevel { get; set; } = LogLevel.Error;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (this.IsEnabled(logLevel))
            {
                if (Context.HttpContext.WebSockets.IsWebSocketRequest)
                {
                    var JSRuntime = Context.HttpContext.RequestServices.GetRequiredService<IJSRuntime>();
                    JSRuntime.InvokeVoidAsync("app.log", exception.Message);
                    Console.WriteLine("S:" + exception.Message);
                }
                else
                {
                    Context.HttpContext.Response.Redirect("/error");
                    Console.WriteLine("W:" + exception.Message);
                }
            }
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return this.MinLevel <= logLevel;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return _DisposableInstance;
        }
        class Disposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
        readonly Disposable _DisposableInstance = new Disposable();
    }
}