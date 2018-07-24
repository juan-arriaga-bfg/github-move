public class TouchReactionDefinitionUpgradeCastle : TouchReactionDefinitionComponent
{
	public override bool Make(BoardPosition position, Piece piece)
	{
		var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(piece.PieceType);
		var upgrade = piece.GetComponent<CastleUpgradeComponent>(CastleUpgradeComponent.ComponentGuid);
		
		if (def == null) return false;

		if (upgrade.Prices.Count == 0)
		{
			UIMessageWindowController.CreateDefaultMessage("Building can not be improved!");
			return true;
		}

		if (upgrade.Check() == false)
		{
			var viewDef = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
			var view = viewDef.AddView(ViewType.CastleUpgrade);
        
			view.Change(!view.IsShow);
			return false;
		}

		CurrencyHellper.Purchase(def.UpgradeCurrency.Name, 1, success =>
		{
			if (!success) return;
				
			piece.Context.ActionExecutor.AddAction(new CheckMatchAction
			{
				At = piece.CachedPosition
			});
		});
		
		return true;
	}
}