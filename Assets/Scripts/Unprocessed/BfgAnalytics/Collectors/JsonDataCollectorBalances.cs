using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorBalances : IJsonDataCollector
    {
        public string Name => "balances";

        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();
            node["soft"] = GetSoft();
            node["premium"] = GetPremium();
            return node;
        }

        private JSONNode GetSoft()
        {
            JSONNode node = new JSONObject();
            node["coins"] = (int)ProfileService.Current.GetStorageItem(Currency.Coins.Name).Amount;
            node["energy"] = (int)ProfileService.Current.GetStorageItem(Currency.Energy.Name).Amount;
            node["mana"] = (int)ProfileService.Current.GetStorageItem(Currency.ManaFake.Name).Amount;
            return node;
        }

        private JSONNode GetPremium()
        {
            JSONNode node = new JSONObject();
            node["crystals"] = (int)ProfileService.Current.GetStorageItem(Currency.Crystals.Name).Amount;
            return node;
        }
    }
}