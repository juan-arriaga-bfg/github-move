using System.Collections.Generic;
using UnityEngine;

public class SequenceComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public string Key;
    
    private int seed = -1;
    
    private List<int> sequence;
    public List<ItemWeight> Weights;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void Init(List<ItemWeight> weights)
    {
        if(weights != null) Weights = weights;
        
        var save = ProfileService.Current.GetComponent<RandomSaveComponent>(RandomSaveComponent.ComponentGuid)?.GetSave(Key);

        seed = save?.Seed ?? Random.Range(0, 10000);
        sequence = ItemWeight.GetRandomSequence(Weights, 100, seed);
        
        if(save == null) return;
        
        if (save.Count == 0)
        {
            UpdateSequence();
            return;
        }
        
        sequence.RemoveRange(0, sequence.Count - save.Count);
    }

    public void Reinit(List<ItemWeight> weights)
    {
        Weights = weights;
        seed = Random.Range(0, 10000);
        sequence = ItemWeight.GetRandomSequence(Weights, 100, seed);
    }

    private bool IsValid()
    {
        return Weights != null && Weights.Count > 0;
    }
    
    public RandomSaveItem Save()
    {
        return new RandomSaveItem{Uid = Key, Seed = seed, Count = sequence.Count};
    }
    
    public void UpdateSequence()
    {
        seed++;
        sequence = Weights.Count == 0 ? new List<int>() : ItemWeight.GetRandomSequence(Weights, 100, seed);
    }
    
    private ItemWeight Next()
    {
        if (sequence.Count == 0) UpdateSequence();

        var index = sequence[0];
        
        sequence.RemoveAt(0);
        
        return Weights[index];
    }

    public ItemWeight GetNext()
    {
        return IsValid() == false ? null : Next().Copy();
    }

    public List<int> GetNextList(int amount)
    {
        var result = new List<int>();

        if (IsValid() == false) return result;

        for (var i = 0; i < amount; i++)
        {
            result.Add(Next().Piece);
        }

        return result;
    }

    public Dictionary<int, int> GetNextDict(int amount)
    {
        var result = new Dictionary<int, int>();
        
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