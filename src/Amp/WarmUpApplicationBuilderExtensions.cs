using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Amp
{
    public static class WarmUpApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWarmUp(this IApplicationBuilder app)
            => app.UseMiddleware<WarmUpMiddleware>();

        public static Task WarmUpAsync(this IApplicationBuilder app)
        {
            var warmUpService = app.ApplicationServices.GetRequiredService<IWarmUpService>();
            return warmUpService.WarmUpAsync();
        }
    }
}
