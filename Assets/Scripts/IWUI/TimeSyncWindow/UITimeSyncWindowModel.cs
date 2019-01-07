using UnityEngine;
using System.Collections;

public class UITimeSyncWindowModel : IWWindowModel
{
     public string Title => LocalizationService.Get("window.timesync.title");
     public string ErrorText => LocalizationService.Get("window.timesync.message.error");
     public string ConnectingText => LocalizationService.Get("window.timesync.message.connecting");
     public string SuccessText => LocalizationService.Get("window.timesync.message.success");
}
