using IW.SimpleJSON;

namespace BfgAnalytics
{
    public interface IAnalyticsManager
    {
        bool IsEnabled { get; set; }

        void Event(string eventName, // details1
                   string name,
                   string type,
                   string action,
                   JsonDataGroup jsonDataGroups = JsonDataGroup.None,
                   JSONObject customData = null);
    }
}