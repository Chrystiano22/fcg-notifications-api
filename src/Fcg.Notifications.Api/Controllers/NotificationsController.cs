using Fcg.Notifications.Api.Contracts.Notifications;
using Fcg.Notifications.Application.Events;
using Fcg.Notifications.Application.Notifications.NotifyPaymentProcessed;
using Fcg.Notifications.Application.Notifications.NotifyUserCreated;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Notifications.Api.Controllers;

[ApiController]
[Route("notificacoes")]
public sealed class NotificationsController : ControllerBase
{
    [HttpPost("usuario-criado")]
    [ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationResponse>> NotifyUserCreated(
        [FromBody] UserCreatedNotificationRequest request,
        [FromServices] INotifyUserCreatedUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new UserCreatedEvent(
                request.UsuarioId,
                request.Nome,
                request.Email,
                request.CriadoEm),
            cancellationToken);

        return Ok(NotificationResponse.FromResult(result));
    }

    [HttpPost("pagamento-processado")]
    [ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationResponse>> NotifyPaymentProcessed(
        [FromBody] PaymentProcessedNotificationRequest request,
        [FromServices] INotifyPaymentProcessedUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new PaymentProcessedEvent(
                request.PedidoId,
                request.UsuarioId,
                request.JogoId,
                request.Preco,
                request.Status,
                request.ProcessadoEm),
            cancellationToken);

        return Ok(NotificationResponse.FromResult(result));
    }
}
