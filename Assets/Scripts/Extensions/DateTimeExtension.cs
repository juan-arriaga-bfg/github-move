﻿using System;

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
    
    public static TimeSpan GetTime(this DateTime datetime)
    {
        return DateTime.UtcNow - datetime;
    }

    public static TimeSpan GetTimeLeft(this DateTime datetime)
    {
        return datetime - DateTime.UtcNow;
    }
    
    public static string GetDelayText(int delay, bool icon = false, string format = null)
    {
        return TimeFormat(new TimeSpan(0, 0, delay - 1), icon, format);
    }

    public static string GetTimeText(this DateTime datetime, bool icon = false, string format = null)
    {
        return TimeFormat(datetime.GetTime(), icon, format);
    }
    
    public static string GetTimeLeftText(this DateTime datetime, bool icon = false, string format = null)
    {
        return TimeFormat(datetime.GetTimeLeft(), icon, format);
    }
    
    private static string TimeFormat(TimeSpan time, bool icon, string format)
    {
        var temp = time.Add(new TimeSpan(0, 0, 1));
        
        if (string.IsNullOrEmpty(format))
        {
            var str = icon ? $"<sprite name={Currency.Timer.Name}> " : "";
            
            return (int) temp.TotalHours > 0
                ? $"{str}<mspace=3em>{temp.Hours:00}:{temp.Minutes:00}:{temp.Seconds:00}</mspace>"
                : $"{str}<mspace=3em>{temp.Minutes:00}:{temp.Seconds:00}</mspace>";
        }
        
        return string.Format(format, temp.Hours, temp.Minutes, temp.Seconds);
    }
}