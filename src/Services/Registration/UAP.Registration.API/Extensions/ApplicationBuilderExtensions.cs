
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

namespace UAP.Registration.API.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the development environment pipeline
    /// </summary>
    public static IApplicationBuilder UseDevelopmentConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Enable Swagger for Development and Docker environments
        if (env.IsDevelopment() || env.EnvironmentName == "Docker")
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "UAP Registration API v1");
                options.RoutePrefix = string.Empty; // Serve at root
                options.DisplayRequestDuration();
                options.EnablePersistAuthorization();
            });

            app.UseCors("AllowAll");
        }
        else
        {
            app.UseCors("ProductionCors");
        }

        return app;
    }
    
    /// <summary>
    /// Configures security headers for the application
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            // Add security headers
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            
            await next();
        });

        return app;
    }
    
    /// <summary>
    /// Configures structured logging with Serilog
    /// </summary>
    public static IApplicationBuilder UseStructuredLogging(this IApplicationBuilder app)
    {
        // Serilog is already configured in Program.cs. This ensures it's used
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex != null || httpContext.Response.StatusCode > 499)
                    return Serilog.Events.LogEventLevel.Error;
                if (elapsed > 1000)
                    return Serilog.Events.LogEventLevel.Warning;
                return Serilog.Events.LogEventLevel.Information;
            };
        });

        return app;
    }
    
    /// <summary>
    /// Configures health check endpoints
    /// </summary>
    public static IApplicationBuilder UseHealthCheckEndpoints(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration
                    }),
                    totalDuration = report.TotalDuration
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        });

        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live")
        });

        return app;
    }
}