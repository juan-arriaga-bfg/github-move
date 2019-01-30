using System.Text.RegularExpressions;
using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorStandart : IJsonDataCollector
    {
        public string Name => "standart";

        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();
            node["server"] = 0;
            node["env"] = Regex.Match(IWProjectVersionSettings.Instance.CurrentVersion,@".*\.([a-zA-Z]+)$").Groups[1].Value;
            node["tv"] = 1;

            return node;
        }
    }
}