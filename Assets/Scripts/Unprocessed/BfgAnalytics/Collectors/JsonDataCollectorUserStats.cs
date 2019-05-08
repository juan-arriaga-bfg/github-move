using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IW.SimpleJSON;
using UnityEngine;

namespace BfgAnalytics
{
    public class JsonDataCollectorUserStats : IJsonDataCollector
    {
        private MatchDefinitionComponent matchDefinition;
        private List<int> firstInChains;
        private Dictionary<int, string> chainNames;
        private Regex chainNameRegex;
        private int lastInstanceId = -1;
        private JSONNode cachedNode;
        public string Name => "userstats";

        public JsonDataCollectorUserStats()
        {
            matchDefinition = new MatchDefinitionComponent(new MatchDefinitionBuilder().Build());
            firstInChains = PieceType.GetIdsByFilter(PieceTypeFilter.Progress)
                                     .Select(id => matchDefinition.GetFirst(id)).Distinct().ToList();
            
            chainNames = new Dictionary<int, string>();
            chainNameRegex = new Regex(@"^([A-Za-z_]+)\d*");
        }

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
            node["top_pieces"] = GetTopPiecesInformation();
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

        private JSONNode GetTopPiecesInformation()
        {
            var codex = GameDataService.Current.CodexManager;
            var codexContent = codex.GetCodexContent();
            if (lastInstanceId == codexContent.InstanceId)
                return cachedNode;
            
            JSONNode node = new JSONObject();
            lastInstanceId = codexContent.InstanceId;
            
            foreach (var item in firstInChains)
            {
                CodexChainState chainState; 
                if(codex.GetChainState(item, out chainState) == false) continue;

                var chain = matchDefinition.GetChain(item);
                var maxUnlockedResult = -1;
                for (var i = chain.Count - 1; i >= 0; i--)
                {
                    var currentMaxElement = chain[i];
                    if (chainState.Unlocked.Contains(currentMaxElement))
                    {
                        maxUnlockedResult = currentMaxElement;
                        break;
                    }
                }
                
                if (maxUnlockedResult == -1) continue;
                
                var itemDef = PieceType.GetDefById(maxUnlockedResult);
                var pieceName = itemDef.Abbreviations[0];
                var chainName = GetChainName(itemDef);
                node[chainName] = pieceName;
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