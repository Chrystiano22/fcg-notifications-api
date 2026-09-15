using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fcg.Notifications.Function;

public sealed class ServerlessQueueEvent
{
    [JsonPropertyName("rmqMessagesByQueue")]
    public Dictionary<string, IReadOnlyCollection<RabbitMqMessage>> RmqMessagesByQueue { get; init; } = [];

    [JsonPropertyName("eventType")]
    public string? EventType { get; init; }

    [JsonPropertyName("payload")]
    public JsonElement? Payload { get; init; }
}

public sealed class RabbitMqMessage
{
    [JsonPropertyName("data")]
    public string Data { get; init; } = string.Empty;

    [JsonPropertyName("basicProperties")]
    public RabbitMqBasicProperties? BasicProperties { get; init; }
}

public sealed class RabbitMqBasicProperties
{
    [JsonPropertyName("type")]
    public string? Type { get; init; }
}
