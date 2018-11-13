using UnityEngine;
using UnityEngine.UI;

public class OrderBubbleView : UIBoardView
{
	[SerializeField] private Image icon;
	[SerializeField] private Image mark;
	[SerializeField] private GameObject question;

	protected override ViewType Id => ViewType.OrderBubble;
	
	private CustomerComponent customer;
    
	public override void SetOfset()
	{
		CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
	}

	public override void Init(Piece piece)
	{
		Ofset = new Vector3(0, 2.0f);
		
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
		
		customer.Order.SetMark(mark);
		
		icon.gameObject.SetActive(customer.Order.State != OrderState.Init);
		question.SetActive(customer.Order.State == OrderState.Init);
		
		icon.sprite = IconService.Current.GetSpriteById(customer.Order.Def.Uid);
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