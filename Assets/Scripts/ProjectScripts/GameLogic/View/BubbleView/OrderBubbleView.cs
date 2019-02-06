using UnityEngine;
using UnityEngine.UI;

public class OrderBubbleView : UIBoardView
{
	[SerializeField] private Image mark;
	[SerializeField] private GameObject question;

	protected override ViewType Id => ViewType.OrderBubble;
	
	private CustomerComponent customer;
    
	public override void SetOffset()
	{
		CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Offset;
	}

	public override void Init(Piece piece)
	{
		Offset = new Vector3(0, 2.0f);
		
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

		if (anchor != null) anchor.gameObject.SetActive(customer.Order.State != OrderState.Init);
		
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
		Context.TouchReaction?.Touch(Context.CachedPosition);
	}
}