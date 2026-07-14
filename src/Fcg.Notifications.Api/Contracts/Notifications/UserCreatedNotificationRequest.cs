namespace Fcg.Notifications.Api.Contracts.Notifications;

public sealed class UserCreatedNotificationRequest
{
    public Guid UsuarioId { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public DateTime CriadoEm { get; init; }
}
