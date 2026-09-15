using System.Text;
using System.Text.Json;
using Amazon.Lambda.Core;
using Fcg.Notifications.Application.DependencyInjection;
using Fcg.Notifications.Application.Events;
using Fcg.Notifications.Application.Notifications.NotifyPaymentProcessed;
using Fcg.Notifications.Application.Notifications.NotifyUserCreated;
using Fcg.Notifications.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Fcg.Notifications.Function;

public sealed class NotificationFunction
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationFunction> _logger;

    public NotificationFunction()
        : this(CreateServiceProvider())
    {
    }

    public NotificationFunction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger<NotificationFunction>();
    }

    public async Task<IReadOnlyCollection<FunctionResult>> HandleAsync(
        ServerlessQueueEvent queueEvent,
        ILambdaContext context)
    {
        ArgumentNullException.ThrowIfNull(queueEvent);

        var results = new List<FunctionResult>();

        if (queueEvent.RmqMessagesByQueue.Count > 0)
        {
            foreach (var queue in queueEvent.RmqMessagesByQueue)
            {
                foreach (var message in queue.Value)
                {
                    results.Add(await ProcessRabbitMqMessageAsync(
                        queue.Key,
                        message,
                        CancellationToken.None));
                }
            }

            return results;
        }

        if (!string.IsNullOrWhiteSpace(queueEvent.EventType) && queueEvent.Payload is not null)
        {
            results.Add(await ProcessDirectPayloadAsync(
                queueEvent.EventType,
                queueEvent.Payload.Value,
                CancellationToken.None));
        }

        return results;
    }

    private async Task<FunctionResult> ProcessRabbitMqMessageAsync(
        string queueName,
        RabbitMqMessage message,
        CancellationToken cancellationToken)
    {
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(message.Data));
        var eventType = ResolveEventType(queueName, message.BasicProperties?.Type);

        return await ProcessJsonAsync(eventType, json, cancellationToken);
    }

    private async Task<FunctionResult> ProcessDirectPayloadAsync(
        string eventType,
        JsonElement payload,
        CancellationToken cancellationToken)
    {
        return await ProcessJsonAsync(eventType, payload.GetRawText(), cancellationToken);
    }

    private async Task<FunctionResult> ProcessJsonAsync(
        string eventType,
        string json,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        if (IsUserCreated(eventType))
        {
            var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedEvent>(json, JsonOptions)
                ?? throw new InvalidOperationException("UserCreatedEvent payload is invalid.");

            var useCase = scope.ServiceProvider.GetRequiredService<INotifyUserCreatedUseCase>();
            var result = await useCase.ExecuteAsync(userCreatedEvent, cancellationToken);

            _logger.LogInformation(
                "Serverless notification processed UserCreatedEvent for user {UserId}. Sent: {Sent}.",
                userCreatedEvent.UserId,
                result.Sent);

            return FunctionResult.FromNotificationResult("UserCreatedEvent", result.Sent, result.Message);
        }

        if (IsPaymentProcessed(eventType))
        {
            var paymentProcessedEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(json, JsonOptions)
                ?? throw new InvalidOperationException("PaymentProcessedEvent payload is invalid.");

            var useCase = scope.ServiceProvider.GetRequiredService<INotifyPaymentProcessedUseCase>();
            var result = await useCase.ExecuteAsync(paymentProcessedEvent, cancellationToken);

            _logger.LogInformation(
                "Serverless notification processed PaymentProcessedEvent for order {OrderId}. Sent: {Sent}.",
                paymentProcessedEvent.OrderId,
                result.Sent);

            return FunctionResult.FromNotificationResult("PaymentProcessedEvent", result.Sent, result.Message);
        }

        throw new InvalidOperationException($"Event type '{eventType}' is not supported.");
    }

    private static string ResolveEventType(string queueName, string? messageType)
    {
        if (!string.IsNullOrWhiteSpace(messageType))
        {
            return messageType;
        }

        return queueName;
    }

    private static bool IsUserCreated(string eventType)
    {
        return eventType.Contains("user-created", StringComparison.OrdinalIgnoreCase)
            || eventType.Contains("UserCreated", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPaymentProcessed(string eventType)
    {
        return eventType.Contains("payment-processed", StringComparison.OrdinalIgnoreCase)
            || eventType.Contains("PaymentProcessed", StringComparison.OrdinalIgnoreCase);
    }

    private static IServiceProvider CreateServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
        services.AddSingleton<IConfiguration>(configuration);
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services.BuildServiceProvider();
    }
}
