using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using UAP.StudentAcademic.Application.Commands;
using UAP.StudentAcademic.Application.EventHandlers;
using UAP.StudentAcademic.Application.Mappings;
using UAP.StudentAcademic.Domain.Events;
using UAP.StudentAcademic.Domain.Interfaces;
using UAP.StudentAcademic.Infrastructure.Data;
using UAP.StudentAcademic.Infrastructure.Repositories;
using UAP.StudentAcademic.Infrastructure.Services;
using IAuthenticationService = Microsoft.AspNetCore.Authentication.IAuthenticationService;

namespace UAP.StudentAcademic.API.Extensions;

public static class ServiceCollectionExtensions
{
        public static IServiceCollection AddEntityFramework(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<StudentAcademicDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(StudentAcademicDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
        });

        return services;
    }

    public static IServiceCollection AddMessageBroker(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();
            
            // Add integration event consumers
            busConfig.AddConsumer<UserCreatedIntegrationEventHandler>();
            busConfig.AddConsumer<CourseCreatedIntegrationEventHandler>();
            
            // Add domain event handlers (handled by MediatR)
            services.AddScoped<INotificationHandler<StudentCreatedDomainEvent>, StudentCreatedDomainEventHandler>();
            services.AddScoped<INotificationHandler<CGPAUpdatedDomainEvent>, CGPAUpdatedDomainEventHandler>();
            services.AddScoped<INotificationHandler<GradeSubmittedDomainEvent>, GradeSubmittedDomainEventHandler>();
            services.AddScoped<INotificationHandler<StudentStatusChangedDomainEvent>, StudentStatusChangedDomainEventHandler>();
            
            busConfig.UsingRabbitMq((context, config) =>
            {
                var rabbitMqHost = configuration.GetSection("RabbitMQ:Host").Value;
                var rabbitMqUser = configuration.GetSection("RabbitMQ:Username").Value ?? "guest";
                var rabbitMqPass = configuration.GetSection("RabbitMQ:Password").Value ?? "guest";
                
                config.Host(rabbitMqHost, "/", h =>
                {
                    h.Username(rabbitMqUser);
                    h.Password(rabbitMqPass);
                });
                
                // Configure specific endpoints
                config.ReceiveEndpoint("user-created-student-academic", e =>
                {
                    e.ConfigureConsumer<UserCreatedIntegrationEventHandler>(context);
                    e.ConcurrentMessageLimit = 5;
                });
                
                config.ReceiveEndpoint("course-created-student-academic", e =>
                {
                    e.ConfigureConsumer<CourseCreatedIntegrationEventHandler>(context);
                    e.ConcurrentMessageLimit = 5;
                });
                
                config.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(CreateStudentCommand).Assembly));
        
        // Add repositories
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ICourseGradeRepository, CourseGradeRepository>();
        services.AddScoped<IAcademicSemesterRepository, AcademicSemesterRepository>();
        
        // Add UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }

    public static IServiceCollection AddHttpClientServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5001");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddPolicyHandler(HttpClientPolicies.GetRetryPolicy())
        .AddPolicyHandler(HttpClientPolicies.GetCircuitBreakerPolicy());

        services.AddHttpClient<ICourseCatalogService, CourseCatalogService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5002");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddPolicyHandler(HttpClientPolicies.GetRetryPolicy())
        .AddPolicyHandler(HttpClientPolicies.GetCircuitBreakerPolicy());

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ICourseCatalogService, CourseCatalogService>();

        return services;
    }

    public static IServiceCollection AddAutoMapperProfiles(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(StudentAcademicProfile));
        return services;
    }
}

public static class HttpClientPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30));
    }
}