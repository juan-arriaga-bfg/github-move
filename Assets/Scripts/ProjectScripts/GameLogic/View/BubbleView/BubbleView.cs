using System.Collections.Generic;
using UnityEngine;

public class BubbleView : UIBoardView
{
    [SerializeField] private NSText message;
    [SerializeField] private NSText button;
    
    protected override ViewType Id => ViewType.FogState;

    public override Vector3 Ofset => new Vector3(0, 2f);

    public override void Init(Piece piece)
    {
        base.Init(piece);
		
        Priority = defaultPriority = 0;
    }
    
    protected override void UpdateView()
    {
        message.Text = "collect a megazor";
        button.Text = "collect";
    }
    
    public void Сollect()
    {
        var action = Context.Context.BoardLogic.MatchActionBuilder.GetMatchAction(new List<BoardPosition>(), Context.PieceType, Context.CachedPosition);
        
        if(action == null) return;
        
        Context.Context.ActionExecutor.AddAction(action);
    }
}