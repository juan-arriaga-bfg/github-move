using System.Collections.Generic;

public class UIDailyRewardWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.dailyReward.title", "window.dailyReward.title");
    
    public string Message => LocalizationService.Get("window.dailyReward.message", "window.dailyReward.message");
    
    public List<DailyRewardDef> Defs;

    public int Day;
}
