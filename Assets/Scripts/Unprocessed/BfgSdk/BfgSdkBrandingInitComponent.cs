public class BfgSdkBrandingInitComponent : AsyncInitComponentBase
{
    private void OnBrandingCompleteNotification (string notification)
    {
        isCompleted = true;
        OnComplete(this);
    }
    
    public override void Execute()
    {
#if UNITY_EDITOR
        isCompleted = true;
        OnComplete(this);
        return;
#endif
        
        NotificationCenter.Instance.AddObserver (OnBrandingCompleteNotification, bfgCommon.BFGBRANDING_NOTIFICATION_COMPLETED);
        bfgManager.startBranding();
    }
}