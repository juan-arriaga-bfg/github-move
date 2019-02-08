using System.Collections.Generic;
using DG.Tweening;

public class RemoveTutorialAnimation : BaseTutorialAnimation
{
    public int PieceId = PieceType.None.Id;
    public BoardPosition Target;
    
    public override void Start()
    {
        if (isStart) return;
        
        base.Start();
        
        if(Find()) return;

        Hard(Target);
    }

    private bool Find()
    {
        if(PieceId == PieceType.None.Id) return false;

        var positions = context.Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(PieceId);

        foreach (var position in positions)
        {
            Hard(position);
        }
        
        return true;
    }
    
    private void Hard(BoardPosition target)
    {
        var board = context.Context.Context;
        
        if(board.BoardLogic.IsEmpty(target)) return;
        
        board.BoardLogic.RemovePieceAt(target);

        DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() => board.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                To = target,
                Positions = new List<BoardPosition> {target},
                AnimationResourceSearch = piece => AnimationOverrideDataService.Current.FindAnimation(piece, def => def.OnDestroyFromBoard)
            }));
    }
}