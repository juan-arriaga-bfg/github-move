using System;

public static class DateTimeExtension
{
    public static long ConvertToUnixTime(this DateTime datetime)
    {
        var sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long) (datetime - sTime).TotalSeconds;
    }
    
    public static long ConvertToUnixTimeMilliseconds(this DateTime datetime)
    {
        var sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long) (datetime - sTime).TotalMilliseconds;
    }
    
    public static DateTime UnixTimeToDateTime(long unixtime)
    {
        var sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return sTime.AddSeconds(unixtime);
    }

    public static int CountOfStepsPassedWhenAppWasInBackground(long then, int delay, out DateTime now)
    {
        if (then == 0)
        {
            now = DateTime.UtcNow;
            return 0;
        }
        
        return CountOfStepsPassedWhenAppWasInBackground(UnixTimeToDateTime(then), delay, out now);
    }
    
    public static int CountOfStepsPassedWhenAppWasInBackground(DateTime then, int delay, out DateTime now)
    {
        var elapsedTime = DateTime.UtcNow - then;
        var count = (int) elapsedTime.TotalSeconds / delay;
        var remainder = (int) elapsedTime.TotalSeconds % delay;
        
        now = DateTime.UtcNow.AddSeconds(-remainder);
        
        return count;
    }
}