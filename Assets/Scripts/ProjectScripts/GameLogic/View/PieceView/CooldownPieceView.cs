using UnityEngine;

public class CooldownPieceView : PieceBoardElementView
{
    private TimerComponent timer;
    
    private ParticleView processParticle;
    private ParticleView readyParticle;

    private WorkplaceLifeComponent life;

    protected virtual string coolDownParticle => string.Empty;
    protected virtual string readyParticleName => string.Empty;
    protected virtual string coolDownLeaveParticle => R.ProductionEndParticle;
    protected virtual string coolDownEnterParticle => string.Empty;

    [SerializeField] private GameObject lockSkin;
    [SerializeField] private GameObject normalSkin;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);
        
        life = Piece.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);
        timer = life?.TimerCooldown;

        if (timer == null) return;
        
        timer.OnStart += OnStart;
        timer.OnComplete += OnComplete;

        ToggleEffectsByState(timer.IsStarted);
        UpdateSate();
    }

    public override void UpdateView()
    {
        base.UpdateView();
        
        if (life?.IsDead == false) return;
        
        ClearParticle(ref processParticle);        
        ClearParticle(ref readyParticle);
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        ClearParticle(ref processParticle);        
        ClearParticle(ref readyParticle);

        if (timer == null) return;
        
        timer.OnStart -= OnStart;
        timer.OnComplete -= OnComplete;
    }

    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);

        if (processParticle != null) processParticle.Particles.Stop();
        if (readyParticle != null) readyParticle.Particles.Stop();
    }
    
    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragEnd(boardPos, worldPos);
        
        PlayAndSyncParticle(processParticle);
        PlayAndSyncParticle(readyParticle);
    }

    protected virtual void OnStart()
    {
        if(string.IsNullOrEmpty(coolDownEnterParticle) == false)
            ParticleView.Show(coolDownEnterParticle, Piece.CachedPosition.SetZ(Piece.CachedPosition.Z + 1));
        ToggleEffectsByState(true);
        UpdateSate();
    }
    
    protected virtual void OnComplete()
    {
        ParticleView.Show(coolDownLeaveParticle, Piece.CachedPosition.SetZ(Piece.CachedPosition.Z + 1));
        ToggleEffectsByState(false);
        
        UpdateSate();
    }
    
    public override void ToggleLockView(bool enabled)
    {
        base.ToggleLockView(enabled);
        
        if (isLockVisual) return;
        
        ToggleEffectsByState(false);
    }

    protected virtual void ToggleEffectsByState(bool isProcessing)
    {
        if (isLockVisual) return;
        
        ClearParticle(ref processParticle);
        ClearParticle(ref readyParticle);

        if (isProcessing) AddParticle(ref processParticle, coolDownParticle);
        else AddParticle(ref readyParticle, readyParticleName);
    }

    private void ClearParticle(ref ParticleView particle)
    {
        if (particle == null) return;
        
        particle.gameObject.SetActive(false);
        Context.DestroyElement(particle);
        particle = null;
    }

    private void AddParticle(ref ParticleView particle, string particleName)
    {
        if (string.IsNullOrEmpty(particleName)) return;
        
        particle = ParticleView.Show(particleName, Piece.CachedPosition);

        var pTransform = particle.transform;
        
        pTransform.SetParent(transform);
        pTransform.localPosition = Vector3.zero;
        
        PlayAndSyncParticle(particle);
    }
    
    private void PlayAndSyncParticle(ParticleView particle)
    {
        if (particle == null) return;
        
        particle.SyncRendererLayers(Piece.CachedPosition.SetZ(3));
        particle.Particles.Play();
    }
    
    protected void UpdateSate()
    {
        if (timer == null || bodySprites == null) return;

        if (lockSkin != null) lockSkin.SetActive(timer.IsStarted);
        if (normalSkin != null) normalSkin.SetActive(!timer.IsStarted);
    }
}