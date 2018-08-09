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
		var value = $"{(resource.Amount < 0 ? "" : "+")}{resource.ToStringIcon(false)}";
		
		amountLabel.Text = $"<color=#{color}>{value}</color>";
		
		DOTween.Kill(animationUid);

		var position = CachedTransform.position.y;
		var sequence = DOTween.Sequence().SetId(animationUid);
		
		sequence.Insert(duration * 0f, amountLabel.TextLabel.DOFade(1f, duration * 0.1f));
		sequence.Insert(duration * 0.9f, amountLabel.TextLabel.DOFade(0f, duration * 0.1f));
		
		sequence.Insert(duration * 0f, CachedTransform.DOMoveY(position + 0.5f, duration * 0.5f).SetEase(Ease.OutQuart));
		sequence.Insert(duration * 0.8f, CachedTransform.DOMoveY(position + 1, duration * 0.2f).SetEase(Ease.InQuart));
		
		if(isPurchase) sequence.InsertCallback(duration * 0.5f, () => { CurrencyHellper.Purchase(resource); });
		
		DestroyOnBoard(duration);
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
		
		DOTween.Kill(animationUid);
		
		var tColor = amountLabel.TextLabel.color;
		amountLabel.TextLabel.color = new Color(tColor.r, tColor.g, tColor.b, 0);
	}

	public static void Show(Vector3 position, CurrencyPair resource)
	{
		var board = BoardService.Current.GetBoardById(0);
		var from = board.BoardDef.GetSectorPosition(position);
		
		Show(from, resource);
	}

	public static void Show(BoardPosition position, CurrencyPair resource)
	{
		if (resource == null) return;
		
		var board = BoardService.Current.GetBoardById(0);
		
		if (resource.Currency == Currency.Coins.Name
		    || resource.Currency == Currency.Crystals.Name
		    || resource.Currency == Currency.Energy.Name
		    || resource.Currency == Currency.Experience.Name)
		{
			CurrencyHellper.Purchase(resource, success =>
			{
				var flay = ResourcesViewManager.Instance.GetFirstViewById(resource.Currency);
				var from = board.BoardDef.GetPiecePosition(position.X, position.Y);
				
				var carriers = ResourcesViewManager.DeliverResource<ResourceCarrier>
				(
					resource.Currency,
					resource.Amount,
					flay.GetAnchorRect(),
					board.BoardDef.ViewCamera.WorldToScreenPoint(from),
					R.ResourceCarrier
				);

				carriers[carriers.Count - 1].Callback = () => { ShowCounter(board, position, resource); };
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
