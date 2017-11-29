using Microsoft.AspNetCore.Http;

namespace Amp
{
    class WarmUpOptions
    {
        public PathString Path { get; set; }
        public WarmUpParallelism Parallelism { get; set; }
    }
}
