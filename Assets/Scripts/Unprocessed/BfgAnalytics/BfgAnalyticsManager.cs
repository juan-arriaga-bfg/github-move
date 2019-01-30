using System;
using BfgAnalytics;
using IW.SimpleJSON;
using JetBrains.Annotations;
using UnityEngine;

namespace BfgAnalytics
{
    public class BfgAnalyticsManager
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
               .RegisterCollector(new JsonDataCollectorStory(), JsonDataGroup.Story)
               .RegisterCollector(new JsonDataCollectorFlags(), JsonDataGroup.Flags)
               .RegisterCollector(new JsonDataCollectorBalance(), JsonDataGroup.Balance)
               .RegisterCollector(new JsonDataCollectorStandart(), JsonDataGroup.Standart);
        }

        public void Event(string details1,
                          string details2,
                          string details3,
                          string name,
                          JsonDataGroup groups = JsonDataGroup.None,
                          long level = 1,
                          long value = 0,
                          JSONNode customData = null)
        {
            try
            {
                string st1 = details1.ToLower();
                string st2 = string.IsNullOrEmpty(details2) ? null : details2.ToLower();
                string st3 = string.IsNullOrEmpty(details2) ? null : details2.ToLower();

                if (!IsEnabled)
                {
                    Debug.Log($"[BfgAnalyticsManager] => WARNING! Analytics Event call when IsEnabled == false for: {st1} | {st2} | {st3}");
                    return;
                }

                string jsonData = jsonDataGenerator.CollectData(groups, customData);

                bool isOk = false;
                do
                {
                    if (string.IsNullOrEmpty(st1))
                    {
                        Debug.LogError("[BfgAnalyticsManager] => details1 is none");
                        break;
                    }

                    if (string.IsNullOrEmpty(st2) && !string.IsNullOrEmpty(st3))
                    {
                        Debug.LogError("[BfgAnalyticsManager] => details2 is none but details3 is set");
                        break;
                    }

                    if (st1.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => Event: st1 too long: {0}", st1);
                        break;
                    }

                    if (st2 != null && st2.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => Event: st2 too long: {0}", st2);
                        break;
                    }

                    if (st3 != null && st3.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => Event: st3 too long: {0}", st3);
                        break;
                    }

                    if (string.IsNullOrEmpty(name))
                    {
                        Debug.LogError("[BfgAnalyticsManager] => name field is required");
                        break;
                    }

                    if (name.Length > 31)
                    {
                        Debug.LogErrorFormat("[BfgAnalyticsManager] => name field too long: '{0}'", name);
                        break;
                    }

                    if (!(level >= 0 && level <= 255))
                    {
                        Debug.LogError("[BfgAnalyticsManager] => level value must be in 0-255");
                        break;
                    }

                    if (!(level >= 0 && level <= 255))
                    {
                        Debug.LogError("[BfgAnalyticsManager] => level value must be in 0-255");
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
                    Debug.Log($"[BfgAnalyticsManager] => Event: st1: {st1}, st2:{st2}, st3:{st3}, name: {name}, level: {level}, value: {value}, sets: {groups.PrettyPrint()}");
#if DEBUG_PRINT_SETS
                Debug.Log($"[BfgAnalyticsManager] => JsonData:\n{IdentJson(jsonData)}");
#endif
#endif

#if !UNITY_EDITOR
                bfgGameReporting.logCustomEventSerialized(name,value,level,st1, st2, st3, jsonData);
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