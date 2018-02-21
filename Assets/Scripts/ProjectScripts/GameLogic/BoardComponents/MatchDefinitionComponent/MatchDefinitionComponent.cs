using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchDefinitionComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid { get { return ComponentGuid; } }

    private Dictionary<int, int> definition;
    
    public MatchDefinitionComponent(Dictionary<int, int> def)
    {
        definition = def;
    }
    
    public int GetNext(int current)
    {
        int next;
        
        return definition.TryGetValue(current, out next) ? next : PieceType.None.Id;
    }

    public List<int> GetChain(int current)
    {
        var chain = new List<int>();
        var next = (current / 100) * 100;

        if (next == current) return new List<int> {current};
        
        do
        {
            chain.Add(next);
            next = GetNext(next);
        }
        while (next != PieceType.None.Id);
        
        chain.Add(current);
        
        return chain[0] == PieceType.None.Id ? new List<int> {current} : chain;
    }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity) {}
}