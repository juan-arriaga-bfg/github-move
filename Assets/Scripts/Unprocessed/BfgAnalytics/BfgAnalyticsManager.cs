#define DEBUG_PRINT_SETS
using Debug = IW.Logger;
using System;
using IW.SimpleJSON;
using JetBrains.Annotations;
using UnityEngine;
using BFGSDK;

namespace BfgAnalytics
{
    public class BfgAnalyticsManager : IAnalyticsManager
    {
        public bool IsEnabled { get; set; }
        
        private readonly JsonDataGenerator jsonDataGenerator;

        [UsedImplicitly]
        private string IdentJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return json;
            }

            JSONNode node = JSONNode.Parse(json);
            return node.ToString(4);
        }

        public BfgAnalyticsManager()
        {
            jsonDataGenerator = new JsonDataGenerator()
               .RegisterCollector(new JsonDataCollectorUserStats(), JsonDataGroup.Userstats)
               .RegisterCollector(new JsonDataCollectorStory(),     JsonDataGroup.Story)
               .RegisterCollector(new JsonDataCollectorFlags(),     JsonDataGroup.Flags)
               .RegisterCollector(new JsonDataCollectorBalances(),  JsonDataGroup.Balances)
               .RegisterCollector(new JsonDataCollectorStandart(),  JsonDataGroup.Standart)
               // .RegisterCollector(new JsonDataCollectorTransaction(),  JsonDataGroup.Transaction)
               .RegisterCollector(new JsonDataCollectorAbTest(),    JsonDataGroup.Abtest);
        }

        public void Event(string eventName,
                          string name,
                          string type,
                          string action,
                          JsonDataGroup jsonDataGroups = JsonDataGroup.None,
                          JSONObject customData = null)
        {
            try
            {
                if (!IsEnabled)
                {
                    Debug.Log($"[BfgAnalyticsManager] => WARNING! Analytics Event call when IsEnabled == false for: {name} | {type} | {action}");
                    return;
                }

                eventName = eventName?.ToLower();
                name      = name?.ToLower();
                type      = type?.ToLower();
                action    = action?.ToLower();
                
                customData = customData ?? new JSONObject();

                if (type != null)
                {
                    customData["type"] = type;
                }
                
                if (action != null)
                {
                    customData["action"] = action;
                }

                string jsonData = jsonDataGenerator.CollectData(jsonDataGroups, customData);
                
                bool isOk = false;
                do
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => Event: 'name' is required but null or empty value provided");
                        break;
                    }
                    
                    if (string.IsNullOrEmpty(eventName))
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => Event: 'eventName' is required but null or empty value provided");
                        break;
                    }
                    
                    if (name != null && name.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => Event: 'name' too long: {0}", name);
                        break;
                    }
                    
                    if (type != null && type.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => Event: 'type' too long: {0}", type);
                        break;
                    }
                    
                    if (action != null && action.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => 'action' field too long: '{0}'", action);
                        break;
                    }

                    if (!string.IsNullOrEmpty(jsonData) && jsonData.Length >= 3000)
                    {
                        Debug.LogError("[BfgAnalyticsManager] => jsonData.Length >= 3000");
                        break;
                    }
                    isOk = true;
                } while (false);

                if (isOk)
                {
#if DEBUG
                    Debug.Log($"[BfgAnalyticsManager] => Event: type: '{type ?? "null"}', action: '{action ?? "null"}', name: '{name ?? "null"}', data groups: {jsonDataGroups.PrettyPrint()}");
    #if DEBUG_PRINT_SETS
                    Debug.Log($"[BfgAnalyticsManager] => Data:\n{IdentJson(jsonData)}");
    #endif
#endif

#if !UNITY_EDITOR
                    bfgGameReporting.logCustomEventSerialized(name, 0,0, eventName, null, null, jsonData);
#endif
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[BfgAnalyticsManager] => " + e);
            }
        }

    }
}