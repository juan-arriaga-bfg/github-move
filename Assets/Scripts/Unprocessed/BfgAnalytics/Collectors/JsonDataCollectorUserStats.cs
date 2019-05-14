using System;
using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorUserStats : IJsonDataCollector
    {
        private JSONNode cachedNode;
        public string Name => "userstats";

        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();

            var profileService = ProfileService.Current;
            
            DateTime creationDate = UnixTimeHelper.UnixTimestampToDateTime(profileService.BaseInformation.CreationTimestamp);
            node["profile_creation_date"] = $"{creationDate:MM-dd-yy}";
            node["last_fog"] = GameDataService.Current.FogsManager.LastOpenFog?.Uid ?? "";
            node["level"] = GetValueByCurrency(Currency.Level);
            node["level_progress"] = GetLevelProgress();
            node["energy_cap"] = GetValueByCurrency(Currency.EnergyLimit);
            node["workers"] = GetValueByCurrency(Currency.WorkerLimit);
            node["workers_available"] = GetValueByCurrency(Currency.Worker);
            node["orders_count"] = GetValueByCurrency(Currency.Order);
            node["daily_obj_count"] = GameDataService.Current.QuestsManager?.DailyQuestCompletedCount ?? 0;
            node["effectiveness"] = profileService.BaseInformation.MatchesCounter.Effectiveness;
            
            return node;
        }

        private int GetValueByCurrency(CurrencyDef currency)
        {
            return ProfileService.Current.GetStorageItem(currency.Name).Amount;
        }

        public static double GetLevelProgress()
        {
            var currentLevel = GameDataService.Current.LevelsManager.Level;
            
            if (currentLevel > GameDataService.Current.LevelsManager.Levels.Count)
                return 0;
            var levelDef = GameDataService.Current.LevelsManager.Levels[currentLevel-1];
            
            var currentExperience = ProfileService.Current.GetStorageItem(Currency.Experience.Name).Amount;
            var nextLevelCost = levelDef.Price.Amount;
            double result = currentExperience / (double)nextLevelCost;
            return Math.Round(result, 2);
        }
    }
}