using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardsBubbleView : UIBoardView
{
	[SerializeField] private List<Image> borders;
	
	public override bool IsTop => true;

	protected override Vector3 offset => new Vector3(0, 1.3f);
	
	protected override ViewType Id => ViewType.RewardsBubble;

	private RewardsStoreComponent storage;
	
	public Action OnClickAction;
	
	public override void Init(Piece piece)
	{
		base.Init(piece);
        
		Priority = defaultPriority = 11;
        
		storage = Context.GetComponent<RewardsStoreComponent>(RewardsStoreComponent.ComponentGuid);

		UpdateIcon();
	}

	public void UpdateIcon()
	{
		if (storage == null || string.IsNullOrEmpty(storage.Icon) || storage.Icon == PieceType.Parse(PieceType.Empty.Id)) return;
		
		CreateIcon(storage.Icon);
	}

	protected override void UpdateView()
	{
		if (storage == null) return;
		
		foreach (var border in borders)
		{
			border.color = storage.IsHighlight ? Color.red : Color.white;
		}
	}

	public void OnClick()
	{
		Context.Context.TutorialLogic.Pause(true);
		
		if (Context.Context.BoardLogic.IsLockedCell(Context.CachedPosition) || storage != null && storage.CheckOutOfCells()) return;
		
		OnClickAction?.Invoke();
		OnClickAction = null;
	}
}