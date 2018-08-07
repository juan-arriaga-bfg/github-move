using DG.Tweening;
using UnityEngine;

public class AddResourceView : BoardElementView 
{
	[SerializeField] private SpriteRenderer icon;
	[SerializeField] private NSText amountLabel;

	private const float duration = 1f;

	private void Show(CurrencyPair resource)
	{
		var color = resource.Amount < 0 ? "EE4444" : "FFFFFF";
		var value = string.Format("{0}{1}", resource.Amount < 0 ? "" : "+", resource.Amount);
		
		icon.gameObject.SetActive(resource.Currency != Currency.Experience.Name);
		
		if (resource.Currency != Currency.Experience.Name)
		{
			icon.sprite = IconService.Current.GetSpriteById(resource.Currency);
			icon.transform.localScale = Vector3.one * (1.5f - icon.sprite.rect.height / 106f);
		}
		else
		{
			value += " EXP";
		}
		
		amountLabel.Text = string.Format("<color=#{0}>{1}</color>", color, value);
		
		DOTween.Kill(animationUid);
		
		var sequence = DOTween.Sequence().SetId(animationUid);
		
		sequence.Insert(0, CachedTransform.DOMove(CachedTransform.position + Vector3.up, duration));
		sequence.Insert(duration*0.5f, icon.DOFade(0f, duration));
		sequence.Insert(duration*0.5f, amountLabel.TextLabel.DOFade(0f, duration));
		sequence.InsertCallback(duration * 0.5f, () => { CurrencyHellper.Purchase(resource); });
		
		DestroyOnBoard(duration);
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
		
		DOTween.Kill(animationUid);
		
		icon.color = Color.white;
		
		var tColor = amountLabel.TextLabel.color;
		
		amountLabel.TextLabel.color = new Color(tColor.r, tColor.g, tColor.b, 1);
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
		    || resource.Currency == Currency.Energy.Name
		    || resource.Currency == Currency.Experience.Name)
		{
			CurrencyHellper.Purchase(resource, success =>
			{
				var flay = ResourcesViewManager.Instance.GetFirstViewById(resource.Currency);
				var from = board.BoardDef.GetPiecePosition(position.X, position.Y);
				
				ResourcesViewManager.DeliverResource<ResourceCarrier>
				(
					resource.Currency,
					resource.Amount,
					flay.GetAnchorRect(),
					board.BoardDef.ViewCamera.WorldToScreenPoint(from),
					R.ResourceCarrier
				);
			});
			return;
		}
		
		var view = board.RendererContext.CreateBoardElementAt<AddResourceView>(R.AddResourceView, position);
		
		view.CachedTransform.localPosition = view.CachedTransform.localPosition + Vector3.up;
		view.Show(resource);
	}
}
