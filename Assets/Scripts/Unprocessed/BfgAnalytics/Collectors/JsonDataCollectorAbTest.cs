using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorAbTest : IJsonDataCollector
    {
        public string Name => "abtest";

        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();

            var manager = GameDataService.Current.AbTestManager;
            
            if (manager.Tests == null || manager.Tests.Count == 0)
            {
                // No test - no data!
            }
            else
            {
                foreach (var test in manager.Tests.Values)
                {
                    node[test.TestName] = test.UserGroup;
                }
            }

            return node;
        }
    }
}