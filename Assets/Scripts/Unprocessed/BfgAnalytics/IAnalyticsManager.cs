using IW.SimpleJSON;

namespace BfgAnalytics
{
    public interface IAnalyticsManager
    {
        bool IsEnabled { get; set; }
        
        // Fields naming
        // st1 - details1 - category
        // st2 - details2 - type
        // st3 - details3 - name
        // n   - name     - action
        // l   - level    - placeholder
        // v   - value    - value
        void Event(string category,
                   string type,
                   string name,
                   string action,
                   JsonDataGroup jsonDataGroups,
                   long placeholder = 1,
                   long value = 0,
                   JSONNode customJsonData = null);
        
        void Event(string category,
                   string type,
                   string name,
                   string action,
                   JsonDataGroup jsonDataGroups,
                   JSONNode customJsonData);
    }
}