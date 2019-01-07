using System;

public static class UnixTimeHelper
{
    /// <summary>
    /// Only UTC allowed
    /// </summary>
    public static DateTime UnixTimestampToDateTime(long unixTime)
    {
        DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
        var ret = DateTime.SpecifyKind(offset.UtcDateTime, DateTimeKind.Utc);   
        return ret;
    }
    
    /// <summary>
    /// Only UTC allowed
    /// </summary>
    public static long DateTimeToUnixTimestamp(DateTime dateTime)
    {
        long unixTime = UtcDateTimeToDateTimeOffset(dateTime).ToUnixTimeSeconds();
        return unixTime;
    }
    
    private static DateTimeOffset UtcDateTimeToDateTimeOffset(DateTime dt)
    {
        // adding negative offset to a min-datetime will throw, this is a 
        // sufficient catch. Note however that a DateTime of just a few hours can still throw
        if (dt == DateTime.MinValue)
        {
            return DateTimeOffset.MinValue;
        }
        
        if (dt == DateTime.MaxValue)
        {
            return DateTimeOffset.MaxValue;
        }

        var fixedDt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        return new DateTimeOffset(fixedDt.Ticks, TimeSpan.Zero);
    } 
}