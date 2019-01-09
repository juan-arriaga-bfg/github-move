using System.Collections.Generic;
using UnityEngine.UI;

public class UIMarketElementViewController : UISimpleScrollElementViewController
{
	[IWUIBinding("#NameLabel")] private NSText nameLabel;
	[IWUIBinding("#ButtonLabel")] private NSText btnLabel;
	
	[IWUIBinding("#BuyButton")] private UIButtonViewController btnBuy;
	[IWUIBinding("#ButtonInfo")] private UIButtonViewController btnInfo;
	
	[IWUIBinding("#ButtonBack")] private Image btnBack;
	
	private bool isFree;
	private bool isClick;
	private bool isReward;

	public override void Init()
	{
		base.Init();
		
		var contentEntity = entity as UIMarketElementEntity;
		
		isClick = false;
		isReward = false;
		isFree = contentEntity.Def.Price == null;

		nameLabel.Text = contentEntity.Name;
		
		ChengeButtons();
		
		btnLabel.Text = isFree
			? LocalizationService.Get("common.button.claim", "common.button.claim")
			: string.Format(LocalizationService.Get("common.button.buyFor", "common.button.buyFor {0}"), contentEntity.Def.Price.ToStringIcon());
	}

	public override void OnViewShowCompleted()
	{
		base.OnViewShowCompleted();
		
		btnBuy
			.ToState(GenericButtonState.Active)
			.OnClick(OnClick);
		
		btnInfo
			.ToState(GenericButtonState.Active)
			.OnClick(OnClickInfo);
	}

	public override void OnViewCloseCompleted()
	{
		var contentEntity = entity as UIMarketElementEntity;
		
		if(contentEntity == null) return;
		
		if (isClick == false || isReward == false) return;
		
		var board = BoardService.Current.FirstBoard;
		var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
		var reward = new CurrencyPair {Currency = contentEntity.Def.Weight.Uid, Amount = contentEntity.Def.Amount};
		
		List<CurrencyPair> currencysReward;
		var piecesReward = CurrencyHellper.FiltrationRewards(new List<CurrencyPair>{reward}, out currencysReward);
		
		board.ActionExecutor.AddAction(new EjectionPieceAction
		{
			GetFrom = () => position,
			Pieces = piecesReward,
			OnComplete = () =>
			{
				var view = board.RendererContext.GetElementAt(position) as CharacterPieceView;
                
				if(view != null) view.StartRewardAnimation();
                    
				AddResourceView.Show(position, currencysReward);
			}
		});
	}
	
	private void ChengeButtons()
	{
		btnBack.sprite = IconService.Current.GetSpriteById($"button{(isFree ? "Green" : "Blue")}");
	}

	private void OnClickInfo()
	{
		UIMessageWindowController.CreateNotImplementedMessage();
	}

	private void OnClick()
	{
		if(isClick) return;
		
		isClick = true;
		
		var board = BoardService.Current.FirstBoard;
		var pos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
		
		if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(pos, 1))
		{
			isClick = false;
			UIErrorWindowController.AddError(LocalizationService.Get("message.error.freeSpace", "message.error.freeSpace"));
			return;
		}
		
		if (isFree)
		{
			OnClickFree();
			return;
		}

		OnClickPaid();
	}
	
	private void OnClickFree()
	{
		isReward = true;
		context.Controller.CloseCurrentWindow();
	}

	private void OnClickPaid()
	{
		var contentEntity = entity as UIMarketElementEntity;
		
		CurrencyHellper.Purchase(Currency.Chest.Name, 1, contentEntity.Def.Price, success =>
		{
			if (success == false)
			{
				isClick = false;
				return;
			}
			
			isReward = true;
			context.Controller.CloseCurrentWindow();
		});
	}
}