using System.Collections.Generic;

public class TouchReactionDefinitionUpgrade : TouchReactionDefinitionComponent
{
	public override bool Make(BoardPosition position, Piece piece)
	{
		var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(piece.PieceType);

		if (def == null) return false;
		
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
				},
				item =>
				{
					// on purchase failed (not enough cash)
					UIMessageWindowController.CreateDefaultMessage("Not enough coins!");
				}
			);
		};
		
		UIService.Get.ShowWindow(UIWindowType.MessageWindow);
		return true;
	}
}