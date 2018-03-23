using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AddCardsView : BoardElementView
{
	[SerializeField] private List<SpriteRenderer> icons;
    
	public void Show(Sprite sprite)
	{
		const float duration = 1.5f;
		var delay = 0f;
        
		DOTween.Kill(animationUid);
        
		var sequence = DOTween.Sequence().SetId(animationUid);
		

		for (int i = 0; i < icons.Count; i++)
		{
			var icon = icons[i];

			icon.sprite = sprite;
			icon.transform.localPosition = Vector3.zero;
			
			delay = 0.1f * i;

			sequence.Insert(delay, icon.DOFade(1, 0.1f));
			sequence.Insert(delay + 0.1f, icon.transform.DOLocalMoveY(-2, duration * 0.5f));
			sequence.Insert(delay + duration * 0.5f, icon.DOFade(0, duration * 0.5f));
		}
		
		DestroyOnBoard(delay + duration);
	}

	public static void AddCard(BoardPosition position, string card)
	{
		var board = BoardService.Current.GetBoardById(0);
		var worldPos = board.BoardDef.GetSectorCenterWorldPosition(position.X, position.Up.Y, position.Z);
		var cardView = board.RendererContext.CreateBoardElementAt<AddCardsView>(R.AddCards, position);

		cardView.CachedTransform.localPosition = cardView.CachedTransform.localPosition + (Vector3.up * 3);
		cardView.Show(IconService.Current.GetSpriteById(card));
        
		board.Manipulator.CameraManipulator.ZoomTo(0.3f, worldPos);
	}
}