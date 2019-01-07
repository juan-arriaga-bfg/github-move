using System;

public interface ISecuredTimeManager
{
    DateTime UtcNow { get; }
    DateTime Now { get; }

    ISecuredTimeManager AddServerTimeProvider(IServerTimeProvider provider);
}