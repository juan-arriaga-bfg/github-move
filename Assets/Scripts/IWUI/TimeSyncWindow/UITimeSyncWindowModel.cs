public class UITimeSyncWindowModel : IWWindowModel
{
     public string TitleError => LocalizationService.Get("window.timesync.title.error").Replace(" {0}", "<mspace=2.5em> {0}</mspace>");
     public string TitleConnecting => LocalizationService.Get("window.timesync.title.connecting");
     public string TitleSuccess => LocalizationService.Get("window.timesync.title.success");
     public string MessageError => LocalizationService.Get("window.timesync.message.error");
     public string MessageConnecting => LocalizationService.Get("window.timesync.message.connecting");
     public string MessageSuccess => LocalizationService.Get("window.timesync.message.success");
}
