using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UAP.Authentication.API.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public DatabaseHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            return HealthCheckResult.Unhealthy("Database connection string is not configured");
        }

        try
        {
            if (connectionString.Contains("Data Source="))
            {
                // SQLite connection
                using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                
                await connection.OpenAsync(combinedCts.Token);
                using var command = new Microsoft.Data.Sqlite.SqliteCommand("SELECT 1", connection);
                await command.ExecuteScalarAsync(combinedCts.Token);
                
                return HealthCheckResult.Healthy("SQLite database is responding correctly");
            }
            else
            {
                // SQL Server connection with timeout
                using var connection = new SqlConnection(connectionString);
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                
                await connection.OpenAsync(combinedCts.Token);
                
                // Create UAP_Auth database if it doesn't exist
                using var createDbCommand = new SqlCommand(
                    "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'UAP_Auth') CREATE DATABASE UAP_Auth", 
                    connection);
                await createDbCommand.ExecuteNonQueryAsync(combinedCts.Token);
                
                // Test the connection
                using var command = new SqlCommand("SELECT 1", connection);
                var result = await command.ExecuteScalarAsync(combinedCts.Token);
                
                return HealthCheckResult.Healthy($"SQL Server database is responding correctly. UAP_Auth database ready. (Result: {result})");
            }
        }
        catch (TaskCanceledException)
        {
            return HealthCheckResult.Unhealthy("Database connection timed out");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Database connection failed: {ex.Message}");
        }
        /*try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                return HealthCheckResult.Unhealthy("Database connection string is not configured");
            }

            // Add actual database connectivity check here
            // For now, returning healthy if connection string exists
            return HealthCheckResult.Healthy("Database is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database check failed", ex);
        }*/
    }
}