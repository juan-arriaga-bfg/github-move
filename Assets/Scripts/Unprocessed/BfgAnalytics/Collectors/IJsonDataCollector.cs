using IW.SimpleJSON;

namespace BfgAnalytics
{
    public interface IJsonDataCollector
    {
        string Name { get; }
        
        JSONNode CollectData();
    }
}