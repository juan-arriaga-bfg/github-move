using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IW.SimpleJSON;

namespace BfgAnalytics
{
    public class JsonDataCollectorLocation : IJsonDataCollector
    {
        public string Name => "location";
        
        private JSONObject cachedNode;
        private int lastInstanceId = -1;
        private MatchDefinitionComponent matchDefinition;
        private List<int> firstInChains;
        private Dictionary<int, string> chainNames;
        private Regex chainNameRegex;

        public JsonDataCollectorLocation()
        {
            matchDefinition = new MatchDefinitionComponent(new MatchDefinitionBuilder().Build());
            firstInChains = PieceType.GetIdsByFilter(PieceTypeFilter.Analytics)
                .Select(id => matchDefinition.GetFirst(id)).Distinct().ToList();
            
            chainNames = new Dictionary<int, string>();
            chainNameRegex = new Regex(@"^([A-Za-z_]+)\d*");
        }
        
        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();

            JSONNode world = new JSONObject();
            node["world"] = world;

            world["name"] = GetWorldName();
            world["class"] = GetClassName();
            world["type"] = GetTypeName();

            JSONNode closeness = GetCloseness();
            world["closeness"] = closeness;

            return node;
        }

        private string GetWorldName()
        {
            return "kingdom";
        }

        private string GetClassName()
        {
            return "main";
        }

        private string GetTypeName()
        {
            return "main";
        }

        private JSONObject GetCloseness()
        {
            var codex = GameDataService.Current.CodexManager;
            var codexContent = codex.GetCodexContent();
            if (lastInstanceId == codexContent.InstanceId)
                return cachedNode;
            
            JSONObject node = new JSONObject();
            lastInstanceId = codexContent.InstanceId;
            
            foreach (var item in firstInChains)
            {
                if(codex.GetChainState(item, out var chainState) == false) continue;

                var chain = matchDefinition.GetChain(item);
                var countUnlocked = chainState.Unlocked.Count;
                var countDenom = chain.Count;
                
                var itemDef = PieceType.GetDefById(item);
                var chainName = GetChainName(itemDef);
                
                var chainData = new JSONObject();
                chainData["num"] = countUnlocked;
                chainData["denom"] = countDenom;
                
                node[chainName] = chainData;
            }

            cachedNode = node;
            return node;
        }
        
        private string GetChainName(PieceTypeDef pieceDef)
        {
            if (chainNames.ContainsKey(pieceDef.Id))
                return chainNames[pieceDef.Id];

            var pieceName = pieceDef.Abbreviations[0];
            var chainName = chainNameRegex.Match(pieceName).Groups[1].Value;
            
            chainNames[pieceDef.Id] = chainName;
            return chainName;
        }
    }
}