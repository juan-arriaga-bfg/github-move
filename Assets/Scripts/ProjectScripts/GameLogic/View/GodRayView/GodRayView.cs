using System;
using DG.Tweening;
using UnityEngine;

public class GodRayView : BoardElementView
{
    [SerializeField] private ParticleSystem particleTop;
    [SerializeField] private ParticleSystem particleBottom;

    const float SMOOTH_STOP_TIME = 0.9f;

    private float lifetime;

    private bool hideInProgress;

    private Action onHided;
    
    private void Show(Action onHided)
    {
        this.onHided = onHided;
        
        DOTween.Kill(this);
        
        hideInProgress = false;
        
        StartParticleSystem(particleTop);
        StartParticleSystem(particleBottom);
        
        DOTween.Sequence()
               .SetId(this)
               .InsertCallback(lifetime, () =>
                {
                    Hide(true);
                });
    }

    public void Hide(bool animated)
    {
        if (!animated)
        {
            Context.RemoveElement(this);
            
            CleanUp();
            onHided?.Invoke();
            return;
        }

        if (hideInProgress)
        {
            return;
        }
        
        DOTween.Kill(this);
        
        StopParticleSystemSmoothly(particleTop,    SMOOTH_STOP_TIME);
        StopParticleSystemSmoothly(particleBottom, SMOOTH_STOP_TIME);

        hideInProgress = true;
        
        DOTween.Sequence()
               .SetId(this)
               .InsertCallback(SMOOTH_STOP_TIME + 5, () =>
                {
                    Context.RemoveElement(this);
                    hideInProgress = false;
                    onHided?.Invoke();
                });
    }

    public override void OnFastDestroy()
    {
        base.OnFastDestroy();
        CleanUp();
    }

    public void OnDestroy()
    {
        CleanUp();
    }

    public static GodRayView Show(BoardPosition position, float lifetime, Action onHided, float offsetX = 0, float offsetY = 0, bool focus = false, bool enableTopHighlight = true, bool enableBottomHighlight = true)
    {
      
        
        var board = BoardService.Current.FirstBoard;
        var target = board.BoardLogic.GetPieceAt(position);

        var multi = target?.Multicellular;

        if (multi != null)
        {
            position = multi.GetTopPosition;
        }

        var view = board.RendererContext.CreateBoardElementAt<GodRayView>(R.GodRayView, position);
        view.lifetime = lifetime;
        
        view.CachedTransform.localPosition = view.CachedTransform.localPosition + (Vector3.up * 2) + new Vector3(offsetX, offsetY);
        view.Show(onHided);
        
        view.particleTop.gameObject.SetActive(enableTopHighlight);
        view.particleBottom.gameObject.SetActive(enableBottomHighlight);

        if (focus == false)
        {
            return view;
        }

        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(position.X, position.Up.Y, position.Z);
        board.Manipulator.CameraManipulator.MoveTo(worldPos);
        
        return view;
    }

    public override void SyncRendererLayers(BoardPosition boardPosition)
    {
        // Do nothing
    }

    private void StopParticleSystemSmoothly(ParticleSystem particleSystem, float stopTime)
    {
        var main = particleSystem.main;
        main.loop = false;
        
        var emission = particleSystem.emission;
        emission.enabled = false;
        
        var particles = new ParticleSystem.Particle[particleSystem.particleCount];
        int count = particleSystem.GetParticles(particles);
        
        for (int i = 0; i < count; i++)
        {
            particles[i].remainingLifetime = Mathf.Min(particles[i].remainingLifetime, stopTime);
        }
        
        particleSystem.SetParticles(particles, count);
    }
    
    private void StartParticleSystem(ParticleSystem particleSystem)
    {       
        var emission = particleSystem.emission;
        emission.enabled = true;

        particleSystem.Play();
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        CleanUp();
    }

    private void CleanUp()
    {
        DOTween.Kill(this);
        particleTop.Stop();
        particleBottom.Stop(); 
    }
}