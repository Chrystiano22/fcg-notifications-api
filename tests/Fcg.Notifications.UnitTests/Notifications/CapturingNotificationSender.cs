using Fcg.Notifications.Application.Events;
using Fcg.Notifications.Application.Notifications;

namespace Fcg.Notifications.UnitTests.Notifications;

internal sealed class CapturingNotificationSender : INotificationSender
{
    public UserCreatedEvent? WelcomeEmailEvent { get; private set; }

    public PaymentProcessedEvent? PurchaseConfirmationEvent { get; private set; }

    public Task SendWelcomeEmailAsync(
        UserCreatedEvent userCreatedEvent,
        CancellationToken cancellationToken = default)
    {
        WelcomeEmailEvent = userCreatedEvent;

        return Task.CompletedTask;
    }

    public Task SendPurchaseConfirmationAsync(
        PaymentProcessedEvent paymentProcessedEvent,
        CancellationToken cancellationToken = default)
    {
        PurchaseConfirmationEvent = paymentProcessedEvent;

        return Task.CompletedTask;
    }
}
