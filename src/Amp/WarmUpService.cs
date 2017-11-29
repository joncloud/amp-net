using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amp
{
    class WarmUpService : IWarmUpService
    {
        readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        readonly IWarmUp[] _warmUps;
        Func<Task> _fn;
        bool _warm;

        public WarmUpService(IEnumerable<IWarmUp> warmUps, IOptions<WarmUpOptions> options)
        {
            _warmUps = warmUps.ToArray();

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

        Task ColdParallelAsync()
            => Task.WhenAll(_warmUps.Select(x => x.InvokeAsync()));

        async Task ColdSequentialAsync()
        {
            foreach (var warmUp in _warmUps)
            {
                await warmUp.InvokeAsync();
            }
        }

        public Task WarmUpAsync() => _fn();
    }
}
