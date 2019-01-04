using System;
using BfgAnalytics;
using IW.SimpleJSON;
using UnityEngine;

namespace BfgAnalytics
{
    public class BfgAnalyticsManager
    {
        public bool IsEnabled;

        private JsonDataGenerator jsonDataGenerator;

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
               .RegisterCollector(new JsonDataCollectorUserNode(), JsonDataGroup.Userstats);
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
                    Debug.Log($"WARNING! Analytics Event call when IsEnabled == false for: {st1} | {st2} | {st3}");
                    return;
                }

                string jsonData = jsonDataGenerator.CollectData(groups, customData);

                bool isOk = false;
                do
                {
#if DEBUG
                    if (string.IsNullOrEmpty(st1))
                    {
                        Debug.LogError("Analytics: details1 is none");
                        break;
                    }

                    if (string.IsNullOrEmpty(st2) && !string.IsNullOrEmpty(st3))
                    {
                        Debug.LogError("Analytics: details2 is none but details3 is set");
                        break;
                    }

                    if (st1.Length > 31)
                    {
                        Debug.LogErrorFormat("Analytics: Event: st1 too long: {0}", st1);
                        break;
                    }

                    if (st2 != null && st2.Length > 31)
                    {
                        Debug.LogErrorFormat("Analytics: Event: st2 too long: {0}", st2);
                        break;
                    }

                    if (st3 != null && st3.Length > 31)
                    {
                        Debug.LogErrorFormat("Analytics: Event: st3 too long: {0}", st3);
                        break;
                    }

                    if (string.IsNullOrEmpty(name))
                    {
                        Debug.LogError("Analytics: name field is required");
                        break;
                    }

                    if (name.Length > 31)
                    {
                        Debug.LogErrorFormat("Analytics: name field too long: '{0}'", name);
                        break;
                    }

                    if (!(level >= 0 && level <= 255))
                    {
                        Debug.LogError("Analytics: level value must be in 0-255");
                        break;
                    }

                    if (!(level >= 0 && level <= 255))
                    {
                        Debug.LogError("Analytics: level value must be in 0-255");
                        break;
                    }

                    if (!string.IsNullOrEmpty(jsonData) && jsonData.Length >= 3000)
                    {
                        Debug.LogError("Analytics: jsonData.Length >= 3000");
                        break;
                    }
#endif
                    isOk = true;
                } while (false);

                if (isOk)
                {
#if DEBUG
                    Debug.Log($"Analytics: Event: st1: {st1}, st2:{st2}, st3:{st3}, name: {name}, level: {level}, value: {value}, sets: {groups.PrettyPrint()}");
#if DEBUG_PRINT_SETS
                Debug.Log($"Analytics: JsonData:\n{IdentJson(jsonData)}");
#endif
#endif

#if !UNITY_EDITOR
                bfgGameReporting.logCustomEventSerialized(name,value,level,st1, st2, st3, jsonData);
#endif
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Analytics: " + e);
            }
            finally
            {
                //
            }
        }

    }
}