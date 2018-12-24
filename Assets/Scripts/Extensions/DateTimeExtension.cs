using System;
using System.Globalization;

public static class DateTimeExtension
{
    private static DateTime GetCurrentTime(bool utc)
    {
        return utc ? DateTime.UtcNow : DateTime.Now;
    }
    
    public static long ConvertToUnixTime(this DateTime datetime, bool useUTC = true)
    {
        var sTime = new DateTime(1970, 1, 1, 0, 0, 0, useUTC ? DateTimeKind.Utc : DateTimeKind.Local);
        return (long) (datetime - sTime).TotalSeconds;
    }
    
    public static long ConvertToUnixTimeMilliseconds(this DateTime datetime, bool useUTC = true)
    {
        var sTime = new DateTime(1970, 1, 1, 0, 0, 0, useUTC ? DateTimeKind.Utc : DateTimeKind.Local);
        return (long) (datetime - sTime).TotalMilliseconds;
    }
    
    public static DateTime UnixTimeToDateTime(long unixtime, bool useUTC = true)
    {
        var sTime = new DateTime(1970, 1, 1, 0, 0, 0, useUTC ? DateTimeKind.Utc : DateTimeKind.Local);
        return sTime.AddSeconds(unixtime);
    }

    public static int CountOfStepsPassedWhenAppWasInBackground(long then, int delay, out DateTime now, bool useUTC = true)
    {
        if (then == 0)
        {
            now = GetCurrentTime(useUTC);
            return 0;
        }
        
        return CountOfStepsPassedWhenAppWasInBackground(UnixTimeToDateTime(then), delay, out now);
    }
    
    public static int CountOfStepsPassedWhenAppWasInBackground(DateTime then, int delay, out DateTime now, bool useUTC = true)
    {
        var elapsedTime = GetCurrentTime(useUTC) - then;
        var count = (int) elapsedTime.TotalSeconds / delay;
        var remainder = (int) elapsedTime.TotalSeconds % delay;
        
        now = GetCurrentTime(useUTC).AddSeconds(-remainder);
        
        return count;
    }
    
    public static TimeSpan GetTime(this DateTime datetime, bool useUTC = true)
    {
        return (useUTC ? DateTime.UtcNow : DateTime.Now) - datetime;
    }

    public static TimeSpan GetTimeLeft(this DateTime datetime, bool useUTC = true)
    {
        return datetime - (useUTC ? DateTime.UtcNow : DateTime.Now);
    }
    
    public static string GetDelayText(int delay, bool icon = false, string format = null)
    {
        return TimeFormat(new TimeSpan(0, 0, delay - 1), icon, format);
    }

    public static string GetTimeLeftText(this DateTime datetime, bool useUTC, bool icon, string format, float mspace = 3f)
    {
        return TimeFormat(datetime.GetTimeLeft(useUTC), icon, format, mspace);
    }
    
    public static string GetTimeLeftText(this DateTime datetime, bool useUTC = true)
    {
        return TimeFormat(datetime.GetTimeLeft(useUTC), false, null);
    }
    
    public static string GetTimeLeftText(this DateTime datetime, bool useUTC, float mspace)
    {
        return TimeFormat(datetime.GetTimeLeft(useUTC), false, null, mspace);
    }
    
    private static string TimeFormat(TimeSpan time, bool icon, string format, float mspace = 3f)
    {
        string mspaceStr = mspace.ToString(CultureInfo.InvariantCulture);
        
        var temp = time.Add(new TimeSpan(0, 0, 1));
        
        if (string.IsNullOrEmpty(format))
        {
            var str = icon ? $"<sprite name={Currency.Timer.Icon}> " : "";
            
            return (int) temp.TotalHours > 0
                ? $"{str}<mspace={mspaceStr}em>{temp.Hours:00}:{temp.Minutes:00}:{temp.Seconds:00}</mspace>"
                : $"{str}<mspace={mspaceStr}em>{temp.Minutes:00}:{temp.Seconds:00}</mspace>";
        }
        
        return string.Format(format, temp.Hours, temp.Minutes, temp.Seconds);
    }
    
    /// <summary>
    /// Convert 2001.02.03 14:30 ->  2001.02.03 00:00
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime TruncDateTimeToDays(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
    }
}