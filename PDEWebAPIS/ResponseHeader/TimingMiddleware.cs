using System.Diagnostics;

namespace PDEWebAPIS.RequestHeader
{
    public class TimingMiddleware
    {
        private readonly RequestDelegate _next;
        public TimingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Took"] = $"{stopwatch.ElapsedMilliseconds} ms";
                return Task.CompletedTask;
            });

            await _next(context);
        }

    }
}
