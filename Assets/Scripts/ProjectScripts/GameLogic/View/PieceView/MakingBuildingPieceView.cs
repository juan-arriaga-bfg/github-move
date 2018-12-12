using System.Linq;

public class MakingBuildingPieceView : BuildingPieceView
{
    private TimerComponent timer;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        var life = Piece.GetComponent<MakingLifeComponent>(MakingLifeComponent.ComponentGuid);

        timer = life?.Timer;
        
        if(timer == null) return;
        
        timer.OnStart += UpdateLockSate;
        timer.OnComplete += UpdateLockSate;

        UpdateLockSate();
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        if(timer == null) return;
        
        timer.OnStart -= UpdateLockSate;
        timer.OnComplete -= UpdateLockSate;
    }
    
    private void UpdateLockSate()
    {
        if(timer == null || bodySprites == null) return;

        ToggleLockedState(timer.IsStarted);
    }
}