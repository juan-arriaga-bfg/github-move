public class TouchReactionConditionFog : TouchReactionConditionComponent
{
	private FogObserver observer;
	
	public override bool Check(BoardPosition position, Piece piece)
	{
		if (observer == null) observer = piece.GetComponent<FogObserver>(FogObserver.ComponentGuid);

		if (!observer.IsActive) return false;
		
		if (piece.Context?.Pathfinder.CanPathToCastle(piece) == false)
		{
			UIErrorWindowController.AddError(LocalizationService.Instance.Manager.GetTextByUid("message.error.pieceLock", "Path not found!"));
			return false;
		}
		
		var key = new BoardPosition(position.X, position.Y);
		var def = GameDataService.Current.FogsManager.GetDef(key);

		if (def == null) return false;
		
		if (IsDone) return true;
        
		IsDone = def.Level <= GameDataService.Current.LevelsManager.Level;
		
		if (IsDone == false)
		{
			var message = string.Format(LocalizationService.Instance.Manager.GetTextByUid("message.error.fogLevel",
						"Reach Level {0} to get access to the fog!"), def.Level);
			
			UIErrorWindowController.AddError(message);
		}
        
		return IsDone;
	}
}