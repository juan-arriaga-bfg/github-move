﻿using UnityEngine;
using UnityEngine.UI;

public class UIChestsShopItem : IWUIWindowViewController
{
	[SerializeField] private Image icon;
	
	[SerializeField] private NSText label;
	[SerializeField] private NSText labelBtn;
	[SerializeField] private NSText labelTimer;
	
	[SerializeField] private GameObject timer;
	[SerializeField] private GameObject button;
	
	private UIChestsShopWindowModel model;
	
	private ChestDef chest;
	
	private bool isFree;
	private bool isClick;
	private bool isReward;
	
	public void Init(ChestDef def)
	{
		model = context.Model as UIChestsShopWindowModel;
		chest = def;
		
		isClick = false;
		isReward = false;
		isFree = PieceType.CH_Free.Id == chest.Piece;
		
		if (isFree)
		{
			model.FreeChestLogic.Timer.OnExecute += UpdateLabel;
			model.FreeChestLogic.Timer.OnComplete += ChengeButtons;
		}

		ChengeButtons();

		label.Text = LocalizationService.Instance.Manager.GetTextByUid($"piece.name.{chest.Uid}", "Chest");
		labelBtn.Text = isFree
			? LocalizationService.Instance.Manager.GetTextByUid("common.button.claim", "Claim!")
			: string.Format(LocalizationService.Instance.Manager.GetTextByUid("common.button.buyFor", "Buy for {0}"), chest.Price.ToStringIcon(false));
		
		icon.sprite = IconService.Current.GetSpriteById(chest.Uid);
	}
	
	public override void OnViewCloseCompleted()
	{
		if (isFree)
		{
			model.FreeChestLogic.Timer.OnExecute -= UpdateLabel;
			model.FreeChestLogic.Timer.OnComplete -= ChengeButtons;
		}
		
		if (isClick == false || isReward == false) return;
		
		var board = BoardService.Current.GetBoardById(0);
		var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
		var piece = board.BoardLogic.GetPieceAt(position);
        
		var menu = piece.TouchReaction?.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
		var spawn = menu?.GetDefinition<TouchReactionDefinitionSpawnShop>();
        
		if(spawn == null) return;

		spawn.Reward = chest.Piece;
		
		if(isFree) model.FreeChestLogic.Timer.Start();
		
		spawn.Make(piece.CachedPosition, piece);
	}

	private void UpdateLabel()
	{
		labelTimer.Text = model.FreeChestLogic.Timer.CompleteTime.GetTimeLeftText();
	}

	private void ChengeButtons()
	{
		var isActive = isFree && model.FreeChestLogic.Timer.IsExecuteable();
		
		timer.SetActive(isActive);
		button.SetActive(!isActive);
	}

	public void OnClick()
	{
		if(isClick) return;
		
		isClick = true;
		
		var board = BoardService.Current.GetBoardById(0);
		var pos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.NPC_SleepingBeauty.Id, 1)[0];
		
		if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(pos, 1))
		{
			isClick = false;
			UIErrorWindowController.AddError(LocalizationService.Instance.Manager.GetTextByUid("message.error.freeSpace", "Free space not found!"));
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
		if (model.FreeChestLogic.Timer.IsExecuteable())
		{
			UIErrorWindowController.AddError(LocalizationService.Instance.Manager.GetTextByUid("message.error.notComplete", "Production of the resource is not complete!"));
			return;
		}
		
		isReward = true;
		context.Controller.CloseCurrentWindow();
	}

	private void OnClickPaid()
	{
		CurrencyHellper.Purchase(Currency.Chest.Name, 1, chest.Price, success =>
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