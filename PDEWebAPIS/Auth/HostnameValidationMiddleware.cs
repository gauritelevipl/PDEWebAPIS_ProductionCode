namespace PDEWebAPIS.Auth
{
    public class HostnameValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _allowedHostname;

        public HostnameValidationMiddleware(RequestDelegate next, string allowedHostname)
        {
            _next = next;
            _allowedHostname = allowedHostname;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestHost = context.Request.Host.Host;
            if (!string.Equals(requestHost, _allowedHostname, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: Invalid Hostname");
                return;
            }

            await _next(context);
        }
    }

    // Register middleware in Program.cs or Startup.cs
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<HostnameValidationMiddleware>("expected-hostname.com");

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

}
