 using System;
 using System.Collections.Generic;
 using UnityEngine;

 public class ForcedUpdateManager : ECSEntity
{    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    const string QUEUE_ID = "ForcedUpdate";
    
    private static DateTime? lastNotifyTime;
    
    private long VersionToLong(string versionStr)
    {
        string[] split = versionStr.Split('.');
        long num = int.Parse(split[0]) * 10000000 
                 + int.Parse(split[1]) * 10000 
                 + int.Parse(split[2]);

        return num;
    }

    public void Init()
    {
        ServerSideConfigService.Current.OnDataReceived += OnDataReceived;
        Check();
    }

    public void Cleanup()
    {
        ServerSideConfigService.Current.OnDataReceived -= OnDataReceived;
    }

    private void OnDataReceived(int guid, object data)
    {
        if (guid == ForcedUpdateServerSideConfigLoader.ComponentGuid)
        {
            Check();
        }
    }

    public void Check()
    {
        var serverData = ServerSideConfigService.Current?.GetData<ForcedUpdateServerConfig>();
        if (serverData == null)
        {
            IW.Logger.Log($"[ForcedUpdateManager] => Check: Skip by no data");
            return;
        }

        if (string.IsNullOrEmpty(serverData.ForceVersion))
        {
            IW.Logger.LogError($"[ForcedUpdateManager] => Check: ForceVersion is null or empty");
            return;
        }

        if (string.IsNullOrEmpty(serverData.NotifyVersion))
        {
            IW.Logger.LogError($"[ForcedUpdateManager] => Check: NotifyVersion is null or empty");
            return;
        }

        if (serverData.ForceVersion == "0" || serverData.NotifyVersion == "0")
        {
            IW.Logger.Log($"[ForcedUpdateManager] => Check: Forced update is not enabled on the server.");
            return;
        }

        try
        {
            string currentVersionStr = IWProjectVersionSettings.Instance.ProductionVersion;
            IW.Logger.Log($"[ForcedUpdateManager] => Check: current: {currentVersionStr}, force: {serverData.ForceVersion}, notify: {serverData.NotifyVersion}");

            long currentVersion = VersionToLong(currentVersionStr);
            long forceVersion   = VersionToLong(serverData.ForceVersion);
            long notifyVersion  = VersionToLong(serverData.NotifyVersion);

            if (currentVersion < forceVersion)
            {
                IW.Logger.Log($"[ForcedUpdateManager] => Check: Schedule FORCE: {currentVersionStr} < {serverData.ForceVersion}");
                ScheduleMessage(true);
            } 
            else if (currentVersion < notifyVersion)
            {
                IW.Logger.Log($"[ForcedUpdateManager] => Check: Schedule NOTIFY: {currentVersionStr} < {serverData.NotifyVersion}");
                ScheduleMessage(false);
            }
            else
            {
                ProfileService.Current?.QueueComponent?.RemoveAction(QUEUE_ID);
                IW.Logger.Log($"[ForcedUpdateManager] => Check: Nn action required");
            }
        }
        catch (Exception e)
        {
            IW.Logger.LogError($"[ForcedUpdateManager] => Check: {e.Message}");
        }
    }
    
    private static void VisitStore()
    {
#if UNITY_EDITOR
        var url = "https://www.google.com/search?q=kittens&source=lnms&tbm=isch&sa=X&ved=0ahUKEwiGtYXizJjiAhUMaFAKHWm1DoAQ_AUIDigB&biw=1920&bih=987";
#else
    #if UNITY_ANDROID
        #if AMAZON
            var url = "amzn://apps/android?p=com.bigfishgames.robinlegendsamazon";
        #else
            var url = "market://details?id=com.bigfishgames.mergetalesgoog";
    #endif
    #elif UNITY_IOS
            var url = "itms-apps://itunes.apple.com/app/id1266459297";
    #endif
#endif

        Application.OpenURL(url);
    }

    private void OnDialogCancelClick()
    {
        ProfileService.Current?.QueueComponent?.RemoveAction(QUEUE_ID);
    }

    private void OnDialogUpdateClick()
    {
        VisitStore();
        ProfileService.Current?.QueueComponent?.RemoveAction(QUEUE_ID);
    }
    
    private void ScheduleMessage(bool force)
    {
        if (ProfileService.Current?.QueueComponent == null)
        {
            IW.Logger.LogError($"[ForcedUpdateManager] => ScheduleMessage: Queue component is not initialized yet!");
            return;
        }

        DefaultSafeQueueBuilder.BuildAndRun(QUEUE_ID, true, () =>
        {
            DateTime now = DateTime.UtcNow;
            if (lastNotifyTime != null)
            {
                double dt = Math.Abs((now - lastNotifyTime.Value).TotalSeconds);
                const int DELAY = 60 * 60;
                if (dt < 60 * 60)
                {
                    IW.Logger.Log($"[ForcedUpdateManager] => ScheduleMessage: ShowWindow: Skip by time: {DELAY - dt}s remaining");
                }
                return;
            }

            lastNotifyTime = now;

            string btnUpdate = LocalizationService.Get("window.forced.update.btn.update", "window.forced.update.btn.update");

            if (force)
            {
                string forceTitle   = LocalizationService.Get("window.forced.update.force.title",   "window.forced.update.force.title");
                string forceMessage = LocalizationService.Get("window.forced.update.force.message", "window.forced.update.force.message");

                var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
                model.Title = forceTitle;
                model.Message = forceMessage;
                model.AcceptLabel = btnUpdate;
                model.ProhibitClose = true;
        
                model.OnAccept = () =>
                {
                    OnDialogUpdateClick();
                };
        
                UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            }
            else
            {
                string notifyTitle   = LocalizationService.Get("window.forced.update.notify.title",   "window.forced.update.notify.title");
                string notifyMessage = LocalizationService.Get("window.forced.update.notify.message", "window.forced.update.notify.message");
                string btnCancel    = LocalizationService.Get("window.forced.update.btn.cancel",    "window.forced.update.btn.cancel");
                
                var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
                model.Title = notifyTitle;
                model.Message = notifyMessage;
                model.AcceptLabel = btnUpdate;
                model.CancelLabel = btnCancel;
        
                model.OnAccept = () => { OnDialogUpdateClick();};
                model.OnCancel = () => { OnDialogCancelClick();};
                model.OnClose  = () => { OnDialogCancelClick();};
        
                UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            }
        });
    }
}