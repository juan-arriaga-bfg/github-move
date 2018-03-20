using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AddResourceView : BoardElementView 
{
	[SerializeField] private SpriteRenderer icon;
	[SerializeField] private NSText amountLabel;

	public void AddResource(CurrencyPair resource, Vector3 targetPos)
	{
		icon.sprite = IconService.Current.GetSpriteById(string.Format("icon_gameboard_{0}" , resource.Currency));
		amountLabel.Text = string.Format("+{0}", resource.Amount);
		
		float duration = 1f;
		
		DOTween.Kill(animationUid);
		
		var sequence = DOTween.Sequence().SetId(animationUid);
		
		sequence.Append(CachedTransform.DOMove(targetPos, duration));
		sequence.Append(icon.DOFade(0f, duration));
		sequence.Append(amountLabel.TextLabel.DOFade(0f, duration));
		sequence.InsertCallback(duration*0.5f, () => { Add(resource); });
		
		DestroyOnBoard(duration);
	}

	private void Add(CurrencyPair resource)
	{
		var shopItem = new ShopItem
		{
			Uid = string.Format("add.test.{0}.{1}", resource.Currency, resource.Amount), 
			ItemUid = resource.Currency, 
			Amount = resource.Amount,
			CurrentPrices = new List<Price>
			{
				new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}
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
			}
		);
	}
}
