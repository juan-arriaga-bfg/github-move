using BfgAnalytics;

public class BfgSdkAnalyticsInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        BfgAnalyticsManager bfgAnalyticsManager = new BfgAnalyticsManager();
        AnalyticsService.Instance.SetManager(bfgAnalyticsManager);
        bfgAnalyticsManager.IsEnabled = true;
        
        isCompleted = true;
        OnComplete(this);
    }
}