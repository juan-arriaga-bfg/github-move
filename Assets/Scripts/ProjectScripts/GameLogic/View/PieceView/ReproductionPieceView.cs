using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ReproductionPieceView : PieceBoardElementView
{
    private TimerComponent timer;
    private ParticleView processParticle;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        var life = Piece.GetComponent<ReproductionLifeComponent>(ReproductionLifeComponent.ComponentGuid);

        timer = life?.Timer;
        
        if(timer == null) return;
        
        timer.OnStart += OnStart;
        timer.OnComplete += OnComplete;
        
        UpdateSate();
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        if(timer == null) return;
        
        timer.OnStart -= OnStart;
        timer.OnComplete -= OnComplete;
    }

    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);
        if(processParticle != null)
            processParticle.Particles.Stop();
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragEnd(boardPos, worldPos);
        if (processParticle != null)
        {
            processParticle.SetTargetPosition(Context.Context.BoardDef.GetWorldPosition(boardPos.X, boardPos.Y));
            processParticle.ParticleRenderer.sortingOrder = bodySprites.First().sortingOrder + 1;
            processParticle.Particles.Play();
        }
            
    }

    private void OnStart()
    {
        Debug.LogError("Start");
        
        ParticleView.Show(R.ProductionStartParticle, Piece.CachedPosition.SetZ(Piece.CachedPosition.Z + 1));
        processParticle = ParticleView.Show(R.ProductionProcessParticle, Piece.CachedPosition);
        processParticle.Particles.Play();

        processParticle.ParticleRenderer.sortingOrder = bodySprites.First().sortingOrder + 1;
        
        UpdateSate();
    }

    private void OnComplete()
    {   
        Debug.LogError("End");
        ParticleView.Show(R.ProductionEndParticle, Piece.CachedPosition.SetZ(Piece.CachedPosition.Z + 1));

        if (processParticle != null)
        {
            Debug.LogError("Remove execute");
            Context.DestroyElement(processParticle);
            processParticle = null;
        }
            
        
        UpdateSate();
    }
    
    private void UpdateSate()
    {
        if(timer == null || bodySprites == null) return;
                
        bodySprites.First().sprite = IconService.Current.GetSpriteById( $"{PieceType.Parse(Piece.PieceType)}{(timer.IsStarted ? "_lock" : "")}");
    }
}