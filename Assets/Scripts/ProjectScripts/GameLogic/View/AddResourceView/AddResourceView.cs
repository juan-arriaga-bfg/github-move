using DG.Tweening;
using UnityEngine;

public class AddResourceView : BoardElementView 
{
	[SerializeField] private SpriteRenderer icon;
	[SerializeField] private NSText amountLabel;

	private void Show(CurrencyPair resource)
	{
		icon.sprite = IconService.Current.GetSpriteById(string.Format("icon_gameboard_{0}" , resource.Currency));
		amountLabel.Text = string.Format("<color=#EE4444>+{0}</color>", resource.Amount);
		
		float duration = 1f;
		
		DOTween.Kill(animationUid);
		
		var sequence = DOTween.Sequence().SetId(animationUid);
		
		sequence.Insert(0, CachedTransform.DOMove(CachedTransform.position + Vector3.up, duration));
		sequence.Insert(duration*0.5f, icon.DOFade(0f, duration));
		sequence.Insert(duration*0.5f, amountLabel.TextLabel.DOFade(0f, duration));
		sequence.InsertCallback(duration*0.5f, () => { CurrencyHellper.Purchase(resource); });
		
		DestroyOnBoard(duration);
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
		
		icon.color = Color.white;

		var tColor = amountLabel.TextLabel.color;
		
		amountLabel.TextLabel.color = new Color(tColor.r, tColor.g, tColor.b, 1);
	}

	public static void Show(BoardPosition position, CurrencyPair resource)
	{
		var board = BoardService.Current.GetBoardById(0);
		var view = board.RendererContext.CreateBoardElementAt<AddResourceView>(R.AddResourceView, position);
		
		view.CachedTransform.localPosition = view.CachedTransform.localPosition + Vector3.up;
		view.Show(resource);
	}
}
