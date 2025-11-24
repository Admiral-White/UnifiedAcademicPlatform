using Serilog;
using UAP.Authentication.API.Extensions;
using UAP.Authentication.API.Middleware;

// Create the logger configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/authentication-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    // Add Serilog for structured logging
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/authentication-service-.txt", rollingInterval: RollingInterval.Day));

// Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    // builder.Services.AddSwaggerGen();
    
    // Add custom services using extension methods
    builder.Services.AddCustomSwagger(builder.Configuration);
    builder.Services.AddCustomCors(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddCustomHealthChecks(builder.Configuration);
    builder.Services.AddRedisCaching(builder.Configuration);
    
    // New Service Additions
    builder.Services.AddEntityFramework(builder.Configuration);
    builder.Services.AddMessageBroker(builder.Configuration);
    builder.Services.AddApplicationServices();
// Add Health Checks old impl
    /*builder.Services.AddHealthChecks()
        .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddRedis(builder.Configuration.GetSection("Redis:ConnectionString").Value);

// Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });*/

    var app = builder.Build();

// Configure the HTTP request pipeline (old impl)
    // if (app.Environment.IsDevelopment())
    // {
    //     app.UseSwagger();
    //     app.UseSwaggerUI();
    //     app.UseCors("AllowAll");
    // }
    //
    // app.UseHttpsRedirection();
    // app.UseAuthorization();
    // app.MapControllers();
    // app.MapHealthChecks("/health");
    
    
    // Configure the HTTP request pipeline using extension methods
    app.UseDevelopmentConfiguration(app.Environment);
    app.UseSecurityHeaders();
    app.UseStructuredLogging();
    app.UseGlobalExceptionHandler();
    
    app.UseHttpsRedirection();
    app.UseRouting();
    
    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();
    
    // Health checks
    app.UseHealthCheckEndpoints();
    
    // Map controllers
    app.MapControllers();

    Log.Information("UAP Authentication Service started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}