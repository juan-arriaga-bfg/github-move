﻿public class TouchReactionConditionFog : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{
		var pathfinder = piece.GetComponent<PathfinderComponent>(PathfinderComponent.ComponentGuid);
		
		if (pathfinder?.CanPathToCastle(piece) == false)
		{
			UIErrorWindowController.AddError("Path not found");
			return false;
		}

		var key = new BoardPosition(position.X, position.Y);
		var def = GameDataService.Current.FogsManager.GetDef(key);

		if (def == null) return false;
		
		if (IsDone) return true;
        
		IsDone = def.Level <= GameDataService.Current.LevelsManager.Level;
		
		if (IsDone == false)
		{
			UIErrorWindowController.AddError($"Reach Level {def.Level} to get access to the fog");
		}
        
		return IsDone;
	}
}