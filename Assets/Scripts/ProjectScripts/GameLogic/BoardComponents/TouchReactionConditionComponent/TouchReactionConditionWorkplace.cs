using System;

public class TouchReactionConditionWorkplace : TouchReactionConditionComponent
{
	public override bool Check(BoardPosition position, Piece piece)
	{
		var life = piece.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);

		if (life == null || IsDone) return true;

		if (life.TimerWork.IsPaused || life.IsHideTimer && life.TimerWork.IsExecuteable() || life.IsUseCooldown && life.TimerCooldown.IsPaused) return false;

		IsDone = life.TimerWork.IsExecuteable() == false;
        
		if (IsDone) return true;

		if (life.TimerWork.IsFree())
		{
			life.TimerWork.FastComplete(string.Empty);
			return false;
		}
			
		if (life.TimerWork.GetPrice() == null) return false;
		
		UIMessageWindowController.CreateTimerCompleteMessage(
			LocalizationService.Get("window.timerComplete.message.default", "window.timerComplete.message.default"),
			life.AnalyticsLocation,
			life.TimerWork,
			life.TimerWork.IsCanceled ? life.Cancel : default(Action));
		
		return false;
	}
}