using System.Collections.Generic;

public class TouchReactionDefinitionSpawnShop : TouchReactionDefinitionComponent
{
	public int Reward = -1;
	
	public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
	{
		return false;
	}
    
	public override bool Make(BoardPosition position, Piece piece)
	{
		const int amount = 1;
        
		var free = new List<BoardPosition>();
		var positions = new List<BoardPosition>();
        
		if (piece.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(position, free, amount) == false)
		{
			return false;
		}

		if (free.Count < amount)
			return false;
        
		foreach (var pos in free)
		{
			positions.Add(pos);
			if(positions.Count == amount) break;
		}

		var target = positions[0];
		var worldPos = piece.Context.BoardDef.GetSectorCenterWorldPosition(target.X, target.Y, target.Z);
		
		piece.Context.Manipulator.CameraManipulator.MoveTo(worldPos);
        
		piece.Context.ActionExecutor.AddAction(new ReproductionPieceAction
		{
			From = position,
			Piece = Reward,
			Positions = positions,
			AnimationResourceSearch = pieceType => AnimationOverrideDataService.Current.FindAnimation(pieceType, def => def.OnPurchaseSpawn),
			OnComplete = () =>
			{
				var view = piece.Context.RendererContext.GetElementAt(position) as CharacterPieceView;
                
				if(view != null) view.StartRewardAnimation();
			}
		});
		
		Reward = -1;
		return true;
	}
}