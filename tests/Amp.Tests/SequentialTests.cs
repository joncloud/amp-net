using Amp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Amp.Tests
{
    class SequencingWarmUp : IWarmUp
    {
        static int _master = 0;
        public int Sequence { get; private set; }

        public Task InvokeAsync()
        {
            Sequence = ++_master;
            return Task.CompletedTask;
        }
    }

    public class SequentialTests : IntegrationTest
    {
        [Fact]
        public Task ShouldExecuteWarmUpsSequentially() =>
            TestAsync(async client =>
            {
                await client.GetAsync("/warmup");

                string json = await client.GetStringAsync("/");
                var numbers = JsonConvert.DeserializeObject<int[]>(json);
                
                Assert.Equal(100, numbers.Length);
                Assert.Equal(Enumerable.Range(1, 100), numbers);
            });

        protected override void Configure(IApplicationBuilder app)
            => app.UseWarmUp();

        protected override void ConfigureServices(IServiceCollection services)
            => Enumerable.Range(0, 100)
                .Aggregate(services, (svc, _) => svc.AddSingleton<IWarmUp, SequencingWarmUp>())
                .AddWarmUp(options => options.Parallelism = WarmUpParallelism.Sequential);
        protected override void Run(IApplicationBuilder app)
        {
            app.Run(async ctx =>
            {
                var sequences = ctx.RequestServices
                    .GetServices<IWarmUp>()
                    .OfType<SequencingWarmUp>()
                    .Select(x => x.Sequence);
                string json = JsonConvert.SerializeObject(sequences);
                await ctx.Response.WriteAsync(json);
            });
        }
    }
}
