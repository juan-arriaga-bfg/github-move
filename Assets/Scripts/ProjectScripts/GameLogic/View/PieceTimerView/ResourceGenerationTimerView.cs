﻿using System;
using UnityEngine;

public class ResourceGenerationTimerView : IWBaseMonoBehaviour
{
	[SerializeField] private BoardProgressBarView boardProgressBarView;
	
	[SerializeField] private NSText boardProgressBarLabel;
	
	[SerializeField] private Transform completeViewAnchor;
	
	private PieceBoardElementView context;

	private TouchReactionConditionDelay _touchReactionConditionDelay;
	
	private Vector3 arrowOffset;

	public virtual void Init(PieceBoardElementView context, Vector3 arrowOffset)
	{
		this.context = context;
		this.arrowOffset = arrowOffset;

		completeViewAnchor.localPosition = completeViewAnchor.localPosition + arrowOffset;
	}

	private void OnDisable()
	{
		_touchReactionConditionDelay = null;
	}

	private void Awake()
	{
		foreach (Transform view in completeViewAnchor)
		{
			DestroyImmediate(view.gameObject);
		}

		var go = (GameObject)Instantiate(ContentService.Instance.Manager.GetObjectByName(R.ReadyArrowView));
		var objComponent = go.GetComponent<Transform>();

		objComponent.SetParent(completeViewAnchor);
		objComponent.localPosition = Vector3.zero;
		objComponent.localRotation = Quaternion.identity;
		objComponent.localScale = Vector3.one;
	}

	private void Update()
	{
		if (context == null || context.Piece == null) return;

		if (_touchReactionConditionDelay == null)
		{
			var touchReactionComponent = context.Piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
			_touchReactionConditionDelay = touchReactionComponent.GetComponent<TouchReactionConditionDelay>(TouchReactionConditionDelay.ComponentGuid);
		}
		
		if (_touchReactionConditionDelay == null) return;

		var currentSeconds = (float)(DateTime.Now - _touchReactionConditionDelay.StartTime).TotalSeconds;
		var targetSeconds = _touchReactionConditionDelay.Delay;

		if (currentSeconds > targetSeconds)
		{
			completeViewAnchor.gameObject.SetActive(true);
			boardProgressBarView.gameObject.SetActive(false);
			
			return;
		}

		completeViewAnchor.gameObject.SetActive(false);
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
