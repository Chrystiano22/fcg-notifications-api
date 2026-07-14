using Fcg.Notifications.Application.Events;

namespace Fcg.Notifications.Application.Notifications.NotifyPaymentProcessed;

public interface INotifyPaymentProcessedUseCase
{
    Task<NotificationResult> ExecuteAsync(
        PaymentProcessedEvent paymentProcessedEvent,
        CancellationToken cancellationToken = default);
}
