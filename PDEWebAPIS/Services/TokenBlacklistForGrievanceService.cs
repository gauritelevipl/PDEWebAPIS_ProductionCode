
using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PDEWebAPIS.Data;
using PDEWebAPIS.Repository;


namespace PDEWebAPIS.Services
{
    public class TokenBlacklistForGrievanceService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TokenBlacklistForGrievanceService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void AddToBlacklist(string token, DateTime expiration)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContextR>();
                var blacklistedToken = new BlacklistTokenForGrievance
                {
                    Token = token,
                    ExpirationTime = expiration
                };
                dbContext.blacklistTokensForGrievance.Add(blacklistedToken);
                dbContext.SaveChanges();

            }
        }
        public bool IsTokenBlacklisted(string token)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                return dbContext.blacklistTokensForGrievance.Any(t => t.Token == token);
            }
        }
    }
}