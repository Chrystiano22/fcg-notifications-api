using Fcg.Notifications.Application.Notifications;
using Fcg.Notifications.Application.Events;
using Fcg.Notifications.Infrastructure.Messaging;
using Fcg.Notifications.Infrastructure.Notifications;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Notifications.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = new NotificationOptions
        {
            FromEmail = configuration["Notifications:FromEmail"] ?? "noreply@fcg.local"
        };

        services.AddSingleton(options);
        services.AddScoped<INotificationSender, LoggingNotificationSender>();
        services.AddMessaging(configuration);

        return services;
    }

    private static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitMqOptions = RabbitMqOptions.FromConfiguration(configuration);

        if (!rabbitMqOptions.Enabled)
        {
            return services;
        }

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<UserCreatedConsumer>();
            busConfigurator.AddConsumer<PaymentProcessedConsumer>();

            busConfigurator.UsingRabbitMq((context, rabbitMqConfigurator) =>
            {
                rabbitMqConfigurator.UseRawJsonSerializer(
                    RawSerializerOptions.AddTransportHeaders | RawSerializerOptions.CopyHeaders,
                    isDefault: true);

                rabbitMqConfigurator.Host(
                    rabbitMqOptions.Host,
                    rabbitMqOptions.VirtualHost,
                    hostConfigurator =>
                    {
                        hostConfigurator.Username(rabbitMqOptions.Username);
                        hostConfigurator.Password(rabbitMqOptions.Password);
                    });

                rabbitMqConfigurator.Message<UserCreatedEvent>(messageConfigurator =>
                    messageConfigurator.SetEntityName(EventNames.UserCreated));
                rabbitMqConfigurator.Message<PaymentProcessedEvent>(messageConfigurator =>
                    messageConfigurator.SetEntityName(EventNames.PaymentProcessed));

                rabbitMqConfigurator.ReceiveEndpoint("notifications-user-created", endpointConfigurator =>
                {
                    endpointConfigurator.UseRawJsonDeserializer(RawSerializerOptions.AnyMessageType, isDefault: true);
                    endpointConfigurator.UseMessageRetry(retryConfigurator =>
                        retryConfigurator.Interval(3, TimeSpan.FromSeconds(5)));
                    endpointConfigurator.ConfigureConsumer<UserCreatedConsumer>(context);
                });

                rabbitMqConfigurator.ReceiveEndpoint("notifications-payment-processed", endpointConfigurator =>
                {
                    endpointConfigurator.UseRawJsonDeserializer(RawSerializerOptions.AnyMessageType, isDefault: true);
                    endpointConfigurator.UseMessageRetry(retryConfigurator =>
                        retryConfigurator.Interval(3, TimeSpan.FromSeconds(5)));
                    endpointConfigurator.ConfigureConsumer<PaymentProcessedConsumer>(context);
                });
            });
        });

        return services;
    }
}
