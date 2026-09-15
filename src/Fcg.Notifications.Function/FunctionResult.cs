namespace Fcg.Notifications.Function;

public sealed record FunctionResult(
    string EventType,
    bool Sent,
    string Message)
{
    public static FunctionResult FromNotificationResult(
        string eventType,
        bool sent,
        string message)
    {
        return new FunctionResult(eventType, sent, message);
    }
}
