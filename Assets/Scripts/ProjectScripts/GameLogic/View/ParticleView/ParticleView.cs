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
        if (duration == 0)
            return;
        DestroyOnBoard(duration);
    }

    public static ParticleView Show(string particleResourceName, BoardPosition position)
    {
        var board = BoardService.Current.GetBoardById(0);
        var particleView = board.RendererContext.CreateBoardElementAt<ParticleView>(particleResourceName, position);

        if (particleView.Particles == null)
            particleView.Particles = particleView.GetComponentInChildren<ParticleSystem>();
        
        particleView.Show();
        return particleView;
    }
}