using UnityEngine;

public class StorageComponent : IECSComponent, ITimerComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid { get { return ComponentGuid; } }

    public int SpawnPiece;
    
    public int Amount;
    public int Capacity;
    public int Filling;
    
    private ViewDefinitionComponent viewDef;
    
    public TimerComponent Timer { get; private set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        var piece = entity as Piece;
        
        Timer = piece.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        viewDef = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(Timer == null) return;
        
        Timer.OnComplete += Update;
        if(Filling != Capacity) Timer.Start();
        
        UpdateView();
    }

    public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        if(Timer == null) return;
        
        Timer.Stop();
        Timer.OnComplete -= Update;
    }

    private void Update()
    {
        Filling = Mathf.Min(Filling + Amount, Capacity);
        
        if(Filling < Capacity) Timer.Start();

        UpdateView();
    }

    private void UpdateView()
    {
        if(viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.StorageState);
        
        view.Change(Filling / (float) Capacity > 0.2f);
    }

    public bool Scatter(out int amount)
    {
        amount = Filling;
        
        if (Filling == 0) return false;
        
        Filling = 0;
        Timer.Start();
        UpdateView();
        
        return true;
    }
}