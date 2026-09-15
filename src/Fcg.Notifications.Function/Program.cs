using System.Text.Json;
using Fcg.Notifications.Function;

if (args.Length < 2 || args[0] != "--event")
{
    Console.WriteLine("Usage: dotnet run --project src/Fcg.Notifications.Function -- --event <payload.json>");
    return;
}

var json = await File.ReadAllTextAsync(args[1]);
var queueEvent = JsonSerializer.Deserialize<ServerlessQueueEvent>(
    json,
    new JsonSerializerOptions(JsonSerializerDefaults.Web));

if (queueEvent is null)
{
    throw new InvalidOperationException("Event payload is invalid.");
}

var function = new NotificationFunction();
var results = await function.HandleAsync(queueEvent, new LocalLambdaContext());

Console.WriteLine(JsonSerializer.Serialize(
    results,
    new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    }));
