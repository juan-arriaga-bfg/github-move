﻿using UnityEngine;
using UnityEngine.UI;

public class OrderBubbleView : UIBoardView
{
	[SerializeField] private Image mark;
	[SerializeField] private GameObject clock;
	[SerializeField] private GameObject question;

	protected override ViewType Id => ViewType.OrderBubble;
	public override bool IsTop => true;

	private CustomerComponent customer;
    
	public override void Init(Piece piece)
	{
		base.Init(piece);
        
		Priority = defaultPriority = -1;
		
		customer = piece.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
		if(customer?.Order == null) return;

		customer.Order.OnStateChange += UpdateIcon;
		UpdateIcon();
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
		
		if(customer?.Order == null) return;
		
		customer.Order.OnStateChange -= UpdateIcon;
		customer = null;
	}

	public void UpdateIcon()
	{
		if (customer == null) return;
		
		customer.Order.SetMark(mark, clock);

		if (anchor != null) anchor.gameObject.SetActive(customer.Order.State != OrderState.Init && customer.Order.State != OrderState.InProgress);
		
		question.SetActive(customer.Order.State == OrderState.Init);
		
		if (string.IsNullOrEmpty(customer.Order.Def.Uid) || customer.Order.Def.Uid == PieceType.Parse(PieceType.Empty.Id)) return;
		
		CreateIcon(customer.Order.Def.Uid);
	}

	public override void OnSwap(bool isEnd)
	{
		base.OnSwap(isEnd);
		
		if (customer == null) return;

		customer.Timer.IsPaused = !isEnd;
		customer.Cooldown.IsPaused = !isEnd;
	}

	public override void OnDrag(bool isEnd)
	{
		base.OnDrag(isEnd);
        
		if (customer == null) return;

		customer.Timer.IsPaused = !isEnd;
		customer.Cooldown.IsPaused = !isEnd;
	}

	public void OnClick()
	{
		Context.Context.TutorialLogic.Pause(true);
		
		if (Context.Context.BoardLogic.IsLockedCell(Context.CachedPosition)) return;
		
		Context.TouchReaction?.Touch(Context.CachedPosition);
	}
}