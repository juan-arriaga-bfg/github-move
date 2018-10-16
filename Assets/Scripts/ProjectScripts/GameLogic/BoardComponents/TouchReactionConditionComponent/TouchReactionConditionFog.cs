public class TouchReactionConditionFog : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{
		var board = piece.Context;
		
//		if (Locker.IsLocked/*boardController?.Pathfinder.CanPathToCastle(piece) == false*/)
//		{
//			if(board.PathfindLocker.HasPath(piece) == false)
//				UIErrorWindowController.AddError("Path not found");
//			return false;
//		}
		
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