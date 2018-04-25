using System;

public class TouchReactionDefinitionUpgradeCastle : TouchReactionDefinitionComponent
{
	public bool isOpen;
	public Action OnClick { get; set; }
	
	public override bool Make(BoardPosition position, Piece piece)
	{
		var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(piece.PieceType);

		if (def == null) return false;

		if (piece.Context.BoardLogic.MatchDefinition.GetNext(piece.PieceType) == PieceType.None.Id)
		{
			UIMessageWindowController.CreateDefaultMessage("Building can not be improved!");
			return true;
		}
        
		if (OnClick != null) OnClick();
		return true;
	}

	public void Upgrade(PieceDef def, Piece piece)
	{
		CurrencyHellper.Purchase(def.UpgradeCurrency.Name, 1, success =>
		{
			if (!success) return;
				
			piece.Context.ActionExecutor.AddAction(new CheckMatchAction
			{
				At = piece.CachedPosition
			});
		});
	}
}