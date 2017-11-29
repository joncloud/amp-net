using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Amp
{
    class WarmUpMiddleware
    {
        readonly IWarmUpService _warmUpService;
        readonly WarmUpOptions _options;
        readonly RequestDelegate _next;

        public WarmUpMiddleware(IWarmUpService warmUpService, IOptions<WarmUpOptions> options, RequestDelegate next)
        {
            _warmUpService = warmUpService;
            _options = options.Value;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(_options.Path))
            {
                await _warmUpService.WarmUpAsync();
                context.Response.StatusCode = StatusCodes.Status200OK;
            }
            else
            {
                await _next(context);
            }
        }
    }
}
