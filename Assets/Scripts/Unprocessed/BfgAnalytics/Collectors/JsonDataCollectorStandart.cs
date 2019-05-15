using System.Text.RegularExpressions;
using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorStandart : IJsonDataCollector
    {
        public string Name => "standard";

        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();
            node["server"] = 0;
#if DEBUG
            node["env"] = "qa";
#else
            node["env"] = "prod";
#endif
            node["tv"] = 1;

            return node;
        }
    }
}