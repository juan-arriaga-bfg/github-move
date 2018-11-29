public class TouchReactionConditionFog : TouchReactionConditionComponent
{
	private FogObserver observer;
	
	public override bool Check(BoardPosition position, Piece piece)
	{
		if (observer == null) observer = piece.GetComponent<FogObserver>(FogObserver.ComponentGuid);

		if (!observer.IsActive) return false;
		
		if (piece.Context?.Pathfinder.CanPathToCastle(piece) == false)
		{
			UIErrorWindowController.AddError(LocalizationService.Get("message.error.pieceLock", "message.error.pieceLock"));
			return false;
		}
		
		var key = new BoardPosition(position.X, position.Y);
		var def = GameDataService.Current.FogsManager.GetDef(key);

		if (def == null) return false;
		
		if (IsDone) return true;
        
		IsDone = def.Level <= GameDataService.Current.LevelsManager.Level;
		
		if (IsDone == false)
		{
			UIErrorWindowController.AddError(string.Format(LocalizationService.Get("message.error.fogLevel", "message.error.fogLevel {0}"), def.Level));
		}
        
		return IsDone;
	}
}