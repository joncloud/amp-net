using Amp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Amp.Tests
{
    class TrackingWarmUp : IWarmUp
    {
        public static ManualResetEvent Reset { get; } = new ManualResetEvent(false);
        public DateTime LastWarmedUpOn { get; private set; }

        public Task InvokeAsync()
        {
            Reset.WaitOne();
            LastWarmedUpOn = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }

    public class ParallelTests : IntegrationTest
    {
        [Fact]
        public Task ShouldExecuteWarmUpsInParallel() =>
            TestAsync(async client =>
            {
                var task = client.GetAsync("/warmup");
                TrackingWarmUp.Reset.Set();
                var now = DateTime.UtcNow;
                await task;

                string json = await client.GetStringAsync("/");
                var dates = JsonConvert.DeserializeObject<DateTime[]>(json);

                Assert.Equal(10, dates.Length);

                var threshold = TimeSpan.FromMilliseconds(100);
                var times = dates.Select(date => date - now);
                foreach (var time in times)
                {
                    Assert.True(time < threshold, $"Time not within threshold. Expected {time} < {threshold}");
                }
            });

        protected override void Configure(IApplicationBuilder app)
            => app.UseWarmUp();

        protected override void ConfigureServices(IServiceCollection services)
            => Enumerable.Range(0, 10)
                .Aggregate(services, (svc, _) => svc.AddSingleton<IWarmUp, TrackingWarmUp>())
                .AddWarmUp(options => options.Parallelism = WarmUpParallelism.Parallel);

        protected override void Run(IApplicationBuilder app) =>
            app.Run(async ctx =>
            {
                var dates = ctx.RequestServices
                    .GetServices<IWarmUp>()
                    .OfType<TrackingWarmUp>()
                    .Select(x => x.LastWarmedUpOn);
                string json = JsonConvert.SerializeObject(dates);
                await ctx.Response.WriteAsync(json);
            });
    }
}
