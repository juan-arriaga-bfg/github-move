public class BfgSdkBrandingInitComponent : ThirdPartyInitComponent
{
    private bool isCompleted;
    
    public override bool IsCompleted => isCompleted;

    private void OnBrandingCompleteNotification (string notification)
    {
        isCompleted = true;
    }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
#if UNITY_EDITOR
        isCompleted = true;
        return;
#endif
        
        NotificationCenter.Instance.AddObserver (OnBrandingCompleteNotification, bfgCommon.BFGBRANDING_NOTIFICATION_COMPLETED);
        bfgManager.startBranding();
    }
}