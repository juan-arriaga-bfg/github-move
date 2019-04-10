using System;

public class CollapseManaAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public virtual int Guid => ComponentGuid;

    public Piece Mana;
    public BoardPosition FogPosition;
    
    public Action OnComplete;
    
    public bool PerformAction(BoardController gameBoardController)
    {
        var animation = new CollapseManaAnimation
        {
            From = Mana.CachedPosition,
            To = FogPosition
        };
		
        animation.OnCompleteEvent += (_) =>
        {
            OnComplete?.Invoke();
        };
		
        gameBoardController.RendererContext.AddAnimationToQueue(animation);
        
        return true;
    }
}
