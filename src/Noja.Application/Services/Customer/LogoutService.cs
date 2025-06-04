using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Noja.Application.Services.Auth
{
    public class LogoutService : ILogoutService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _tokenExpiryBuffer;
        private readonly ILogger<LogoutService> _logger;

        public LogoutService(IMemoryCache cache, ILogger<LogoutService> logger)
        {
            _cache = cache;
            _tokenExpiryBuffer = TimeSpan.FromHours(24);
            _logger = logger;
        }

        public async Task<bool> LogoutAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                // _logger.LogWarning("Attempted to logout with empty token");
                return await Task.FromResult(false);
            }

            var cacheKey = GetCacheKey(token);
            _cache.Set(cacheKey, true, _tokenExpiryBuffer);
            _logger.LogInformation($"Token blacklisted with {cacheKey}");
            return await Task.FromResult(true);

        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                // _logger.LogWarning("Attempted to check blacklist with empty token");
                return await Task.FromResult(false);
            }
            
            var cacheKey = GetCacheKey(token);
            var isBlacklisted = _cache.TryGetValue(cacheKey, out _);
            // _logger.LogInformation($"Token check: {cacheKey}, Blacklisted: {isBlacklisted}");
            
            return await Task.FromResult(isBlacklisted);

        }


        private string GetCacheKey(string token)
        {
            return $"BlacklistedToken:{token}";
        }
    }
}