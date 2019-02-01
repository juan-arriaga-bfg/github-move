using System.Collections.Generic;
using IW.SimpleJSON;

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
            return JsonDataGroup.Standart | JsonDataGroup.Userstats | JsonDataGroup.Balances | JsonDataGroup.Flags | JsonDataGroup.Story;
        }
        
        /// <summary>
        /// Create json like this: {"premium": {"crystals": 10}, "soft": {"energy": 50, "coins": 500}}
        /// </summary>
        /// <param name="premium"></param>
        /// <param name="soft"></param>
        /// <returns></returns>
        public static JSONNode CreateCurrenciesJson(List<CurrencyPair> premium, List<CurrencyPair> soft)
        {
            JSONNode CreateCurrencyNode(CurrencyPair pair)
            {
                JSONNode node = new JSONObject();
                node[pair.Currency] = new JSONNumber(pair.Amount);
                return node;
            }
            
            JSONObject ret = new JSONObject();

            if (premium != null && premium.Count > 0)
            {
                var premiumNode = new JSONArray();
                foreach (var currencyPair in premium)
                {
                    premiumNode.Add(CreateCurrencyNode(currencyPair)); 
                }

                ret.Add("premium", premiumNode);
            }

            if (soft != null && soft.Count > 0)
            {
                var softNode = new JSONArray();
                foreach (var currencyPair in soft)
                {
                    softNode.Add(CreateCurrencyNode(currencyPair));
                }
                ret.Add("soft", softNode);
            }

            return ret;
        }

        public static void SendQuestStartEvent(string questId)
        {
            AnalyticsService.Current?.Event("progress", "quest", questId, "start", DefaultJsonData());
        }
        
        public static void SendQuestCompletedEvent(string id)
        {
            JSONNode transaction = new JSONObject();
            AnalyticsService.Current?.Event("progress", "quest", id, "end", DefaultJsonData(), transaction);
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
            JSONNode transaction = new JSONObject();
            AnalyticsService.Current?.Event("progress", "fog", id, "unlock", DefaultJsonData(), transaction);
        }
        
        public static void SendLevelReachedEvent(int level)
        {
            JSONNode transaction = new JSONObject();
            AnalyticsService.Current?.Event("progress", "level", level.ToString(), null, DefaultJsonData(), transaction);
        }        
        
        public static void SendPurchase()
        {
            JSONNode customJsonData = new JSONObject();

            JSONNode transactionNode = new JSONObject();
            transactionNode["location"] = "shop";
            transactionNode["reason"] = "item1";
            transactionNode["isiap"] = false;
            transactionNode["isfree"] = false;

            transactionNode["spend"] = CreateCurrenciesJson(
                new List<CurrencyPair>
                {
                    new CurrencyPair {Currency = Currency.Cash.Name, Amount = 1},
                },
                null
            );
            
            transactionNode["collect"] = CreateCurrenciesJson(
                new List<CurrencyPair>
                {
                    new CurrencyPair {Currency = Currency.Crystals.Name, Amount = 1},
                },
                new List<CurrencyPair>
                {
                    new CurrencyPair {Currency = Currency.Energy.Name, Amount = 2},
                    new CurrencyPair {Currency = Currency.Coins.Name, Amount = 3},
                }
            );
            
            customJsonData.Add("transaction", transactionNode);

            // todo: Договориться насчет type, name, action
            AnalyticsService.Current?.Event("economy", null, null, null, DefaultJsonData(), customJsonData);
        }
    }
}