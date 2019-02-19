using System.Collections.Generic;
using UnityEngine;

public class CharactersDataManager : SequenceData, IDataLoader<List<CharacterDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private List<int> characters;
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
                characters = PieceType.GetIdsByFilter(PieceTypeFilter.Character);
                defs = data;
                
                for (var i = characters.Count - 1; i >= 0; i--)
                {
                    var id = characters[i];
                    
                    if (GameDataService.Current.CodexManager.IsPieceUnlocked(id) == false) continue;

                    characters.RemoveAt(i);
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
        characters.Remove(id);
        UpdateSequence();
        GetSequence(Currency.Character.Name).Reinit(CharactersWeights);
    }

    private void UpdateSequence()
    {
        var amount = Mathf.Min(3, characters.Count);
        
        CharactersWeights = new List<ItemWeight>();
        
        if (amount == 0)
        {
            CharactersWeights.Add(new ItemWeight{Uid = PieceType.Hard1.Abbreviations[0], Weight = 10});
            return;
        }
        
        if (amount < defs.Count) defs.RemoveRange(amount - 1, defs.Count - amount);
        
        for (var i = 0; i < defs.Count; i++)
        {
            var def = defs[i];
            var chain = GameDataService.Current.MatchDefinition.GetChain(characters[i]);

            foreach (var item in def.PieceWeights)
            {
                var uid = PieceType.Parse(chain[int.Parse(item.Uid) - 1]);
                CharactersWeights.Add(new ItemWeight{Uid = uid, Weight = item.Weight});
            }
        }
    }
}
