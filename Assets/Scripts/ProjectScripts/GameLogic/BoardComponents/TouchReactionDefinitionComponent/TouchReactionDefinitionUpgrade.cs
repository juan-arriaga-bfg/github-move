using System.Collections.Generic;

public class TouchReactionDefinitionUpgrade : TouchReactionDefinitionComponent
{
	public override bool Make(BoardPosition position, Piece piece)
	{
		var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(piece.PieceType);

		if (def == null) return false;

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
		var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
		model.Title = "Message";
		model.Message = string.Format("You need to improve the level of the {0}", def.UpgradeTargetPiece);
		model.AcceptLabel = string.Format("Go to {0}", def.UpgradeTargetPiece);;
        
		model.OnAccept = () => { HintArrowView.AddHint(GameDataService.Current.PiecesManager.CastlePosition); };
		model.OnCancel = null;
        
		UIService.Get.ShowWindow(UIWindowType.MessageWindow);
	}

	private void UpgradeMessage(PieceDef def, BoardPosition position, Piece piece)
	{
		var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

		model.Title = "Upgrade";
		model.Message = string.Format("upgrade to {0} level, for {1} coins?", def.Uid[def.Uid.Length - 1], def.UpgradePrice.Amount);

		model.AcceptLabel = "Upgrade";
		model.CancelLabel = "Cancel";
		
		model.OnCancel = () => { };
		model.OnAccept = () =>
		{
			var shopItem = new ShopItem
			{
				Uid = string.Format("purchase.test.{0}.10", def.UpgradeCurrency.Name), 
				ItemUid = def.UpgradeCurrency.Name, 
				Amount = 1,
				CurrentPrices = new List<Price>
				{
					new Price{Currency = def.UpgradePrice.Currency, DefaultPriceAmount = def.UpgradePrice.Amount}
				}
			};
        
			ShopService.Current.PurchaseItem
			(
				shopItem,
				(item, s) =>
				{
					// on purchase success
					
					piece.Context.ActionExecutor.AddAction(new CheckMatchAction
					{
						At = position
					});
				},
				item =>
				{
					// on purchase failed (not enough cash)
					UIMessageWindowController.CreateDefaultMessage("Not enough coins!");
				}
			);
		};
		
		UIService.Get.ShowWindow(UIWindowType.MessageWindow);
	}
}