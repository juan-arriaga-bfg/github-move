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
        LoadData(new ResourceConfigDataMapper<List<CharacterDef>>("configs/characters.data", NSConfigsSettings.Instance.IsUseEncryption));
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
                
                for (var i = Characters.Count - 1; i >= 0; i--)
                {
                    var id = Characters[i];
                    
                    if (GameDataService.Current.CodexManager.IsPieceUnlocked(id) == false) continue;

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
        if (Characters.Contains(id) == false) return;
        
        Characters.Remove(id);
        UpdateSequence();
        GetSequence(Currency.Character.Name).Reinit(CharactersWeights);
        GetSequence(ChestKey).Reinit(CharactersChestWeights);
        
        OnUpdateSequence?.Invoke();
        GameDataService.Current.FogsManager.UpdateFogObserver(id);
    }

    private void UpdateSequence()
    {
        var amount = Mathf.Min(3, Characters.Count);
        
        CharactersWeights = new List<ItemWeight>();
        CharactersChestWeights = new List<ItemWeight>();
        
        if (amount == 0)
        {
            CharactersWeights.Add(new ItemWeight{Uid = ReplacePiece.Abbreviations[0], Weight = 10});
            CharactersChestWeights.Add(new ItemWeight{Uid = ReplacePiece.Abbreviations[0], Weight = 10});
            GameDataService.Current.MarketManager.Defs.Find(item => item.Current?.RandomType == MarketRandomType.NPCChests)?.Update(true);
            return;
        }
        
        for (var i = 0; i < defs.Count; i++)
        {
            var def = defs[i];
            var isReplace = i < amount;
            var chain = isReplace ? GameDataService.Current.MatchDefinition.GetChain(Characters[i]) : new List<int>();

            UpdateCurrentSequence(def.PieceWeights, CharactersWeights, isReplace, chain);
            UpdateCurrentSequence(def.ChestWeights, CharactersChestWeights, isReplace, chain);
        }
    }

    private void UpdateCurrentSequence(List<ItemWeight> target, List<ItemWeight> current, bool isReplace, List<int> chain)
    {
        foreach (var item in target)
        {
            var uid = isReplace ? PieceType.Parse(chain[int.Parse(item.Uid) - 1]) : ReplacePiece.Abbreviations[0];
            current.Add(new ItemWeight{Uid = uid, Weight = item.Weight});
        }
    }
}
