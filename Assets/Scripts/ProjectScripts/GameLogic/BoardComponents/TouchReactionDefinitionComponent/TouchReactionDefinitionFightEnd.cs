﻿using System.Collections.Generic;

public class TouchReactionDefinitionFightEnd : TouchReactionDefinitionComponent
{
	public bool isDead;
	
	public override bool Make(BoardPosition position, Piece piece)
	{
		if (isDead) return false;

		isDead = true;
		
		piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
		{
			To = position,
			Positions = new List<BoardPosition>{position}
		});
		
		piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.EnemyDeath, GameDataService.Current.GetEnemy(true).Chest);
		
		return true;
	}
}