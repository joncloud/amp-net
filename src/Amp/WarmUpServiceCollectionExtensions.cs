using Microsoft.Extensions.DependencyInjection;
using System;

namespace Amp
{
    public static class WarmUpServiceCollectionExtensions
    {
        public static IServiceCollection AddWarmUp(this IServiceCollection services)
            => services.AddWarmUp(builder => builder.Path = "/warmup");
        
        public static IServiceCollection AddWarmUp(this IServiceCollection services, Action<IWarmUpBuilder> configure)
        {
            var builder = new WarmUpBuilder();
            configure(builder);
            return services.Configure<WarmUpOptions>(builder.Configure)
                .AddSingleton<IWarmUpService, WarmUpService>();
        }
    }
}
