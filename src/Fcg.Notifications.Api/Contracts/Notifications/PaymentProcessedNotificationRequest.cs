namespace Fcg.Notifications.Api.Contracts.Notifications;

public sealed class PaymentProcessedNotificationRequest
{
    public Guid PedidoId { get; init; }

    public Guid UsuarioId { get; init; }

    public Guid JogoId { get; init; }

    public decimal Preco { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTime ProcessadoEm { get; init; }
}
