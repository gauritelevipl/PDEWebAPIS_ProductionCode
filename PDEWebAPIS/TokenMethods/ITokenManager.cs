using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using PDEWebAPIS.TokenMethods;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secretKey;
    private readonly TokenBlacklistService _blacklistService;

    public JwtMiddleware(RequestDelegate next, string secretKey, TokenBlacklistService blacklistService)
    {
        _next = next;
        _secretKey = secretKey;
        _blacklistService = blacklistService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token != null)
        {
            if (!_blacklistService.IsTokenBlacklisted(token) && ValidateJwtToken(token))
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                context.User = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("401 || Unauthorized: Invalid or expired token.");
                return;
            }
        }
        await _next(context);
    }

    private bool ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
