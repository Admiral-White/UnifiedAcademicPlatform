using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace UAP.Authentication.API.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public RedisHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var redisConnection = _configuration.GetSection("Redis:ConnectionString").Value;
        
        if (string.IsNullOrEmpty(redisConnection))
        {
            return HealthCheckResult.Healthy("Redis is not configured, using in-memory cache");
        }

        try
        {
            using var connection = await ConnectionMultiplexer.ConnectAsync(redisConnection);
            var database = connection.GetDatabase();
            
            var testKey = "health_check_test";
            await database.StringSetAsync(testKey, "test", TimeSpan.FromSeconds(10));
            var value = await database.StringGetAsync(testKey);
            
            if (value == "test")
            {
                return HealthCheckResult.Healthy("Redis is responding correctly");
            }
            else
            {
                return HealthCheckResult.Unhealthy("Redis returned unexpected value");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis connection failed", ex);
        }
    }
}