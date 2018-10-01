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

        if (sprite != null) unlockedMaterial = sprite.material;
        UpdateLockSate();
    }
    
    public override void ResetViewOnDestroy()
    {
        sprite.material = unlockedMaterial;
        base.ResetViewOnDestroy();
        
        if(timer == null) return;
        
        timer.OnStart -= UpdateLockSate;
        timer.OnComplete -= UpdateLockSate;
    }
    
    private void UpdateLockSate()
    {
        if(timer == null || sprite == null) return;
        
        sprite.material = timer.IsStarted ? lockedMaterial : unlockedMaterial;
    }
}