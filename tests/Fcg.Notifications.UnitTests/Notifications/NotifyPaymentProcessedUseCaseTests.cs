using Fcg.Notifications.Application.Events;
using Fcg.Notifications.Application.Notifications;
using Fcg.Notifications.Application.Notifications.NotifyPaymentProcessed;

namespace Fcg.Notifications.UnitTests.Notifications;

public sealed class NotifyPaymentProcessedUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenPaymentIsApproved_SendsPurchaseConfirmation()
    {
        var paymentProcessedEvent = CreatePaymentProcessedEvent(PaymentStatus.Approved);
        var sender = new CapturingNotificationSender();
        var useCase = new NotifyPaymentProcessedUseCase(sender);

        var result = await useCase.ExecuteAsync(paymentProcessedEvent);

        Assert.True(result.Sent);
        Assert.Equal("PurchaseConfirmationEmail", result.NotificationType);
        Assert.Equal(paymentProcessedEvent, sender.PurchaseConfirmationEvent);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPaymentIsRejected_DoesNotSendPurchaseConfirmation()
    {
        var paymentProcessedEvent = CreatePaymentProcessedEvent(PaymentStatus.Rejected);
        var sender = new CapturingNotificationSender();
        var useCase = new NotifyPaymentProcessedUseCase(sender);

        var result = await useCase.ExecuteAsync(paymentProcessedEvent);

        Assert.False(result.Sent);
        Assert.Equal("PurchaseConfirmationEmail", result.NotificationType);
        Assert.Null(sender.PurchaseConfirmationEvent);
    }

    private static PaymentProcessedEvent CreatePaymentProcessedEvent(string status)
    {
        return new PaymentProcessedEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            49.90m,
            status,
            DateTime.UtcNow);
    }
}
