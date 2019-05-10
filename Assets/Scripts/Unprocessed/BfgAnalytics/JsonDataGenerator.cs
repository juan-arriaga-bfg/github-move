using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using IW.SimpleJSON;
using UnityEngine;

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

            try
            {
                JSONNode node = new JSONObject();

                if (customData != null)
                {
                    foreach (KeyValuePair<string, JSONNode> item in customData)
                    {
                        node.Add(item.Key, item.Value);
                    }
                }
                
                foreach (JsonDataGroup item in groups.AsArray())
                {
                    if (item == JsonDataGroup.None)
                    {
                        continue;
                    }

                    if (groups.Has(item))
                    {
                        if (collectors.TryGetValue(item, out IJsonDataCollector collector))
                        {
                            string name = collector.Name;
                            JSONNode data = collector.CollectData();
                            node.Add(name, data);
                        }
                        else
                        {
                            throw new Exception($"No collector for group {item.ToString()} is registered!");
                        }
                    }
                }

                return node.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonDataGenerator] => CollectData: Exception: {e.Message}\n{e.StackTrace}");
                return null;
            }
        }
    }
}