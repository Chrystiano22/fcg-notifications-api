using System.Net;
using System.Net.Http.Json;
using Fcg.Notifications.Api.Contracts.Notifications;
using Fcg.Notifications.Application.Notifications;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fcg.Notifications.UnitTests.Notifications;

public sealed class NotificationsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public NotificationsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task NotifyUserCreated_WhenRequestIsValid_ReturnsSentWelcomeEmail()
    {
        var request = new UserCreatedNotificationRequest
        {
            UsuarioId = Guid.NewGuid(),
            Nome = "Chrys",
            Email = "chrys@fcg.local",
            CriadoEm = DateTime.UtcNow
        };
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/notificacoes/usuario-criado", request);
        var notification = await response.Content.ReadFromJsonAsync<NotificationResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(notification);
        Assert.True(notification.Enviada);
        Assert.Equal("WelcomeEmail", notification.Tipo);
    }

    [Fact]
    public async Task NotifyPaymentProcessed_WhenPaymentIsApproved_ReturnsSentPurchaseConfirmation()
    {
        var request = new PaymentProcessedNotificationRequest
        {
            PedidoId = Guid.NewGuid(),
            UsuarioId = Guid.NewGuid(),
            JogoId = Guid.NewGuid(),
            Preco = 49.90m,
            Status = PaymentStatus.Approved,
            ProcessadoEm = DateTime.UtcNow
        };
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/notificacoes/pagamento-processado", request);
        var notification = await response.Content.ReadFromJsonAsync<NotificationResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(notification);
        Assert.True(notification.Enviada);
        Assert.Equal("PurchaseConfirmationEmail", notification.Tipo);
    }
}
