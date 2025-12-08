using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UAP.CourseCatalog.Application.Commands;
using UAP.CourseCatalog.Application.EventHandlers;
using UAP.CourseCatalog.Domain.Events;
using UAP.CourseCatalog.Domain.Interfaces;
using UAP.CourseCatalog.Infrastructure.Data;
using UAP.CourseCatalog.Infrastructure.Repositories;
using UAP.Shared.Infrastructure.Interfaces;

namespace UAP.CourseCatalog.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(CreateCourseCommand).Assembly));
    
        // Add repositories with their interfaces
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
    
        // Add UnitOfWork with its interface
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    
        return services;
    }
    
    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        /*services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();
        
            // Add all consumers
            busConfig.AddConsumer<UserCreatedIntegrationEventHandler>();
        
            // Add domain event handlers (they will be handled by MediatR)
            services.AddScoped<INotificationHandler<CourseCreatedDomainEvent>, CourseCreatedDomainEventHandler>();
            services.AddScoped<INotificationHandler<CourseCapacityUpdatedDomainEvent>, CourseCapacityUpdatedDomainEventHandler>();
            services.AddScoped<INotificationHandler<CourseFullDomainEvent>, CourseFullDomainEventHandler>();
            services.AddScoped<INotificationHandler<DepartmentCreatedDomainEvent>, DepartmentCreatedDomainEventHandler>();
        
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
            
                config.ConfigureEndpoints(context);
            });
        });

        return services;*/
        
        
    services.AddMassTransit(busConfig =>
    {
        busConfig.SetKebabCaseEndpointNameFormatter();
        
        // Add all integration event consumers
        busConfig.AddConsumer<UserCreatedIntegrationEventHandler>();
        busConfig.AddConsumer<UserDeactivatedIntegrationEventHandler>();
        busConfig.AddConsumer<UserUpdatedIntegrationEventHandler>();
        busConfig.AddConsumer<UserRoleChangedIntegrationEventHandler>();
        
        // Add domain event handlers (they will be handled by MediatR)
        services.AddScoped<INotificationHandler<CourseCreatedDomainEvent>, CourseCreatedDomainEventHandler>();
        services.AddScoped<INotificationHandler<CourseUpdatedDomainEvent>, CourseUpdatedDomainEventHandler>();
        services.AddScoped<INotificationHandler<CourseCapacityUpdatedDomainEvent>, CourseCapacityUpdatedDomainEventHandler>();
        services.AddScoped<INotificationHandler<CourseFullDomainEvent>, CourseFullDomainEventHandler>();
        services.AddScoped<INotificationHandler<CourseDeactivatedDomainEvent>, CourseDeactivatedDomainEventHandler>();
        services.AddScoped<INotificationHandler<DepartmentCreatedDomainEvent>, DepartmentCreatedDomainEventHandler>();
        
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
            
            // Configure specific endpoints for better control
            config.ReceiveEndpoint("user-created-course-catalog", e =>
            {
                e.ConfigureConsumer<UserCreatedIntegrationEventHandler>(context);
                e.ConcurrentMessageLimit = 5;
            });
            
            config.ReceiveEndpoint("user-deactivated-course-catalog", e =>
            {
                e.ConfigureConsumer<UserDeactivatedIntegrationEventHandler>(context);
                e.ConcurrentMessageLimit = 3;
            });
            
             config.ReceiveEndpoint("user-updated-course-catalog", e =>
             {
                            e.ConfigureConsumer<UserUpdatedIntegrationEventHandler>(context);
                            //e.ConfigureConsumer<UserRoleChangedIntegrationEventHandler>(context);
                            e.ConcurrentMessageLimit = 5;
             });
             
             config.ReceiveEndpoint("user-role-changed-course-catalog", e =>
             {
                            e.ConfigureConsumer<UserRoleChangedIntegrationEventHandler>(context);
                            e.ConcurrentMessageLimit = 5;
             });
            // Course events endpoints
            config.ReceiveEndpoint("course-events", e =>
            {
                // All course-related events can use the same endpoint
                e.ConcurrentMessageLimit = 10;
            });
            
            config.ConfigureEndpoints(context);
        });
    });

    return services;
    }
}