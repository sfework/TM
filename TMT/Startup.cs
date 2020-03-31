using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Options;

namespace TMT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<Command.Helper.CacheHelper>();
            services.AddLogging(options => { options.ClearProviders(); });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(App => App.Run(ExceptionHandler));
            app.UseStaticFiles();
            app.UseRouting();
            app.UseErrorHelper();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
        public async Task ExceptionHandler(HttpContext Context)
        {
            IExceptionHandlerFeature ExModel = Context.Features.Get<IExceptionHandlerFeature>();
            Exception Ex = ExModel?.Error;
            if (Ex != null)
            {
                await Context.Response.WriteAsync(Ex.Message);
            }
        }
    }
}