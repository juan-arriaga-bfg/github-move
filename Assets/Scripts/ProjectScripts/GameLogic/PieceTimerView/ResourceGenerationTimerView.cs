using System;
using UnityEngine;

public class ResourceGenerationTimerView : IWBaseMonoBehaviour
{
	[SerializeField] private BoardProgressBarView boardProgressBarView;
	
	[SerializeField] private NSText boardProgressBarLabel;
	
	[SerializeField] private Transform completeView;
	
	private PieceBoardElementView context;

	private TouchReactonConditionDelay touchReactonConditionDelay;

	public virtual void Init(PieceBoardElementView context)
	{
		this.context = context;
	}

	private void OnDisable()
	{
		touchReactonConditionDelay = null;
	}

	private void Update()
	{
		if (context == null || context.Piece == null) return;

		if (touchReactonConditionDelay == null)
		{
			var touchReactionComponent = context.Piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
			touchReactonConditionDelay = touchReactionComponent.GetComponent<TouchReactonConditionDelay>(TouchReactonConditionDelay.ComponentGuid);
		}
		
		if (touchReactonConditionDelay == null) return;

		var currentSeconds = (float)(DateTime.Now - touchReactonConditionDelay.StartTime).TotalSeconds;
		var targetSeconds = touchReactonConditionDelay.Delay;

		if (currentSeconds > targetSeconds)
		{
			completeView.gameObject.SetActive(true);
			boardProgressBarView.gameObject.SetActive(false);
			
			return;
		}

		completeView.gameObject.SetActive(false);
		boardProgressBarView.gameObject.SetActive(true);
		
		float progress = currentSeconds / (float) targetSeconds;
		
		boardProgressBarView.SetProgress(progress);

		boardProgressBarLabel.Text = GetFormattedTime(TimeSpan.FromSeconds(targetSeconds - currentSeconds));

	}
	
	public string GetFormattedTime(TimeSpan time)
	{
		var str = "";

		if ((int) time.TotalHours > 0)
		{
			str = string.Format("{0}:{1}", time.Hours, (time.Minutes > 9 ? "" : "0") + time.Minutes);
		}
		else
		{
			str = string.Format("{0}:{1}", (time.Minutes > 9 ? "" : "0") + time.Minutes,
				(time.Seconds > 9 ? "" : "0") + time.Seconds);
		}

		return str;
	}
}
