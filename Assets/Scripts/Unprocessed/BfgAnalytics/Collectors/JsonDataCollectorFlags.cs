using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorFlags : IJsonDataCollector
    {
        public string Name => "flags";

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
            foreach (var fog in fogManager.Fogs)
            {
                if (!fogManager.IsFogCleared(fog.Uid))
                    return 0;
            }

            return 1;
        }
    }
}