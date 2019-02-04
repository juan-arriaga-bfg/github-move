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
        private static MatchDefinitionComponent matchDefinition = new MatchDefinitionComponent(new MatchDefinitionBuilder().Build());
        public string Name => "userstats";

        public JSONNode CollectData()
        {
            JSONNode node = new JSONObject();

            Func<CurrencyDef, int> getValueByCurrency =
                (currency) => ProfileService.Current.GetStorageItem(currency.Name).Amount;

            var creationDate = ProfileService.Current.GetComponent<BaseInformationSaveComponent>(BaseInformationSaveComponent.ComponentGuid).CreationDateTime;
            node["profile_creation_date"] = $"{creationDate.Month}/{creationDate.Day}/{creationDate.Year}";
            node["last_fog"] = GameDataService.Current.FogsManager.LastOpenFog?.Uid ?? "";
            node["level"] = getValueByCurrency(Currency.Level);
            node["level_progress"] = GetLevelProgress();
            node["energy_cap"] = getValueByCurrency(Currency.EnergyLimit);
            node["workers"] = getValueByCurrency(Currency.WorkerLimit);
            node["workers_available"] = getValueByCurrency(Currency.Worker);
            node["top_pieces"] = GetTopPiecesInformation();
            
            return node;
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
            JSONNode node = new JSONObject();
            var codex = GameDataService.Current.CodexManager;
            var firstInChain = PieceType.GetIdsByFilter(PieceTypeFilter.Normal).Select(id => matchDefinition.GetFirst(id)).Distinct();

            foreach (var item in firstInChain)
            {
                CodexChainState chainState; 
                if(codex.GetChainState(item, out chainState) == false) continue;

                var chain = matchDefinition.GetChain(item);
                var maxUnlockedResult = -1;
                for (var i = chain.Count - 1; i >= 0; i--)
                {
                    var currentMaxElement = chain[i];
                    if (chainState.Unlocked.Contains(currentMaxElement) ||
                        chainState.PendingReward.Contains(currentMaxElement))
                    {
                        maxUnlockedResult = currentMaxElement;
                        break;
                    }
                }
                
                if (maxUnlockedResult == -1) continue;
                
                var itemDef = PieceType.GetDefById(maxUnlockedResult);
                var pieceName = itemDef.Abbreviations[0];
                var chainName = Regex.Match(pieceName, @"^([A-Z]+)\d+").Groups[1].Value;
                node[chainName] = pieceName;
            }

            return node;
        }
    }
}