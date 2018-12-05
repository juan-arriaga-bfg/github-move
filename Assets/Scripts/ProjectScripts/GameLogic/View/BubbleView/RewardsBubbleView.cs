using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardsBubbleView : UIBoardView
{
	[SerializeField] private Image icon;

	protected override ViewType Id => ViewType.RewardsBubble;
    
	public Action OnClickAction;

	public override void SetOfset()
	{
		CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
	}

	public override void Init(Piece piece)
	{
		base.Init(piece);
        
		Ofset = new Vector3(0, 1.5f);
        
		SetOfset();
        
		Priority = defaultPriority = 11;
        
		var storage = piece.GetComponent<RewardsStoreComponent>(RewardsStoreComponent.ComponentGuid);
        
		if (storage == null) return;
        
		icon.sprite = IconService.Current.GetSpriteById(storage.Icon);
	}
	
	public void OnClick()
	{
		Context.Context.TutorialLogic.Pause(true);
		OnClickAction?.Invoke();
		OnClickAction = null;
	}
}