namespace Fcg.Notifications.Application.Notifications;

public sealed record NotificationResult(
    bool Sent,
    string NotificationType,
    string Message);
