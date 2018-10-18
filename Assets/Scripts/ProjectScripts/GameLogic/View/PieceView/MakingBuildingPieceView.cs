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

        if (bodySprite != null) unlockedMaterial = bodySprite.material;
        UpdateLockSate();
    }
    
    public override void ResetViewOnDestroy()
    {
        bodySprite.material = unlockedMaterial;
        base.ResetViewOnDestroy();
        
        if(timer == null) return;
        
        timer.OnStart -= UpdateLockSate;
        timer.OnComplete -= UpdateLockSate;
    }
    
    private void UpdateLockSate()
    {
        if(timer == null || bodySprite == null) return;
        
        bodySprite.material = timer.IsStarted ? lockedMaterial : unlockedMaterial;
    }
}