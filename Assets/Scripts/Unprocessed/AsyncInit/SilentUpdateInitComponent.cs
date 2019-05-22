using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;

public class SilentUpdateInitComponent : AsyncInitComponentBase
{
    private const float TIMEOUT = 5f;

    private const string KEY = "SilentUpdateInitComponent_lastCheckedVersion";
    
    public override void Execute()
    {
        SilentUpdateManager manager = new SilentUpdateManager();
        SilentUpdateService.Instance.SetManager(manager);

        string currentVersion = IWProjectVersionSettings.Instance.ProductionVersion;
        string lastCheckedVersion = ObscuredPrefs.GetString(KEY, null);

        if (currentVersion == lastCheckedVersion)
        {
            IW.Logger.Log($"[SilentUpdateInitComponent] => Wait for updates disabled because version {currentVersion} already checked");
                
            manager.Init(null);
            isCompleted = true;
            OnComplete(this);
            return;
        }

        IW.Logger.Log($"[SilentUpdateInitComponent] => First check for v{currentVersion}! Let's wait for updates");

        DOTween.Sequence()
               .SetId(this)
               .AppendInterval(TIMEOUT)
               .AppendCallback(() =>
                {
                    IW.Logger.Log($"[SilentUpdateInitComponent] => Wait for updates break by timeout");
                    
                    isCompleted = true;
                    OnComplete(this);
                });
        
        manager.Init((isOk) =>
        {
            if (isCompleted)
            {
                return;
            }

            DOTween.Kill(this);

            if (isOk)
            {
                ObscuredPrefs.SetString(KEY, currentVersion);
                manager.ApplyPendingPackages();
            }
            
            isCompleted = true;
            OnComplete(this);
        });
    }
}