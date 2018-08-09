using UnityEngine;

public class ParticleView : BoardElementView
{
    [SerializeField] private float duration;
    private void Show()
    {
        DestroyOnBoard(duration);
    }

    public static ParticleView Show(string particleResourceName, BoardPosition position)
    {
        var board = BoardService.Current.GetBoardById(0);
        var particleView = board.RendererContext.CreateBoardElementAt<ParticleView>(particleResourceName, position);

        particleView.Show();
        return particleView;
    }
}