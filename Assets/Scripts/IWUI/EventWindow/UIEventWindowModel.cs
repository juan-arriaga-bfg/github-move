using System;

public class UIEventWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.event.title", "window.event.title");
    public string Message => LocalizationService.Get("window.event.message", "window.event.message");
    public string MessageFinish => LocalizationService.Get("window.event.message.finish", "window.event.message.finish");
    
    public string ButtonText => LocalizationService.Get("common.button.show", "common.button.show");
    public string ButtonFinishText => LocalizationService.Get("window.event.button.finish", "window.event.button.finish");
    
    public string VIPText => LocalizationService.Get("window.event.vip", "window.event.vip");
    
    public string TimerText
    {
        get
        {
            var description = LocalizationService.Get("window.event.timer", "window.event.timer");
            var time = Timer != null && Timer.IsStarted 
                ? Timer.CompleteTime.GetTimeLeftText(Timer.UseUTC, true, null, true, 2.5f)
                : DateTime.UtcNow.GetTimeLeftText(true, true, null, true, 2.5f);
            
            return $"<color=#FFFFFF><font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR SubtitleFinal\">{description} </font></color>{time}";
        }
    }

    public TimerComponent Timer;
}
