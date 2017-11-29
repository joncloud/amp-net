using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amp
{
    class WarmUpService : IWarmUpService
    {
        readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        readonly IServiceProvider _services;
        Func<Task> _fn;
        bool _warm;

        public WarmUpService(IServiceProvider services, IOptions<WarmUpOptions> options)
        {
            _services = services;

            switch (options.Value.Parallelism)
            {
                case WarmUpParallelism.Parallel:
                    _fn = LockAndSwitch(ColdParallelAsync);
                    break;
                case WarmUpParallelism.Sequential:
                    _fn = LockAndSwitch(ColdSequentialAsync);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options), options.Value.Parallelism, "Invalid Parallelism Value");
            }
        }

        static Task WarmAsync() => Task.CompletedTask;

        Func<Task> LockAndSwitch(Func<Task> fn)
        {
            return async () =>
            {
                await _semaphoreSlim.WaitAsync();
                try
                {
                    if (_warm) return;

                    await fn();

                    _fn = WarmAsync;
                    _warm = true;
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            };
        }

        async Task ColdParallelAsync()
        {
            using (var scope = _services.CreateScope())
            {
                var warmUps = scope.ServiceProvider.GetServices<IWarmUp>();
                await Task.WhenAll(warmUps.Select(x => x.InvokeAsync()));
            }
        }

        async Task ColdSequentialAsync()
        {
            using (var scope = _services.CreateScope())
            {
                var warmUps = scope.ServiceProvider.GetServices<IWarmUp>();
                foreach (var warmUp in warmUps)
                {
                    await warmUp.InvokeAsync();
                }
            }
        }

        public Task WarmUpAsync() => _fn();
    }
}
