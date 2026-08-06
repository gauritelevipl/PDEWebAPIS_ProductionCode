using PDEWebAPIS.TokenMethods;
using PDEWebAPIS.Services;

namespace PDEWebAPIS.ResponseHeader
{
    public class TokenBlockListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenBlacklistForGrievanceService _tokenBlacklistForGrievanceService;

        public TokenBlockListMiddleware(RequestDelegate next, TokenBlacklistForGrievanceService tokenBlacklistForGrievanceService)
        {
            _next = next;
            _tokenBlacklistForGrievanceService = tokenBlacklistForGrievanceService;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token) && _tokenBlacklistForGrievanceService.IsTokenBlacklisted(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token is blacklisted. Please login again.");
                return;
            }

            await _next(context);
        }
    }

}