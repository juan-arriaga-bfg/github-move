// #define DEBUG_PRINT_SETS

using System;
using IW.SimpleJSON;
using JetBrains.Annotations;
using UnityEngine;

namespace BfgAnalytics
{
    public class BfgAnalyticsManager : IAnalyticsManager
    {
        public bool IsEnabled;

        private JsonDataGenerator jsonDataGenerator;

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
               .RegisterCollector(new JsonDataCollectorStandart(),  JsonDataGroup.Transaction);
        }

        public void Event(string category,
                          string type,
                          string name,
                          string action,
                          JsonDataGroup jsonDataGroups = JsonDataGroup.None,
                          long placeholder = 1,
                          long value = 0,
                          JSONNode customData = null)
        {
            try
            {
                string st1 = category.ToLower();
                string st2 = string.IsNullOrEmpty(type) ? null : type.ToLower();
                string st3 = string.IsNullOrEmpty(name) ? null : name.ToLower();

                if (!IsEnabled)
                {
                    Debug.Log($"[BfgAnalyticsManager] => WARNING! Analytics Event call when IsEnabled == false for: {st1} | {st2} | {st3}");
                    return;
                }

                string jsonData = jsonDataGenerator.CollectData(jsonDataGroups, customData);

                bool isOk = false;
                do
                {
                    if (string.IsNullOrEmpty(st1))
                    {
                        Debug.LogError("[BfgAnalyticsManager] => category is none");
                        break;
                    }

                    if (string.IsNullOrEmpty(st2) && !string.IsNullOrEmpty(st3))
                    {
                        Debug.LogError("[BfgAnalyticsManager] => 'type' is not set but 'name' has a value");
                        break;
                    }

                    if (st1.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => Event: 'category' too long: {0}", st1);
                        break;
                    }

                    if (st2 != null && st2.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => Event: 'type' too long: {0}", st2);
                        break;
                    }

                    if (st3 != null && st3.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => Event: 'name' too long: {0}", st3);
                        break;
                    }

                    // if (string.IsNullOrEmpty(action))
                    // {
                    //     Debug.LogError("[BfgAnalyticsManager] => 'action' field is required");
                    //     break;
                    // }

                    if (action.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => 'action' field too long: '{0}'", action);
                        break;
                    }

                    if (!(placeholder >= 0 && placeholder <= 255))
                    {
                        Debug.LogError("[BfgAnalyticsManager] => 'placeholder' value must be in 0-255");
                        break;
                    }

                    if (!(placeholder >= 0 && placeholder <= 255))
                    {
                        Debug.LogError("[BfgAnalyticsManager] => 'placeholder' value must be in 0-255");
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
                    Debug.Log($"[BfgAnalyticsManager] => Event: category: {st1}, type:{st2}, name:{st3}, action: {action}, placeholder: {placeholder}, value: {value}, data: {jsonDataGroups.PrettyPrint()}");
#if DEBUG_PRINT_SETS
                    Debug.Log($"[BfgAnalyticsManager] => Data:\n{IdentJson(jsonData)}");
#endif
#endif

#if !UNITY_EDITOR
{
                // Fields naming
                // st1 - details1 - category
                // st2 - details2 - type
                // st3 - details3 - name
                // n   - name     - action
                // l   - placeholder    - placeholder
                // v   - value    - value
                bfgGameReporting.logCustomEventSerialized(action,value,placeholder,st1, st2, st3, jsonData);
#endif
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[BfgAnalyticsManager] => " + e);
            }
            finally
            {
                //
            }
        }

    }
}