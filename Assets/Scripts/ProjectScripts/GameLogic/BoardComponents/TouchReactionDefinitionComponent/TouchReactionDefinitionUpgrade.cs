public class TouchReactionDefinitionUpgrade : TouchReactionDefinitionComponent
{
	public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
	{
		return viewDefinition != null && viewDefinition.AddView(ViewType.SimpleUpgrade).IsShow;
	}

	public override bool Make(BoardPosition position, Piece piece)
	{
		var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(piece.PieceType);

		if (def == null) return false;

		if (piece.Context.BoardLogic.MatchDefinition.GetNext(piece.PieceType) == PieceType.None.Id)
		{
			UIMessageWindowController.CreateDefaultMessage("Building can not be improved!");
			return true;
		}

		if (def.IsMaxLevel())
		{
			MaxLevelMessage(def, piece.Context);
			return true;
		}
		
		UpgradeMessage(def, position, piece);
		return true;
	}

	private void MaxLevelMessage(PieceDef def, BoardController board)
	{
		var pieceName = def.UpgradeTargetCurrency.Name.Replace("Level", "");
		var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
		model.Title = "Message";
		model.Message = $"You need to improve the level of the {pieceName}";
		model.AcceptLabel = $"Go to {pieceName}";
        
		model.OnAccept = () => { board.HintCooldown.Step(GameDataService.Current.PiecesManager.CastlePosition); };
		model.OnCancel = null;
        
		UIService.Get.ShowWindow(UIWindowType.MessageWindow);
	}

	private void UpgradeMessage(PieceDef def, BoardPosition position, Piece piece)
	{
		var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

		model.Title = "Upgrade";
		model.Message = $"Upgrade to {def.CurrentLevel() + 1} level, for {def.UpgradePrices[0].ToStringIcon(false)}?";

		model.AcceptLabel = "Upgrade";
		model.CancelLabel = "Cancel";

		model.OnCancel = () => { };
		model.OnAccept = () =>
		{
			CurrencyHellper.Purchase(def.UpgradeCurrency.Name, 1, def.UpgradePrices[0], success =>
			{
				if (!success) return;
				
				piece.Context.ActionExecutor.AddAction(new CheckMatchAction
				{
					At = position
				});
			});
		};
		
		UIService.Get.ShowWindow(UIWindowType.MessageWindow);
	}
}