using System;

public class UIEventPreviewWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.event.title", "window.event.title");
    public string Message => LocalizationService.Get("window.event.preview.message", "window.event.preview.message");
    public string ButtonText => LocalizationService.Get("common.button.ok", "common.button.ok");
    public string TimerText
    {
        get
        {
            var description = LocalizationService.Get("window.event.preview.timer", "window.event.preview.timer");
            var time = Countdown != null && Countdown.IsStarted 
                ? Countdown.CompleteTime.GetTimeLeftText(Countdown.UseUTC, true, null, false, 2.5f)
                : DateTime.UtcNow.GetTimeLeftText(true, true, null, false, 2.5f);
            
            return $"<color=#FFFFFF><font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SubtitleFinal\">{description} </font></color>{time}";
        }
    }

    public TimerComponent Countdown;
}
