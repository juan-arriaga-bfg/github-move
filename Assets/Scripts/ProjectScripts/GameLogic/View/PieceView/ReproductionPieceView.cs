using UnityEngine;

public class ReproductionPieceView : PieceBoardElementView
{
    [SerializeField] private Material lockedMaterial;
    
    private Material unlockedMaterial;
    
    private TimerComponent timer;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        var life = Piece.GetComponent<ReproductionLifeComponent>(ReproductionLifeComponent.ComponentGuid);

        timer = life?.Timer;
        
        if(timer == null) return;
        
        timer.OnStart += UpdateSate;
        timer.OnComplete += UpdateSate;

        if (bodySprite != null) unlockedMaterial = bodySprite.material;
        UpdateSate();
    }
    
    public override void ResetViewOnDestroy()
    {
        bodySprite.material = unlockedMaterial;
        base.ResetViewOnDestroy();
        
        if(timer == null) return;
        
        timer.OnStart -= UpdateSate;
        timer.OnComplete -= UpdateSate;
    }
    
    private void UpdateSate()
    {
        if(timer == null || bodySprite == null) return;
        
        bodySprite.material = timer.IsStarted ? lockedMaterial : unlockedMaterial;
    }
}