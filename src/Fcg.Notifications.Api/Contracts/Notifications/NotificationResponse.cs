using Fcg.Notifications.Application.Notifications;

namespace Fcg.Notifications.Api.Contracts.Notifications;

public sealed class NotificationResponse
{
    public bool Enviada { get; init; }

    public string Tipo { get; init; } = string.Empty;

    public string Mensagem { get; init; } = string.Empty;

    public static NotificationResponse FromResult(NotificationResult result)
    {
        return new NotificationResponse
        {
            Enviada = result.Sent,
            Tipo = result.NotificationType,
            Mensagem = result.Message
        };
    }
}
