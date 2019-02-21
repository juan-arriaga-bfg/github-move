using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorFlags : IJsonDataCollector
    {
        public string Name => "flags";

        private string lastOpenFog;
        private int eocCachedValue;
        
        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();
            
            node["payer"] = ProfileService.Current.GetComponent<BaseInformationSaveComponent>(BaseInformationSaveComponent.ComponentGuid).IsPayer ? 1 : 0;
            node["eoc"] = CheckEndOfContent();
            node["cheating"] = 0;

            return node;
        }

        private int CheckEndOfContent()
        {
            var fogManager = GameDataService.Current.FogsManager;
            var lastuid = fogManager.LastOpenFog?.Uid;
            if (lastuid == lastOpenFog) return eocCachedValue;

            lastOpenFog = lastuid;
            
            foreach (var fog in fogManager.Fogs)
            {
                if (fog.IsActive && fogManager.IsFogCleared(fog.Uid) == false)
                {
                    eocCachedValue = 0;
                    return eocCachedValue;
                }           
            }

            eocCachedValue = 1;
            return eocCachedValue;
        }
    }
}