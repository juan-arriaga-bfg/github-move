using System;
using System.Collections.Generic;
using IW.SimpleJSON;
using UnityEngine;

namespace BfgAnalytics
{
    public static class Analytics
    {
        // Fields naming
        // st1 - details1 - category= economy
        // st2 - details2 - type
        // st3 - details3 - name
        // n   - name     - action
        // l   - level    - placeholder
        // v   - value    - value

        // https://docs.google.com/spreadsheets/d/1zMj09QQnhSDSs9hYdzrmM02KO044h853QaPOdFqLimc/edit#gid=1419311239
        
        public static JsonDataGroup DefaultJsonData()
        {
            return JsonDataGroup.Standart | JsonDataGroup.Userstats | JsonDataGroup.Balances | JsonDataGroup.Flags | JsonDataGroup.Story | JsonDataGroup.Abtest;
        }

        public static JsonDataGroup TutorialJsonData()
        {
            return JsonDataGroup.Standart | JsonDataGroup.Abtest;
        }
        
        public static void SendQuestStartEvent(string questId)
        {
            AnalyticsService.Current?.Event("progress", "quest", questId, "start", DefaultJsonData());
        }
        
        public static void SendQuestCompletedEvent(string id)
        {
            AnalyticsService.Current?.Event("progress", "quest", id, "end", DefaultJsonData());
        }
        
        public static void SendPieceUnlockedEvent(string id)
        {
            AnalyticsService.Current?.Event("progress", "piece", id, "unlock", DefaultJsonData());
        }
        
        public static void SendCharUnlockedEvent(string id)
        {
            AnalyticsService.Current?.Event("progress", "character", id, "unlock", DefaultJsonData());
        }
        
        public static void SendFogClearedEvent(string id)
        {
            AnalyticsService.Current?.Event("progress", "fog", id, "unlock", DefaultJsonData());
        }
        
        public static void SendLevelReachedEvent(int level)
        {
            AnalyticsService.Current?.Event("progress", "level", level.ToString(), null, DefaultJsonData());
        }

		public static void SendTutorialStartStepEvent(string name)
        {   
            JSONNode customJsonData = new JSONObject();
            JSONNode ctrNode = new JSONObject();

            customJsonData["ctr"] = ctrNode;

            ctrNode["seconds"] = 0;
            
            AnalyticsService.Current?.Event("ftue", null, name, "start", TutorialJsonData(), customJsonData);
        }

        public static void SendTutorialEndStepEvent(string name, DateTime startTime)
        {
            JSONNode customJsonData = new JSONObject();
            JSONNode ctrNode = new JSONObject();

            customJsonData["ctr"] = ctrNode;

            var seconds = (DateTime.UtcNow - startTime).TotalSeconds;
            ctrNode["seconds"] = (int)Math.Round(seconds);
            
            AnalyticsService.Current?.Event("ftue", null, name, "end", TutorialJsonData(), customJsonData);
        }
        
        public static void SendPurchase(string location, string reason, List<CurrencyPair> spend, List<CurrencyPair> collect, bool isIap, bool isFree)
        {
            JSONNode customJsonData = CreateTransaction(location, reason, spend, collect, isIap, isFree);

            // todo: Договориться насчет type, name, action
            AnalyticsService.Current?.Event("economy", null, null, null, DefaultJsonData(), customJsonData);
        }
        
        public static void SendDailyRewardClaim(int day)
        {
            AnalyticsService.Current?.Event("activity", "dailyreward", day.ToString(), null, DefaultJsonData());
        }

        private static JSONNode CreateTransaction(string location, string reason, List<CurrencyPair> spend, List<CurrencyPair> collect, bool isIap, bool isFree)
        {
            JSONNode customJsonData = new JSONObject();
            JSONNode transactionNode = new JSONObject();
            
            transactionNode["location"] = location;
            transactionNode["reason"] = reason;
            transactionNode["isiap"] = isIap ? 1 : 0;
            transactionNode["isfree"] = isFree ? 1 : 0;

            if (spend != null)
            {
                var node = CreateCurrenciesJson(spend);
                if (node.Count != 0) transactionNode["spend"] = node;
            }
            
            if (collect != null)
            {
                var node = CreateCurrenciesJson(collect);
                if (node.Count != 0) transactionNode["collect"] = node;
            }
            
            customJsonData.Add("transaction", transactionNode);
            
            return customJsonData;
        }
        
        /// <summary>
        /// Create json like this: {"premium": {"crystals": 10}, "soft": {"coins": 500}}
        /// </summary>
        /// <param name="currencies"></param>
        /// <returns></returns>
        private static JSONNode CreateCurrenciesJson(List<CurrencyPair> currencies)
        {
            JSONNode CreateCurrencyNode(CurrencyPair pair)
            {
                JSONNode node = new JSONObject();
                node[pair.Currency] = new JSONNumber(pair.Amount);
                return node;
            }
            
            var ret = new JSONObject();
            var premium = new JSONArray();
            var soft = new JSONArray();

            foreach (var pair in currencies)
            {
                if (pair == null) continue;
                
                if (pair.Currency == Currency.Crystals.Name)
                {
                    premium.Add(CreateCurrencyNode(pair));
                    continue;
                }

                if (pair.Currency == Currency.Coins.Name)
                {
                    soft.Add(CreateCurrencyNode(pair));
                    continue;
                }
            }

            if (premium.Count > 0) ret.Add("premium", premium);
            if (soft.Count > 0) ret.Add("soft", soft);

            return ret;
        }
    }
}