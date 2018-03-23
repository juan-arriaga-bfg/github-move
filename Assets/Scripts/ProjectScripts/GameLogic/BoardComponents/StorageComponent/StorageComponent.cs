using UnityEngine;

public class StorageComponent : IECSComponent, ITimerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }

    public int SpawnPiece;
    public int Capacity;
    public int Filling;

    public TimerComponent Timer { get; private set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        var piece = entity as Piece;
        
        Timer = piece.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        
        if(Timer == null) return;

        Timer.OnComplete += Update;
        if(Filling < Capacity) Timer.Start();
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
        if(Timer == null) return;
        
        Timer.OnComplete -= Update;
    }

    private void Update()
    {
        Filling = Mathf.Min(Filling + 1, Capacity);
        
        if(Filling < Capacity) Timer.Start();
    }
    
    public bool Scatter(out int amount)
    {
        amount = Filling;
        
        if (Filling == 0) return false;

        Filling = 0;
        Timer.Start();
        
        return true;
    }
}