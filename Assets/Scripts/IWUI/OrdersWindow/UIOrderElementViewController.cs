using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIOrderElementViewController : UISimpleScrollElementViewController
{
	[IWUIBinding] private UIOrdersTabButtonViewController button;
	[IWUIBinding("#Mark")] private Image mark;
	[IWUIBinding("#Clock")] private GameObject clock;
	[IWUIBinding("#Token")] private GameObject token;
	
	public override void Init()
	{
		base.Init();
        
		button
			.SetActiveScale(1.2f)
			.ToState(GenericButtonState.UnActive);
		
		var contentEntity = entity as UIOrderElementEntity;
		
		contentEntity.Data.OnStateChange += UpdateMark;

		UpdateMark();

		var eventGameIsActive = BoardService.Current.FirstBoard.BoardLogic.EventGamesLogic.GetEventGame(EventGameType.OrderSoftLaunch, out var eventGame) && eventGame.State == EventGameState.InProgress;
		
		token.SetActive(eventGameIsActive);
		token.transform.localScale = Vector3.one;
	}

	private void OnDisable()
	{
		if(!(entity is UIOrderElementEntity contentEntity)) return;
		
		contentEntity.Data.OnStateChange -= UpdateMark;
	}

	private void UpdateMark()
	{
		var contentEntity = entity as UIOrderElementEntity;
		
		contentEntity.Data.SetMark(mark, clock);
	}

	public override void OnViewShowCompleted()
	{
		base.OnViewShowCompleted();
		
		button.OnClick(Select);
		DOTween.Kill(token);
	}
	
	public override void OnSelect()
	{
		base.OnSelect();
        
		button.ToState(GenericButtonState.Active);
	}
	
	public override void OnDeselect()
	{
		base.OnDeselect();
        
		button.ToState(GenericButtonState.UnActive);
	}

	public void HighlightToken()
	{
		DOTween.Kill(token);
		
		var sequence = DOTween.Sequence().SetId(token);

		sequence.SetLoops(3);
		sequence.Append(token.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack));
		sequence.Append(token.transform.DOScale(1f, 0.15f));
	}
}