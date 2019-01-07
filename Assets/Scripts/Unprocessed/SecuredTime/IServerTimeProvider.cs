using System;

public interface IServerTimeProvider
{
    void GetServerTime(Action<bool, long> onComplete);
}