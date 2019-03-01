public class TouchReactionConditionFog : TouchReactionConditionComponent
{
	private FogObserver observer;
	
	public override bool Check(BoardPosition position, Piece piece)
	{
		var board = piece.Context;
		
		var key = new BoardPosition(position.X, position.Y);
		var def = GameDataService.Current.FogsManager.GetDef(key);

		if (def == null) return false;
		
		if (IsDone) return true;

		if (def.IsActive == false)
		{
			UIErrorWindowController.AddError(LocalizationService.Get("message.error.fogLock", "message.error.fogLock"));
			return false;
		}

		if (def.HeroId != PieceType.None.Id && GameDataService.Current.CodexManager.IsPieceUnlocked(def.HeroId) == false)
		{
			var hero = LocalizationService.Get($"piece.name.{PieceType.Parse(def.HeroId)}", $"piece.name.{PieceType.Parse(def.HeroId)}");
			UIErrorWindowController.AddError(string.Format(LocalizationService.Get("message.error.fogHero", "message.error.fogHero {0}"), hero));
			return false;
		}
		
		if (def.Level > GameDataService.Current.LevelsManager.Level)
		{
			UIErrorWindowController.AddError(string.Format(LocalizationService.Get("message.error.fogLevel", "message.error.fogLevel {0}"), def.Level));
			return false;
		}

		IsDone = true;
		
		return IsDone;
	}
}