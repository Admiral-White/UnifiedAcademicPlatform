using Serilog;
using UAP.StudentAcademic.API.Extensions;
using UAP.StudentAcademic.API.Middleware;
using UAP.Shared.Infrastructure.Middleware;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();


try
{
    Log.Information("Starting UAP Student Academic Service");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog for structured logging
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();
    builder.Services.AddCors(options =>
    {
        if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Docker")
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        }
        else
        {
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(allowedOrigins ?? new[] { "https://localhost:5003" })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        }
    });

    // Add Student Academic specific services
    UAP.StudentAcademic.API.Extensions.ServiceCollectionExtensions.AddEntityFramework(builder.Services, builder.Configuration);
    UAP.StudentAcademic.API.Extensions.ServiceCollectionExtensions.AddMessageBroker(builder.Services, builder.Configuration);
    UAP.StudentAcademic.API.Extensions.ServiceCollectionExtensions.AddApplicationServices(builder.Services);
    UAP.StudentAcademic.API.Extensions.ServiceCollectionExtensions.AddHttpClientServices(builder.Services, builder.Configuration);
    UAP.StudentAcademic.API.Extensions.ServiceCollectionExtensions.AddAutoMapperProfiles(builder.Services);

    var app = builder.Build();

    // Configure the HTTP request pipeline
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "UAP StudentAcademic API v1");
        options.RoutePrefix = string.Empty;
    });
    
    app.UseHttpsRedirection();
    app.UseCors();
    app.UseRouting();
    
    // API Key Authentication for service-to-service calls
    app.UseApiKeyAuthentication();
    
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapControllers();
    app.MapHealthChecks("/health");

    Log.Information("UAP Student Academic Service started successfully");
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




// Old Impl
/*var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "UAP StudentAcademic API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();*/


