using System;
using UnityEngine;

public class StorageComponent : IECSComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }

    public int SpawnPiece;
    public int Capacity;
    public int Delay;

    private DateTime startTime;
    private int filling;
    
    public int Filling
    {
        get { return filling; }
    }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        filling = 0;
        startTime = DateTime.Now;
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public bool IsExecuteable()
    {
        return SpawnPiece != PieceType.None.Id && Capacity > 0 && filling < Capacity;
    }

    public void Execute()
    {
        var time = DateTime.Now - startTime;
        
        Debug.LogError("!!!!!!!!! " + filling);
        
        if(time.TotalSeconds < Delay) return;
        
        startTime = DateTime.Now;
        filling = Mathf.Min(filling + 10, Capacity);
    }

    public bool IsPersistence
    {
        get { return false; }
    }
    
    public bool Scatter(out int piece, out int amount)
    {
        piece = SpawnPiece;
        amount = filling;
        
        if (filling == 0) return false;

        filling = 0;
        startTime = DateTime.Now;
        
        return true;
    }
}