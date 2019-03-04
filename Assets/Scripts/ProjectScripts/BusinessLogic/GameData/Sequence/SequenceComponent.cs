using System.Collections.Generic;
using UnityEngine;

public class SequenceComponent : IECSComponent
{
    private const int Range = 10000;
    
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public string Key;
    
    private int seed = -1;
    
    private List<byte> sequence;
    private List<ItemWeight> weights;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public ECSEntity Context;
    
    public void Init(List<ItemWeight> value)
    {
        weights = value ?? new List<ItemWeight>();
        
        var save = ((GameDataManager)Context).UserProfile.GetComponent<SequenceSaveComponent>(SequenceSaveComponent.ComponentGuid)?.GetSave(Key);
        
        seed = save?.Seed ?? Random.Range(0, Range);
        sequence = ItemWeight.GetRandomSequence(weights, seed);
        
        if(save == null) return;
        
        sequence.RemoveRange(0, sequence.Count - save.Count);
    }

    public void Reinit(List<ItemWeight> value)
    {
        weights = value ?? new List<ItemWeight>();
        seed = Random.Range(0, Range);
        sequence = ItemWeight.GetRandomSequence(weights, seed);
    }

    private bool IsValid()
    {
        return weights != null && weights.Count > 0;
    }
    
    public SequenceSaveItem Save()
    {
        return new SequenceSaveItem{Uid = Key, Seed = seed, Count = sequence.Count};
    }

    private void UpdateSequence()
    {
        seed++;
        sequence = weights.Count == 0 ? new List<byte>() : ItemWeight.GetRandomSequence(weights, seed);
    }
    
    private ItemWeight Next()
    {
        if (sequence.Count == 0) UpdateSequence();

        var index = sequence[0];
        
        sequence.RemoveAt(0);
        
        return weights[index];
    }

    public ItemWeight GetNext()
    {
        return IsValid() == false ? null : Next().Copy();
    }

    public List<int> GetNextList(int amount, List<int> list = null)
    {
        var result = list ?? new List<int>();

        if (IsValid() == false) return result;

        for (var i = 0; i < amount; i++)
        {
            result.Add(Next().Piece);
        }

        return result;
    }
    
    public Dictionary<int, int> GetNextDict(int amount, Dictionary<int, int> dict = null)
    {
        var result = dict ?? new Dictionary<int, int>();
        
        if (IsValid() == false) return result;

        for (var i = 0; i < amount; i++)
        {
            var id = Next().Piece;
            
            if (result.ContainsKey(id) == false) result.Add(id, 0);

            result[id]++;
        }

        return result;
    }
}