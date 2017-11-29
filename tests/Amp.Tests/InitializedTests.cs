using Amp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Amp.Tests
{

    public class InitializedTests : IntegrationTest
    {
        [Fact]
        public Task ShouldWarmUpWithoutExplicitCall() =>
            TestAsync(async client =>
            {
                string s = await client.GetStringAsync("/");
                Assert.Equal("1", s);
            });

        [Fact]
        public Task ShouldNotWarmUpTwice() => 
            TestAsync(async client =>
            {
                string[] s = await Task.WhenAll(
                    client.GetStringAsync("/"),
                    client.GetStringAsync("/"));

                Assert.Equal(new[] { "1", "1" }, s);
            });

        protected override void Configure(IApplicationBuilder app)
            => app.WarmUpAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        protected override void ConfigureServices(IServiceCollection services)
            => services.AddWarmUp();
    }
}
