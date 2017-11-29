using Amp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Amp.Tests
{
    public class MiddlewareTests : IntegrationTest
    {
        [Fact]
        public Task ShouldWarmUpAfterCallToWarmUpPath() =>
            TestAsync(async client =>
            {
                string s = await client.GetStringAsync("/");
                Assert.Equal("0", s);

                var statusCode = (int)(await client.GetAsync("/warmup")).StatusCode;
                Assert.Equal(StatusCodes.Status200OK, statusCode);

                s = await client.GetStringAsync("/");
                Assert.Equal("1", s);
            });

        [Fact]
        public Task ShouldOnlyWarmUpOnceAfterConsecutiveCalls() =>
            TestAsync(async client =>
            {
                await client.GetAsync("/warmup");
                await client.GetAsync("/warmup");

                string s = await client.GetStringAsync("/");
                Assert.Equal("1", s);
            });

        protected override void ConfigureServices(IServiceCollection services)
            => services.AddWarmUp();

        protected override void Configure(IApplicationBuilder app)
            => app.UseWarmUp();
    }
}
