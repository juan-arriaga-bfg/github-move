using System.Linq;
using UnityEngine;

public class ReproductionPieceView : PieceBoardElementView
{
    private TimerComponent timer;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        var life = Piece.GetComponent<ReproductionLifeComponent>(ReproductionLifeComponent.ComponentGuid);

        timer = life?.Timer;
        
        if(timer == null) return;
        
        timer.OnStart += UpdateSate;
        timer.OnComplete += UpdateSate;
        
        UpdateSate();
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        if(timer == null) return;
        
        timer.OnStart -= UpdateSate;
        timer.OnComplete -= UpdateSate;
    }
    
    private void UpdateSate()
    {
        if(timer == null || bodySprites == null) return;
        
        bodySprites.First().sprite = IconService.Current.GetSpriteById( $"{PieceType.Parse(Piece.PieceType)}{(timer.IsStarted ? "_lock" : "")}");
    }
}