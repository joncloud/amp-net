using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Amp.Tests
{
    class ScopedWarmUp : IWarmUp
    {
        static int _counter;
        public int Value { get; }

        public ScopedWarmUp() => Value = ++_counter;

        public Task InvokeAsync() => Task.CompletedTask;
    }

    public class ScopedTests : IntegrationTest
    {
        [Fact]
        public Task ShouldSupportScopingWarmUps() =>
            TestAsync(async client =>
            {
                await client.GetAsync("/warmup");

                string json = await client.GetStringAsync("/");
                var numbers = JsonConvert.DeserializeObject<int[]>(json);

                Assert.Equal(10, numbers.Length);
                Assert.Equal(Enumerable.Range(11, 10), numbers);
            });

        protected override void Configure(IApplicationBuilder app)
            => app.UseWarmUp();

        protected override void ConfigureServices(IServiceCollection services)
            => Enumerable.Range(0, 10)
                .Aggregate(services, (svc, _) => svc.AddScoped<IWarmUp, ScopedWarmUp>())
                .AddWarmUp();

        protected override void Run(IApplicationBuilder app) =>
            app.Run(async ctx =>
            {
                var sequences = ctx.RequestServices
                    .GetServices<IWarmUp>()
                    .OfType<ScopedWarmUp>()
                    .Select(x => x.Value);
                string json = JsonConvert.SerializeObject(sequences);
                await ctx.Response.WriteAsync(json);
            });
    }
}
