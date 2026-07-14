using Fcg.Notifications.Application.Events;
using Fcg.Notifications.Application.Notifications;
using Fcg.Notifications.Application.Notifications.NotifyUserCreated;

namespace Fcg.Notifications.UnitTests.Notifications;

public sealed class NotifyUserCreatedUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenUserCreated_SendsWelcomeEmail()
    {
        var userCreatedEvent = new UserCreatedEvent(
            Guid.NewGuid(),
            "Chrys",
            "chrys@fcg.local",
            DateTime.UtcNow);
        var sender = new CapturingNotificationSender();
        var useCase = new NotifyUserCreatedUseCase(sender);

        var result = await useCase.ExecuteAsync(userCreatedEvent);

        Assert.True(result.Sent);
        Assert.Equal("WelcomeEmail", result.NotificationType);
        Assert.Equal(userCreatedEvent, sender.WelcomeEmailEvent);
        Assert.Null(sender.PurchaseConfirmationEvent);
    }
}
