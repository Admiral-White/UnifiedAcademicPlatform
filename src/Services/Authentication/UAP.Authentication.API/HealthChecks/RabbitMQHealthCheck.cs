using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace UAP.Authentication.API.HealthChecks;

/// <summary>
/// Custom health check for RabbitMQ connectivity
/// </summary>
public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public RabbitMQHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        var host = _configuration.GetSection("RabbitMQ:Host").Value;
        var username = _configuration.GetSection("RabbitMQ:Username").Value ?? "guest";
        var password = _configuration.GetSection("RabbitMQ:Password").Value ?? "guest";

        if (string.IsNullOrEmpty(host))
        {
            return HealthCheckResult.Healthy("RabbitMQ is not configured");
        }

        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = host,
                UserName = username,
                Password = password,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            
            return HealthCheckResult.Healthy("RabbitMQ is responding correctly");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ connection failed", ex);
        }
    }
}