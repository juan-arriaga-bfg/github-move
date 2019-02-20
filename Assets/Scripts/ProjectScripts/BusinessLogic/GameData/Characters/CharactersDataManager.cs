using System.Collections.Generic;
using UnityEngine;

public class CharactersDataManager : SequenceData, IDataLoader<List<CharacterDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public List<int> Characters;
    private List<CharacterDef> defs;
    public List<ItemWeight> CharactersWeights;
    
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
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
    
    public void UnlockNewCharacter(int id)
    {
        Characters.Remove(id);
        UpdateSequence();
        GetSequence(Currency.Character.Name).Reinit(CharactersWeights);
    }

    private void UpdateSequence()
    {
        var amount = Mathf.Min(3, Characters.Count);
        
        CharactersWeights = new List<ItemWeight>();
        
        if (amount == 0)
        {
            CharactersWeights.Add(new ItemWeight{Uid = PieceType.Hard1.Abbreviations[0], Weight = 10});
            return;
        }
        
        for (var i = 0; i < defs.Count; i++)
        {
            var def = defs[i];
            var chain = i < amount ? GameDataService.Current.MatchDefinition.GetChain(Characters[i]) : new List<int>();

            foreach (var item in def.PieceWeights)
            {
                var uid = i < amount ? PieceType.Parse(chain[int.Parse(item.Uid) - 1]) : PieceType.Hard1.Abbreviations[0];
                CharactersWeights.Add(new ItemWeight{Uid = uid, Weight = item.Weight});
            }
        }
    }
}
