using System.Collections.Generic;

public class TouchReactionDefinitionUpgrade : TouchReactionDefinitionComponent
{
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
			MaxLevelMessage(def);
			return true;
		}
		
		UpgradeMessage(def, position, piece);
		return true;
	}

	private void MaxLevelMessage(PieceDef def)
	{
		var pieceName = def.UpgradeTargetCurrency.Name.Replace("Level", "");
		var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
		model.Title = "Message";
		model.Message = string.Format("You need to improve the level of the {0}", pieceName);
		model.AcceptLabel = string.Format("Go to {0}", pieceName);
        
		model.OnAccept = () => { HintArrowView.Show(GameDataService.Current.PiecesManager.CastlePosition); };
		model.OnCancel = null;
        
		UIService.Get.ShowWindow(UIWindowType.MessageWindow);
	}

	private void UpgradeMessage(PieceDef def, BoardPosition position, Piece piece)
	{
		var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

		model.Title = "Upgrade";
		model.Message = string.Format("Upgrade to {0} level, for {1} coins?", def.CurrentLevel() + 1, def.UpgradePrice.Amount);

		model.AcceptLabel = "Upgrade";
		model.CancelLabel = "Cancel";
		
		model.OnCancel = () => { };
		model.OnAccept = () =>
		{
			CurrencyHellper.Purchase(def.UpgradeCurrency.Name, 1, def.UpgradePrice, success =>
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