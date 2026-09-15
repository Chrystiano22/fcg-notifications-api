using Amazon.Lambda.Core;

namespace Fcg.Notifications.Function;

public sealed class LocalLambdaContext : ILambdaContext
{
    public string AwsRequestId { get; } = Guid.NewGuid().ToString();

    public IClientContext ClientContext => throw new NotSupportedException();

    public string FunctionName => "fcg-notifications-function-local";

    public string FunctionVersion => "local";

    public ICognitoIdentity Identity => throw new NotSupportedException();

    public string InvokedFunctionArn => "local";

    public ILambdaLogger Logger { get; } = new LocalLambdaLogger();

    public string LogGroupName => "local";

    public string LogStreamName => "local";

    public int MemoryLimitInMB => 256;

    public TimeSpan RemainingTime => TimeSpan.FromMinutes(5);

    public CancellationToken CancellationToken => CancellationToken.None;
}

public sealed class LocalLambdaLogger : ILambdaLogger
{
    public void Log(string message)
    {
        Console.Write(message);
    }

    public void LogLine(string message)
    {
        Console.WriteLine(message);
    }
}
