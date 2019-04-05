using System;

public class DataManagersTimersHandlerComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid => ComponentGuid;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        GameDataService.Current.DailyRewardManager.StartTimer();
        GameDataService.Current.QuestsManager.StartDailyTimer();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        GameDataService.Current.DailyRewardManager.StopTimer();
        GameDataService.Current.QuestsManager.StopDailyTimer();
    }
}