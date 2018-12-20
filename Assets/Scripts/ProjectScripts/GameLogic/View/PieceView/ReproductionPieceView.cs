﻿using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ReproductionPieceView : PieceBoardElementView
{
    private TimerComponent timer;
    private ParticleView processParticle;
    private ParticleView readyParticle;

    private ReproductionLifeComponent reproductionLife;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        var life = Piece.GetComponent<ReproductionLifeComponent>(ReproductionLifeComponent.ComponentGuid);

        timer = life?.Timer;
        
        if(timer == null) return;
        
        timer.OnStart += OnStart;
        timer.OnComplete += OnComplete;

        ToggleEffectsByState(timer.IsStarted);
        
        UpdateSate();
    }

    public override void UpdateView()
    {
        base.UpdateView();
        
        if(reproductionLife == null)
            reproductionLife = Piece.GetComponent<ReproductionLifeComponent>(ReproductionLifeComponent.ComponentGuid);

        if (reproductionLife?.IsDead == true)
        {
            ClearParticle(ref processParticle);        
            ClearParticle(ref readyParticle);
        }
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        ClearParticle(ref processParticle);        
        ClearParticle(ref readyParticle);
        
        if(timer == null) return;
        
        timer.OnStart -= OnStart;
        timer.OnComplete -= OnComplete;
    }

    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);
           
        processParticle?.Particles.Stop();
        readyParticle?.Particles.Stop();
    }
    
    private void SyncParticlePosition(ParticleView particle)
    {
        particle.SyncRendererLayers(Piece.CachedPosition.SetZ(3));
    }

    private void PlayAndSyncParticle(ParticleView particle)
    {
        if (particle != null)
        {
            SyncParticlePosition(particle);
            particle.Particles.Play();
        }
    }
    
    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragEnd(boardPos, worldPos);
        
        PlayAndSyncParticle(processParticle);
        PlayAndSyncParticle(readyParticle);
    }

    private void OnStart()
    {
        ToggleEffectsByState(true);
        
        UpdateSate();
    }

    protected virtual void ToggleEffectsByState(bool isProcessing)
    {
        if(BoardService.Current.FirstBoard.TutorialLogic.CheckLockPR() == false) return;
        
        if (isProcessing)
        {
            ClearParticle(ref readyParticle);
        
            processParticle = ParticleView.Show(R.ProductionProcessParticle, Piece.CachedPosition);
            processParticle.transform.SetParent(transform);
            processParticle.transform.localPosition = Vector3.zero;
        
            PlayAndSyncParticle(processParticle);
        }
        else
        {
            ClearParticle(ref processParticle);
        
            readyParticle = ParticleView.Show(R.ProductionReadyParticle, Piece.CachedPosition);
            readyParticle.transform.SetParent(transform);
            readyParticle.transform.localPosition = Vector3.zero;
        
            PlayAndSyncParticle(readyParticle);   
        }
    }

    private void ClearParticle(ref ParticleView particle)
    {
        if (particle == null)
            return;
        particle.gameObject.SetActive(false);
        Context.DestroyElement(particle);
        particle = null;
    }

    private void OnComplete()
    {
        ParticleView.Show(R.ProductionEndParticle, Piece.CachedPosition.SetZ(Piece.CachedPosition.Z + 1));

        ToggleEffectsByState(false);
        
        UpdateSate();
    }
    
    private void UpdateSate()
    {
        if(timer == null || bodySprites == null) return;
                
        bodySprites.First().sprite = IconService.Current.GetSpriteById( $"{PieceType.Parse(Piece.PieceType)}{(timer.IsStarted ? "_lock" : "")}");
    }
}