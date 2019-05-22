using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharactersDataManager : SequenceData, IDataLoader<List<CharacterDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public static readonly PieceTypeDef ReplacePiece = PieceType.Soft1;

    public List<int> Characters;
    private List<CharacterDef> defs;
    public List<ItemWeight> CharactersWeights;
    public List<ItemWeight> CharactersChestWeights;

    public Action OnUpdateSequence;

    public string ChestKey => $"{Currency.Character.Name}_{PieceType.CH_NPC.Abbreviations[0]}";
    
    public override void Reload()
    {
        base.Reload();
        LoadData(new HybridConfigDataMapper<List<CharacterDef>>("configs/characters.data", NSConfigsSettings.Instance.IsUseEncryption));
    }
    
    public void LoadData(IDataMapper<List<CharacterDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Characters = PieceType.GetIdsByFilter(PieceTypeFilter.Character, PieceTypeFilter.Fake);
                Characters.Remove(PieceType.NPC_A.Id);
                
                defs = data;
                
                var dataManager = (GameDataManager) context;
                
                for (var i = Characters.Count - 1; i >= 0; i--)
                {
                    var id = Characters[i];
                    
                    if (dataManager.CodexManager.IsPieceUnlocked(id) == false) continue;

                    Characters.RemoveAt(i);
                }
                
                UpdateSequence();
                AddSequence(Currency.Character.Name, CharactersWeights);
                AddSequence(ChestKey, CharactersChestWeights);
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
    
    public void UnlockNewCharacter(int id)
    {
        if (id != PieceType.Boost_WR.Id && Characters.Contains(id) == false) return;
        
        Characters.Remove(id);
        UpdateSequence();
        GetSequence(Currency.Character.Name).Reinit(CharactersWeights);
        GetSequence(ChestKey).Reinit(CharactersChestWeights);
        
        OnUpdateSequence?.Invoke();
        GameDataService.Current.FogsManager.UpdateFogObserver(id);
        GameDataService.Current.FogsManager.UpdateUnlockedStates();
    }

    private void UpdateSequence()
    {
        var amount = Mathf.Min(3, Characters.Count);
        var dataManager = (GameDataManager) context;
        var ignoreSecond = amount > 1 && Characters[1] - Characters[0] != PieceType.NPC_B.Id - PieceType.NPC_A.Id;
        
        CharactersWeights = new List<ItemWeight>();
        CharactersChestWeights = new List<ItemWeight>();
        
        for (var i = 0; i < defs.Count; i++)
        {
            var def = defs[i];
            var isNotReplace = i < amount;
            var chain = isNotReplace ? dataManager.MatchDefinition.GetChain(Characters[i]) : new List<int>();

            var pieceWeights = i > 0 && ignoreSecond
                ? def.PieceWeights.FindAll(weight => int.TryParse(weight.Uid, out _) == false)
                : def.PieceWeights;
            
            var chestWeights = i > 0 && ignoreSecond
                ? def.ChestWeights.FindAll(weight => int.TryParse(weight.Uid, out _) == false)
                : def.ChestWeights;

            UpdateCurrentSequence(pieceWeights, CharactersWeights, isNotReplace, chain);
            UpdateCurrentSequence(chestWeights, CharactersChestWeights, isNotReplace, chain);
        }
        
        if (amount == 0) dataManager.MarketManager.Defs.Find(item => item.Current?.RandomType == MarketRandomType.NPCChests)?.Update(true);
    }

    private void UpdateCurrentSequence(List<ItemWeight> target, List<ItemWeight> current, bool isNotReplace, List<int> chain)
    {
        var dataManager = (GameDataManager) context;
        var weights = dataManager.TutorialDataManager.CheckUnlockWorker() == false
            ? target.FindAll(weight => dataManager.MatchDefinition.GetLast(weight.Piece) != PieceType.Boost_WR.Id)
            : target;
        
        foreach (var item in weights)
        {
            var uid = int.TryParse(item.Uid, out var index) ? (isNotReplace ? PieceType.Parse(chain[index - 1]) : ReplacePiece.Abbreviations[0]) : item.Uid;
            
            current.Add(new ItemWeight{Uid = uid, Weight = item.Weight});
        }
    }
}