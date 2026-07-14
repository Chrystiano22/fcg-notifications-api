using Fcg.Notifications.Application.Events;
using Fcg.Notifications.Application.Notifications;
using Microsoft.Extensions.Logging;

namespace Fcg.Notifications.Infrastructure.Notifications;

public sealed class LoggingNotificationSender : INotificationSender
{
    private readonly NotificationOptions _options;
    private readonly ILogger<LoggingNotificationSender> _logger;

    public LoggingNotificationSender(
        NotificationOptions options,
        ILogger<LoggingNotificationSender> logger)
    {
        _options = options;
        _logger = logger;
    }

    public Task SendWelcomeEmailAsync(
        UserCreatedEvent userCreatedEvent,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Welcome email simulated from {FromEmail} to {Email} for user {UserId} ({Name}).",
            _options.FromEmail,
            userCreatedEvent.Email,
            userCreatedEvent.UserId,
            userCreatedEvent.Name);

        return Task.CompletedTask;
    }

    public Task SendPurchaseConfirmationAsync(
        PaymentProcessedEvent paymentProcessedEvent,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Purchase confirmation email simulated from {FromEmail} for order {OrderId}, user {UserId}, game {GameId}, price {Price}.",
            _options.FromEmail,
            paymentProcessedEvent.OrderId,
            paymentProcessedEvent.UserId,
            paymentProcessedEvent.GameId,
            paymentProcessedEvent.Price);

        return Task.CompletedTask;
    }
}
