using Fcg.Notifications.Application.Events;
using Fcg.Notifications.Application.Notifications.NotifyPaymentProcessed;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Fcg.Notifications.Infrastructure.Messaging;

public sealed class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly INotifyPaymentProcessedUseCase _useCase;
    private readonly ILogger<PaymentProcessedConsumer> _logger;

    public PaymentProcessedConsumer(
        INotifyPaymentProcessedUseCase useCase,
        ILogger<PaymentProcessedConsumer> logger)
    {
        _useCase = useCase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var result = await _useCase.ExecuteAsync(context.Message, context.CancellationToken);

        _logger.LogInformation(
            "PaymentProcessedEvent consumed for order {OrderId}. Notification sent: {Sent}.",
            context.Message.OrderId,
            result.Sent);
    }
}
