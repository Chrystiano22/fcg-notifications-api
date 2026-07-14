using Fcg.Notifications.Application.DependencyInjection;
using Fcg.Notifications.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapGet("/health", () => Results.Ok(new
{
    service = "NotificationsAPI",
    status = "Healthy",
    checkedAt = DateTimeOffset.UtcNow
}))
.WithName("GetHealth")
.WithOpenApi();

app.MapGet("/info", () => Results.Ok(new
{
    service = "NotificationsAPI",
    responsibility = "UserCreatedEvent and PaymentProcessedEvent notification simulation logs"
}))
.WithName("GetInfo")
.WithOpenApi();

app.MapControllers();

app.Run();

public partial class Program
{
}
