using System;
using System.Linq;
using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorStory : IJsonDataCollector
    {
        public string Name => "story";

        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();
            
            node["completed_count"] = GameDataService.Current.QuestsManager.FinishedQuests.Count(elem => !(elem is DailyQuestEntity));
            node["active"] = GetActiveQuests();

            return node;
        }

        private JSONArray GetActiveQuests()
        {
            JSONArray arr = new JSONArray();
            var activeQuests = GameDataService.Current.QuestsManager.ActiveStoryQuests;
            foreach (var quest in activeQuests)
            {
                arr.Add(quest.Id);    
            }

            return arr;
        }
    }
}