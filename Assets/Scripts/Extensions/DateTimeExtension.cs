using System;

public static class DateTimeExtension
{
    public static long ConvertToUnixTime(this DateTime datetime)
    {
        var sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return (long) (datetime - sTime).TotalSeconds;
    }
    
    public static DateTime UnixTimeToDateTime(long unixtime)
    {
        var sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return sTime.AddSeconds(unixtime);
    }
}