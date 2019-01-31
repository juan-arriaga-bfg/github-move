using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorTransaction : IJsonDataCollector
    {
        public string Name => "transaction";

        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();
            return node;
        }
    }
}