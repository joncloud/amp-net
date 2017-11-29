using Microsoft.AspNetCore.Http;

namespace Amp
{
    public interface IWarmUpBuilder
    {
        PathString Path { get; set; }
        WarmUpParallelism Parallelism { get; set; }
    }
}
