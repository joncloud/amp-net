using Amp;
using System.Threading.Tasks;

namespace Amp.Tests
{
    class StaticWarmUp : IWarmUp
    {
        public int WarmUpCount { get; private set; }

        public Task InvokeAsync()
        {
            WarmUpCount++;
            return Task.CompletedTask;
        }
    }
}
