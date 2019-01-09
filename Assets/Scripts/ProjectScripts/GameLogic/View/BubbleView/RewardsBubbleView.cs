using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardsBubbleView : UIBoardView
{
	[SerializeField] private List<Image> borders;

	protected override ViewType Id => ViewType.RewardsBubble;

	private RewardsStoreComponent storage;
	
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
        
		storage = piece.GetComponent<RewardsStoreComponent>(RewardsStoreComponent.ComponentGuid);
        
		if (storage == null || string.IsNullOrEmpty(storage.Icon) || storage.Icon == PieceType.Parse(PieceType.Empty.Id)) return;
		
		CreateIcon(storage.Icon);
	}

	protected override void UpdateView()
	{
		if (storage == null) return;
		
		foreach (var border in borders)
		{
			border.color = storage.IsHightlight ? Color.red : Color.white;
		}
	}

	public void OnClick()
	{
		Context.Context.TutorialLogic.Pause(true);
		
		if (storage != null && storage.CheckOutOfCells()) return;
		
		OnClickAction?.Invoke();
		OnClickAction = null;
	}
}