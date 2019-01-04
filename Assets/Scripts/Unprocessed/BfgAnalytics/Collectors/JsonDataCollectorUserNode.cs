using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorUserNode : IJsonDataCollector
    {
        public string Name => "usernode";

        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();
            node["eoc"] = 0;
            node["cheater"] = 0;
            node["profile_creation_date"] = "01.01.2019";
            node["top_pieces"] = GetTopPiecesInformation();

            return node;
        }

        private JSONNode GetTopPiecesInformation()
        {
            JSONNode node = new JSONObject();
            node["A"] = "A1";
            node["B"] = "B1";

            return node;
        }
    }
}