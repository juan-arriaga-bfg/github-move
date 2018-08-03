using UnityEngine;

public class ParticleView : BoardElementView
{
    private void Show(float duration)
    {
        DestroyOnBoard(duration);
    }

    public static void Show(string particleResourceName, BoardPosition position, float offsetX = 0, float offsetY = 0)
    {
        var board = BoardService.Current.GetBoardById(0);
        var particleView = board.RendererContext.CreateBoardElementAt<ParticleView>(particleResourceName, position);

        float particleDuration = 0;
        var particleComponent = particleView.GetComponent<ParticleSystem>();
        if (particleComponent != null)
            particleDuration = particleComponent.duration;
        particleView.Show(particleDuration);
    }
}