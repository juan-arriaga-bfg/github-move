using System;
using System.Globalization;
using System.Text;

public static class DateTimeExtension
{
    private static DateTime GetCurrentTime(bool utc)
    {
        // return utc ? DateTime.UtcNow : DateTime.Now;

        var secureTime = SecuredTimeService.Current;
        return utc ? secureTime.UtcNow : secureTime.Now;
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
        //return (useUTC ? DateTime.UtcNow : DateTime.Now) - datetime;
        var secureTime = SecuredTimeService.Current;
        return (useUTC ? secureTime.UtcNow : secureTime.Now) - datetime;
    }

    public static TimeSpan GetTimeLeft(this DateTime datetime, bool useUTC = true)
    {
        // return datetime - (useUTC ? DateTime.UtcNow : DateTime.Now);
        var secureTime = SecuredTimeService.Current;
        return datetime - (useUTC ? secureTime.UtcNow : secureTime.Now);
    }
    
    public static string GetDelayText(int delay, bool icon = false, string format = null, bool daysConvert = false)
    {
        return TimeFormat(new TimeSpan(0, 0, delay), icon, format, true, daysConvert: daysConvert);
    }
    
    public static string GetTimeLeftText(this DateTime datetime, bool useUTC, bool icon, string format, bool isColon = true, bool daysConvert = false)
    {
        return TimeFormat(datetime.GetTimeLeft(useUTC), icon, format, isColon, daysConvert: daysConvert);
    }

    public static string GetTimeLeftText(this DateTime datetime, bool useUTC, bool icon, string format, bool isColon = true, float mspace = 3f, bool daysConvert = false)
    {
        return TimeFormat(datetime.GetTimeLeft(useUTC), icon, format, isColon, mspace, daysConvert);
    }
    
    public static string GetTimeLeftText(this DateTime datetime, bool useUTC = true, bool daysConvert = false)
    {
        return TimeFormat(datetime.GetTimeLeft(useUTC), false, null, true, 3f, daysConvert);
    }
    
    public static string GetTimeLeftText(this DateTime datetime, bool useUTC, float mspace, bool daysConvert = false)
    {
        return TimeFormat(datetime.GetTimeLeft(useUTC), false, null, true, mspace, daysConvert);
    }
    
    public static string TimeFormat(TimeSpan time, bool icon, string format, bool isColon = true, float mspace = 3f, bool daysConvert = false)
    {
        var iconStr = icon ? $"<sprite name={Currency.Timer.Icon}> " : "";
        var mspaceStr = mspace.ToString(CultureInfo.InvariantCulture);
        
        var temp = time.Add(new TimeSpan(0, 0, 0, 0, 500));
        string value;

        var d = isColon ? ":" : $"{LocalizationService.Get("common.abbreviation.day")} ";
        var h = isColon ? ":" : $"{LocalizationService.Get("common.abbreviation.hour")} ";
        var m = isColon ? (daysConvert && temp.Days > 0 ? "" : ":") : $"<space=0.6em>{LocalizationService.Get("common.abbreviation.minute")} ";
        var s = isColon ? "" : $"{LocalizationService.Get("common.abbreviation.second")} ";
        
        if (string.IsNullOrEmpty(format)) format = "{0}<mspace={1}em>{2}</mspace>";

        if (daysConvert && temp.Days > 0) value = $"{temp.Days:0}{d}{temp.Hours:00}{h}{temp.Minutes:00}{m}";
        else if ((int) temp.TotalHours > 0) value = $"{temp.Days * 24 + temp.Hours:0}{h}{temp.Minutes:00}{m}{temp.Seconds:00}{s}";
        else if ((int) temp.TotalSeconds > 0) value = $"{temp.Minutes:00}{m}{temp.Seconds:00}{s}";
        else value = $"00{m}00{s}";
        
        return string.Format(format, iconStr, mspaceStr, value);
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