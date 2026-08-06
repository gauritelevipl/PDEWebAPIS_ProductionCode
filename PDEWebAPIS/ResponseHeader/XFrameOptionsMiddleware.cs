namespace PDEWebAPIS.RequestHeader
{
    public class XFrameOptionsMiddleware
    {
        private readonly RequestDelegate _next;

        public XFrameOptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Define your Content-Security-Policy here
            //string csp = "default-src 'self'; " +
            //             "script-src 'self' https://trusted-scripts.example.com; " +
            //             "style-src 'self' 'unsafe-inline'; " +
            //             "frame-ancestors 'self' https://trusted-site.example.com";

            //string csp = "default-src 'self';";
            //csp += " frame-ancestors 'self';";

            string csp = "default-src, base-ur";
            // Add the X-Frame-Options header
            context.Response.Headers.Add("X-Frame-Options", "DENY"); // Or "DENY"
           // Add the CSP header
            context.Response.Headers.Add("Content-Security-Policy", csp);
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Add("Cache-Control", "nocache, no-store, max-age=63115200");
            await _next(context);
        }
    }

    // Register middleware in `Program.cs` or `Startup.cs`
    public static class XFrameOptionsMiddlewareExtensions
    {
        public static IApplicationBuilder UseXFrameOptions(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<XFrameOptionsMiddleware>();
        }
    }

}
