using Microsoft.AspNetCore.Http;

namespace Amp
{
    class WarmUpBuilder : IWarmUpBuilder
    {
        public PathString Path { get; set; } = "/warmup";
        public WarmUpParallelism Parallelism { get; set; }

        public void Configure(WarmUpOptions options)
        {
            options.Path = Path;
            options.Parallelism = Parallelism;
        }
    }
}
