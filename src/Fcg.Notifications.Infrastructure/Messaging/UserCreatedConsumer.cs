using Fcg.Notifications.Application.Events;
using Fcg.Notifications.Application.Notifications.NotifyUserCreated;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Fcg.Notifications.Infrastructure.Messaging;

public sealed class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly INotifyUserCreatedUseCase _useCase;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(
        INotifyUserCreatedUseCase useCase,
        ILogger<UserCreatedConsumer> logger)
    {
        _useCase = useCase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var result = await _useCase.ExecuteAsync(context.Message, context.CancellationToken);

        _logger.LogInformation(
            "UserCreatedEvent consumed for user {UserId}. Notification sent: {Sent}.",
            context.Message.UserId,
            result.Sent);
    }
}
