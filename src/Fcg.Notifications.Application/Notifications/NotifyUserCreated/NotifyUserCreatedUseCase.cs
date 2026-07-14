using Fcg.Notifications.Application.Events;

namespace Fcg.Notifications.Application.Notifications.NotifyUserCreated;

public sealed class NotifyUserCreatedUseCase : INotifyUserCreatedUseCase
{
    private readonly INotificationSender _notificationSender;

    public NotifyUserCreatedUseCase(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public async Task<NotificationResult> ExecuteAsync(
        UserCreatedEvent userCreatedEvent,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userCreatedEvent);

        await _notificationSender.SendWelcomeEmailAsync(userCreatedEvent, cancellationToken);

        return new NotificationResult(
            true,
            "WelcomeEmail",
            $"Welcome email logged for user {userCreatedEvent.UserId}.");
    }
}
