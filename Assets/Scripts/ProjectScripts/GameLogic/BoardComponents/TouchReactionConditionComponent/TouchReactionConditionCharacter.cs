using System.Linq;

public class TouchReactionConditionCharacter : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
    {
        var tutorialDataManager = GameDataService.Current.TutorialDataManager;
        
		if (tutorialDataManager.CheckLockOrders() == false)
		{
			var view = piece.ViewDefinition.AddView(ViewType.Bubble) as BubbleView;

			if (view.IsShow == false)
			{
				view.SetData(
					LocalizationService.Get("common.message.unavailable.orders", "common.message.unavailable.orders"),
					LocalizationService.Get("common.button.ok", "common.button.ok"),
					(p) => { view.Change(false); },
					true,
					false
				);
				
				view.FitToScreen();
			}
			
			view.Change(!view.IsShow);
			return false;
		}
		
		var customer = piece.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);

		if (customer?.Order == null) return false;

		if (customer.Order.State != OrderState.Complete) return true;

	    var amount = customer.Order.GetAmountOfResult();
			
		if(piece.Context.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(position, amount) == false)
		{
            UIErrorWindowController.AddNoFreeSpaceError();
			return false;
		}
		
		NSAudioService.Current.Play(SoundId.OrderClaim, false, 1);
			
		customer.GetReward();
		
		return false;
	}
}