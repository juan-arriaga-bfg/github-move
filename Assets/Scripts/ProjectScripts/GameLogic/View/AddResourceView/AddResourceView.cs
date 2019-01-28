using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AddResourceView : BoardElementView 
{
	[SerializeField] private NSText amountLabel;

	private const float duration = 1f;

	private bool isPurchase;

	private void Show(CurrencyPair resource)
	{
		var color = resource.Amount < 0 ? "EE4444" : "FFFFFF";
		var value = $"{(resource.Amount < 0 ? "" : "+")}{resource.ToStringIcon()}";
		
		amountLabel.Text = $"<color=#{color}>{value}</color>";
		
		DOTween.Kill(amountLabel);

		var position = CachedTransform.position.y;
		var sequence = DOTween.Sequence().SetId(amountLabel);
		
		sequence.Insert(duration * 0f, amountLabel.TextLabel.DOFade(1f, duration * 0.1f));
		sequence.Insert(duration * 0.9f, amountLabel.TextLabel.DOFade(0f, duration * 0.1f));
		
		sequence.Insert(duration * 0f, CachedTransform.DOMoveY(position + 0.5f, duration * 0.5f).SetEase(Ease.OutQuart));
		sequence.Insert(duration * 0.5f, CachedTransform.DOMoveY(position + 0.7f, duration * 0.3f).SetEase(Ease.Linear));
		sequence.Insert(duration * 0.8f, CachedTransform.DOMoveY(position + 1, duration * 0.2f).SetEase(Ease.InQuart));
		
		if(isPurchase) sequence.InsertCallback(duration * 0.5f, () => { CurrencyHelper.Purchase(resource); });
		
		DestroyOnBoard(duration);
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
		
		DOTween.Kill(amountLabel);
		
		var tColor = amountLabel.TextLabel.color;
		amountLabel.TextLabel.color = new Color(tColor.r, tColor.g, tColor.b, 0);
	}
	
	public static void Show(Vector3 position, CurrencyPair resource, float delay = 0f)
	{
		var board = BoardService.Current.FirstBoard;
		var from = board.BoardDef.GetSectorPosition(position);
		
		Show(from, resource, delay);
	}
	
	public static void Show(BoardPosition position, List<CurrencyPair> resource)
	{
		var sequence = DOTween.Sequence();
        
		for (var i = 0; i < resource.Count; i++)
		{
			var reward = resource[i];
			sequence.InsertCallback(0.5f * i, () => Show(position, reward));
		}
	}

	public static void Show(BoardPosition position, CurrencyPair resource, float delay = 0f)
	{
		if (resource == null) return;
		
		var board = BoardService.Current.FirstBoard;
		var from = board.BoardDef.GetPiecePosition(position.X, position.Y);
		var transaction = CurrencyHelper.PurchaseAsync(resource,null, board.BoardDef.ViewCamera.WorldToScreenPoint(from));

		Show(position, transaction, delay);
	}
	
	public static void Show(BoardPosition position, ShopItemTransaction transaction, float delay = 0f)
	{
		var board = BoardService.Current.FirstBoard;
		var resource = new CurrencyPair{Currency = transaction.ShopItem.ItemUid, Amount = transaction.ShopItem.Amount};
		
		if (resource.Currency == Currency.Coins.Name
		    || resource.Currency == Currency.Crystals.Name
		    || resource.Currency == Currency.Energy.Name
		    || resource.Currency == Currency.Mana.Name
		    || resource.Currency == Currency.Experience.Name)
		{
			DOTween.Sequence().InsertCallback(0.2f + delay, () =>
			{
				transaction.Complete();
				ShowCounter(board, position, resource);
			});
			
			return;
		}
		
		ShowCounter(board, position, resource, true);
	}
	
	private static void ShowCounter(BoardController board, BoardPosition position, CurrencyPair resource, bool isPurchase = false)
	{
		var view = board.RendererContext.CreateBoardElementAt<AddResourceView>(R.AddResourceView, position);
		
		view.CachedTransform.localPosition = view.CachedTransform.localPosition + Vector3.up;
		view.isPurchase = isPurchase;
		view.Show(resource);
	}
}
