using System.Collections.Generic;
using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataGenerator
    {
        private readonly Dictionary<JsonDataGroup, IJsonDataCollector> collectors = new Dictionary<JsonDataGroup, IJsonDataCollector>();

        public JsonDataGenerator RegisterCollector(IJsonDataCollector collector, JsonDataGroup group)
        {
            collectors.Add(group, collector);

            return this;
        }

        public string CollectData(JsonDataGroup groups, JSONNode customData = null)
        {
            if (groups == JsonDataGroup.None && customData == null)
            {
                return null;
            }
            
            JSONNode node = new JSONObject();
            
            foreach (JsonDataGroup item in groups.AsArray())
            {
                if (groups.Has(item))
                {
                    var collector = collectors[item];
                    string name = collector.Name;
                    JSONNode data = collector.CollectData();
                    node.Add(name, data);
                }
            }

            if (customData != null)
            {
                node.Add(customData);
            }

            return node.ToString();
        }
    }
}