using System;
using UnityEngine;

public class ParticleView : BoardElementView
{
    [SerializeField] private float duration;
    public ParticleSystem Particles;
    private ParticleSystemRenderer particleRenderer = null;

    public ParticleSystemRenderer ParticleRenderer
    {
        get
        {
            if (particleRenderer != null)
                return particleRenderer;
            return particleRenderer = Particles.GetComponent<ParticleSystemRenderer>();
        }
    }
        
    
    private void Show()
    {
        if (Math.Abs(duration) < 0.001)
            return;
        DestroyOnBoard(duration);
    }

    public static ParticleView Show(string particleResourceName, BoardPosition position)
    {
        var board = BoardService.Current.FirstBoard;
        var particleView = board.RendererContext.CreateBoardElementAt<ParticleView>(particleResourceName, position);

        if (particleView.Particles == null)
            particleView.Particles = particleView.GetComponentInChildren<ParticleSystem>();
        
        particleView.Show();
        return particleView;
    }
    
    public static ParticleView Show(string particleResourceName, BoardPosition position, Vector3 worldPosition, Vector3 scale)
    {
        var board = BoardService.Current.FirstBoard;
        var particleView = board.RendererContext.CreateBoardElementAt<ParticleView>(particleResourceName, position);

        if (particleView.Particles == null)
            particleView.Particles = particleView.GetComponentInChildren<ParticleSystem>();

        particleView.CachedTransform.position = worldPosition;
        particleView.CachedTransform.localScale = scale;
        
        particleView.Show();
        return particleView;
    }
    
    public override void SyncRendererLayers(BoardPosition boardPosition)
    {
        base.SyncRendererLayers(boardPosition);
        
        if (CachedTransform.parent == Context.ViewRoot) return;

        if (cachedSortingGroup != null)
        {
            Destroy(cachedSortingGroup);
        }
    }
}