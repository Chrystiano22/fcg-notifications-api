using Fcg.Notifications.Application.Events;

namespace Fcg.Notifications.Application.Notifications;

public interface INotificationSender
{
    Task SendWelcomeEmailAsync(
        UserCreatedEvent userCreatedEvent,
        CancellationToken cancellationToken = default);

    Task SendPurchaseConfirmationAsync(
        PaymentProcessedEvent paymentProcessedEvent,
        CancellationToken cancellationToken = default);
}
