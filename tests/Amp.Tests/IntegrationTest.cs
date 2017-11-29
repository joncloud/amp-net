using Amp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Amp.Tests
{
    public abstract class IntegrationTest
    {
        protected abstract void ConfigureServices(IServiceCollection services);
        protected abstract void Configure(IApplicationBuilder app);

        protected virtual void Run(IApplicationBuilder app)
        {
            app.Run(async ctx =>
            {
                var warmUp = ctx.RequestServices.GetRequiredService<StaticWarmUp>();
                await ctx.Response.WriteAsync(warmUp.WarmUpCount.ToString());
            });
        }

        protected async Task TestAsync(Func<HttpClient, Task> fn)
        {
            var webHostBuilder = CreateWebHostBuilder();
            using (var server = new TestServer(webHostBuilder))
            using (var client = server.CreateClient())
            {
                await fn(client);
            }
        }

        IWebHostBuilder CreateWebHostBuilder()
            => new WebHostBuilder()
               .ConfigureServices(services =>
               {
                   services.AddSingleton<StaticWarmUp>().AddSingleton<IWarmUp>(svc => svc.GetRequiredService<StaticWarmUp>());
                   ConfigureServices(services);
               })
               .Configure(app =>
               {
                   Configure(app);
                   Run(app);
               });
    }
}
