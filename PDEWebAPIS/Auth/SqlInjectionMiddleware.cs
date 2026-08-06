using System.Text.RegularExpressions;

namespace PDEWebAPIS.Auth
{
    public class SqlInjectionMiddleware
    {
        private readonly RequestDelegate _next;

        // Stronger regex patterns for SQL injection
        private static readonly Regex SqlInjectionRegex = new Regex(
            @"(pg_sleep\s*\()|(union\s+select)|(drop\s+table)|(insert\s+into)|(update\s+\w+\s+set)|(--|;|/\*|\*/)|(\bor\b\s+\d+=\d+)|(\band\b\s+\d+=\d+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public SqlInjectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var fullRequest = context.Request.QueryString.ToString() + " " + context.Request.Path;

            if (SqlInjectionRegex.IsMatch(fullRequest))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Blocked: SQL injection attempt detected.");
                return;
            }

            if (context.Request.ContentLength > 0 &&
                context.Request.ContentLength < 1024 * 100 && // don’t read huge bodies
                (context.Request.Method == HttpMethods.Post ||
                 context.Request.Method == HttpMethods.Put ||
                 context.Request.Method == HttpMethods.Patch))
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (SqlInjectionRegex.IsMatch(body))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Blocked: SQL injection attempt detected.");
                    return;
                }
            }

            await _next(context);
        }
    }

    //public class SqlInjectionMiddleware
    //{
    //    private readonly RequestDelegate _next;

    //    // Define dangerous patterns (you can expand this list)
    //    private static readonly string[] BlockedKeywords =
    //    {
    //    "pg_sleep", "drop", "delete", "insert", "update",
    //    "--", ";", "/*", "*/", "xp_", "exec", "union"
    //};

    //    public SqlInjectionMiddleware(RequestDelegate next)
    //    {
    //        _next = next;
    //    }

    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        // Collect input from Query, Path, and Body
    //        var requestContent = context.Request.QueryString.ToString() +
    //                             context.Request.Path.ToString();

    //        // Check query string and path
    //        if (ContainsSqlInjection(requestContent))
    //        {
    //            context.Response.StatusCode = StatusCodes.Status400BadRequest;
    //            await context.Response.WriteAsync("Request blocked: possible SQL injection attempt.");
    //            return;
    //        }

    //        // Check request body (only if small and readable, not streaming)
    //        if (context.Request.ContentLength > 0 &&
    //            context.Request.ContentLength < 1024 * 100 && // avoid big payloads
    //            (context.Request.Method == HttpMethods.Post ||
    //             context.Request.Method == HttpMethods.Put ||
    //             context.Request.Method == HttpMethods.Patch))
    //        {
    //            context.Request.EnableBuffering();
    //            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
    //            var body = await reader.ReadToEndAsync();
    //            context.Request.Body.Position = 0;

    //            if (ContainsSqlInjection(body))
    //            {
    //                context.Response.StatusCode = StatusCodes.Status400BadRequest;
    //                await context.Response.WriteAsync("Request blocked: possible SQL injection attempt.");
    //                return;
    //            }
    //        }

    //        await _next(context);
    //    }

    //    private bool ContainsSqlInjection(string input)
    //    {
    //        if (string.IsNullOrWhiteSpace(input)) return false;

    //        foreach (var keyword in BlockedKeywords)
    //        {
    //            if (Regex.IsMatch(input, @"\b" + Regex.Escape(keyword) + @"\b", RegexOptions.IgnoreCase))
    //            {
    //                return true;
    //            }
    //        }

    //        return false;
    //    }
    //}
}
