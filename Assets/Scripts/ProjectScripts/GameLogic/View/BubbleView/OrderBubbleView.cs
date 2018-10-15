using UnityEngine;
using UnityEngine.UI;

public class OrderBubbleView : UIBoardView
{
	[SerializeField] private Image icon;
	[SerializeField] private Image mark;

	protected override ViewType Id => ViewType.OrderBubble;
	
	private CustomerComponent customer;
    
	public override void SetOfset()
	{
		CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
	}

	public override void Init(Piece piece)
	{
		Ofset = new Vector3(0, 2f);
		
		base.Init(piece);
        
		Priority = defaultPriority = -1;
		
		customer = piece.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
		if(customer?.Order == null) return;

		customer.Order.OnStateChange += UpdateIcon;
		UpdateIcon();
	}

	public void UpdateIcon()
	{
		if (customer == null) return;
		
		mark.gameObject.SetActive(customer.Order.State == OrderState.Waiting || customer.Order.State == OrderState.Complete);
		
		icon.sprite = IconService.Current.GetSpriteById(customer.Order.State == OrderState.Init ? "codexQuestion" : customer.Order.Def.Uid);
		
		if(customer.Order.State == OrderState.Waiting) mark.sprite = IconService.Current.GetSpriteById("icon_Warning");
		if(customer.Order.State == OrderState.Complete) mark.sprite = IconService.Current.GetSpriteById("icon_Complete");
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
		Context.TouchReaction?.Touch(Context.CachedPosition);
	}
}