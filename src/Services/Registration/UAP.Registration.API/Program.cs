using Serilog;
using UAP.Registration.API.Extensions;
using UAP.Registration.API.Middleware;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    
    builder.Services.AddCustomSwagger(builder.Configuration);
    builder.Services.AddCustomCors(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddCustomHealthChecks(builder.Configuration);
    builder.Services.AddEntityFramework(builder.Configuration);
    builder.Services.AddMessageBroker(builder.Configuration);
    builder.Services.AddApplicationServices();

    var app = builder.Build();

    app.UseDevelopmentConfiguration(app.Environment);
    app.UseSecurityHeaders();
    app.UseStructuredLogging();
    app.UseGlobalExceptionHandler();
    
    app.UseHttpsRedirection();
    app.UseRouting();
    
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.UseHealthCheckEndpoints();
    app.MapControllers();

    Log.Information("UAP Registration Service started successfully");

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
