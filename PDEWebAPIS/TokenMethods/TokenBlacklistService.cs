using System.Collections.Concurrent;

namespace PDEWebAPIS.TokenMethods
{
    public class TokenBlacklistService
    {
        private static readonly ConcurrentDictionary<string, bool> BlacklistedTokens = new();

        public void BlacklistToken(string token)
        {
            BlacklistedTokens[token] = true;
        }

        public bool IsTokenBlacklisted(string token)
        {
            return BlacklistedTokens.ContainsKey(token);
        }
    }
}
