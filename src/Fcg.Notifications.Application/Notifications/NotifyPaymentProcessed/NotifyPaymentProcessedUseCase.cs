using Fcg.Notifications.Application.Events;

namespace Fcg.Notifications.Application.Notifications.NotifyPaymentProcessed;

public sealed class NotifyPaymentProcessedUseCase : INotifyPaymentProcessedUseCase
{
    private readonly INotificationSender _notificationSender;

    public NotifyPaymentProcessedUseCase(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public async Task<NotificationResult> ExecuteAsync(
        PaymentProcessedEvent paymentProcessedEvent,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paymentProcessedEvent);

        if (!string.Equals(paymentProcessedEvent.Status, PaymentStatus.Approved, StringComparison.OrdinalIgnoreCase))
        {
            return new NotificationResult(
                false,
                "PurchaseConfirmationEmail",
                $"Payment {paymentProcessedEvent.OrderId} was not approved. Notification ignored.");
        }

        await _notificationSender.SendPurchaseConfirmationAsync(paymentProcessedEvent, cancellationToken);

        return new NotificationResult(
            true,
            "PurchaseConfirmationEmail",
            $"Purchase confirmation logged for order {paymentProcessedEvent.OrderId}.");
    }
}
