using Fcg.Notifications.Application.Events;

namespace Fcg.Notifications.Application.Notifications.NotifyUserCreated;

public interface INotifyUserCreatedUseCase
{
    Task<NotificationResult> ExecuteAsync(
        UserCreatedEvent userCreatedEvent,
        CancellationToken cancellationToken = default);
}
