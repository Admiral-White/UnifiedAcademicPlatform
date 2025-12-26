using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UAP.Shared.Infrastructure.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip API key validation for health checks and swagger
        var path = context.Request.Path.Value?.ToLower();
        if (path != null && (path.Contains("/health") || path.Contains("/swagger")))
        {
            await _next(context);
            return;
        }

        var headerName = _configuration["ApiKey:HeaderName"] ?? "X-API-Key";
        
        if (!context.Request.Headers.TryGetValue(headerName, out var extractedApiKey))
        {
            _logger.LogWarning("API Key missing from request to {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "API Key is missing" }));
            return;
        }

        var validKeys = _configuration.GetSection("ApiKey:ValidKeys").Get<Dictionary<string, string>>();
        
        if (validKeys == null || !validKeys.Values.Contains(extractedApiKey.ToString()))
        {
            _logger.LogWarning("Invalid API Key attempted for {Path}", context.Request.Path);
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Invalid API Key" }));
            return;
        }

        await _next(context);
    }
}
