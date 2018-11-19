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

        if (bodySprites != null) unlockedMaterial = bodySprites.First().material;
        UpdateLockSate();
    }
    
    public override void ResetViewOnDestroy()
    {
        bodySprites.ForEach(sprite => sprite.material = unlockedMaterial);
        base.ResetViewOnDestroy();
        
        if(timer == null) return;
        
        timer.OnStart -= UpdateLockSate;
        timer.OnComplete -= UpdateLockSate;
    }
    
    private void UpdateLockSate()
    {
        if(timer == null || bodySprites == null) return;
        
        bodySprites.ForEach(sprite => sprite.material = timer.IsStarted ? lockedMaterial : unlockedMaterial);
    }
}