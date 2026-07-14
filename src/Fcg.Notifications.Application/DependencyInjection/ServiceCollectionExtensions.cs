using Fcg.Notifications.Application.Notifications.NotifyPaymentProcessed;
using Fcg.Notifications.Application.Notifications.NotifyUserCreated;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Notifications.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<INotifyUserCreatedUseCase, NotifyUserCreatedUseCase>();
        services.AddScoped<INotifyPaymentProcessedUseCase, NotifyPaymentProcessedUseCase>();

        return services;
    }
}
