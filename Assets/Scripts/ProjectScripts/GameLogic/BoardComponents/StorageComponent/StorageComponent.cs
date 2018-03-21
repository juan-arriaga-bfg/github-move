using UnityEngine;

public class StorageComponent : IECSComponent, ITimerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }

    public int SpawnPiece;
    public int Capacity;
    
    private int filling;
    
    public int Filling
    {
        get { return filling; }
    }
    
    public TimerComponent Timer { get; private set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        var piece = entity as Piece;
        
        filling = 0;
        Timer = piece.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        if(Timer == null) return;

        Timer.OnComplete += Update;
        Timer.Start();
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
        if(Timer == null) return;
        
        Timer.OnComplete -= Update;
    }

    private void Update()
    {
        filling = Mathf.Min(filling + 1, Capacity);
        
        if(filling < Capacity) Timer.Start();
    }
    
    public bool Scatter(out int amount)
    {
        amount = filling;
        
        if (filling == 0) return false;

        filling = 0;
        Timer.Start();
        
        return true;
    }
}