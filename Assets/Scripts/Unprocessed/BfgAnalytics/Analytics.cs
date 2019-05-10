using System;
using System.Collections.Generic;
using IW.SimpleJSON;
using UnityEngine;

namespace BfgAnalytics
{
    public static class Analytics
    {
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
            AnalyticsService.Current?.Event("progress", questId, "quest", "start", DefaultJsonData());
        }
        
        public static void SendQuestCompletedEvent(string questId)
        {
            AnalyticsService.Current?.Event("progress", questId, "quest", "end", DefaultJsonData());
        }
        
        public static void SendPieceUnlockedEvent(string pieceId)
        {
            AnalyticsService.Current?.Event("progress", pieceId, "piece", "unlock", DefaultJsonData());
        }
        
        public static void SendCharUnlockedEvent(string charId)
        {
            AnalyticsService.Current?.Event("progress", charId, "character", "unlock", DefaultJsonData());
        }
        
        public static void SendFogClearedEvent(string fogId)
        {
            AnalyticsService.Current?.Event("progress", fogId, "fog", "unlock", DefaultJsonData());
        }
        
        public static void SendLevelReachedEvent(int level)
        {
            AnalyticsService.Current?.Event("progress", level.ToString(), "level",null, DefaultJsonData());
        }

		public static void SendTutorialStartStepEvent(string stepName)
        {   
            JSONObject customJsonData = new JSONObject();
            JSONObject ctrNode = new JSONObject();

            customJsonData["ctr"] = ctrNode;

            ctrNode["seconds"] = 0;
            
            AnalyticsService.Current?.Event("ftue", stepName, "ftue", "start", TutorialJsonData(), customJsonData);
        }

        public static void SendTutorialEndStepEvent(string stepName, DateTime startTime)
        {
            JSONObject customJsonData = new JSONObject();
            JSONObject ctrNode = new JSONObject();

            customJsonData["ctr"] = ctrNode;

            var seconds = (DateTime.UtcNow - startTime).TotalSeconds;
            ctrNode["seconds"] = (int)Math.Round(seconds);
            
            AnalyticsService.Current?.Event("ftue", stepName, "ftue", "end", TutorialJsonData(), customJsonData);
        }
        
        public static void SendPurchase(string location, string reason, List<CurrencyPair> spend, List<CurrencyPair> collect, bool isIap, bool isFree)
        {
            JSONObject customJsonData = CreateTransaction(location, reason, spend, collect, isIap, isFree);

            AnalyticsService.Current?.Event("economy", "economy",null, null, DefaultJsonData(), customJsonData);
        }
        
        public static void SendDailyRewardClaim(int day)
        {
            AnalyticsService.Current?.Event("activity", day.ToString(), "dailyreward",null, DefaultJsonData());
        }

        private static JSONObject CreateTransaction(string location, string reason, List<CurrencyPair> spend, List<CurrencyPair> collect, bool isIap, bool isFree)
        {
            JSONObject customJsonData = new JSONObject();
            JSONObject transactionNode = new JSONObject();
            
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
        private static JSONObject CreateCurrenciesJson(List<CurrencyPair> currencies)
        {
            JSONObject CreateCurrencyNode(CurrencyPair pair)
            {
                JSONObject node = new JSONObject();
                node[pair.Currency] = new JSONNumber(pair.Amount);
                return node;
            }
            
            var ret = new JSONObject();
            var premium = new JSONObject();
            var soft = new JSONObject();

            foreach (var pair in currencies)
            {
                if (pair == null) continue;
                
                if (pair.Currency == Currency.Crystals.Name)
                {
                    premium[pair.Currency] = pair.Amount;
                    continue;
                }

                if (pair.Currency == Currency.Coins.Name)
                {
                    soft[pair.Currency] = pair.Amount;
                    continue;
                }
            }

            if (premium.Count > 0) ret.Add("premium", premium);
            if (soft.Count > 0) ret.Add("soft", soft);

            return ret;
        }
    }
}