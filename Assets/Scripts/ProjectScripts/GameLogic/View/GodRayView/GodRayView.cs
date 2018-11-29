using DG.Tweening;
using UnityEngine;

public class GodRayView : BoardElementView, IBoardEventListener
{
    [SerializeField] private ParticleSystem particle;

    private Piece target;
    
    private void Show()
    {
        particle.gameObject.SetActive(true);
    }

    public void Remove(float delay = 10f)
    {
        const float stopTime = 1.5f;

        DOTween.Sequence()
               .SetId(this)
               .InsertCallback(delay, () =>
                {
                    particle.Stop(true);
                });

        DestroyOnBoard(delay + stopTime);
    }

    public override void Init(BoardRenderer context)
    {
        base.Init(context);
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
    }

    public override void OnFastDestroy()
    {
        base.OnFastDestroy();
        DOTween.Kill(this);
    }

    public void OnDestroy()
    {
        DOTween.Kill(this); 
    }

    public static GodRayView Show(BoardPosition position, float offsetX = 0, float offsetY = 0, bool focus = false)
    {
        var board = BoardService.Current.GetBoardById(0);
        var target = board.BoardLogic.GetPieceAt(position);

        var multi = target?.Multicellular;

        if (multi != null)
        {
            position = multi.GetTopPosition;
        }

        var view = board.RendererContext.CreateBoardElementAt<GodRayView>(R.GodRayView, position);

        view.target = target;
        
        view.CachedTransform.localPosition = view.CachedTransform.localPosition + (Vector3.up * 2) + new Vector3(offsetX, offsetY);
        view.Show();

        if (focus == false)
        {
            return view;
        }

        var worldPos = board.BoardDef.GetSectorCenterWorldPosition(position.X, position.Up.Y, position.Z);
        board.Manipulator.CameraManipulator.MoveTo(worldPos);
        
        return view;
    }

    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceUI)
        {
            return;
        }

        if ((context as PieceBoardElementView)?.Piece != target)
        {
            return;
        }

        Remove(0);
    }
}