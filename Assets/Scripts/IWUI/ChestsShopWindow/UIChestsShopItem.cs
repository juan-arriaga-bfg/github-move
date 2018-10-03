using UnityEngine;
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
	public int reward = -1;
	
	private bool isFree;
	private bool isClick;
	
	public void Init(ChestDef def)
	{
		model = context.Model as UIChestsShopWindowModel;
		chest = def;
		
		isClick = false;
		isFree = GameDataService.Current.LevelsManager.Chest == chest.Piece;
		
		if (isFree)
		{
			model.FreeChestLogic.Timer.OnExecute += UpdateLabel;
			model.FreeChestLogic.Timer.OnComplete += ChengeButtons;
		}

		ChengeButtons();
		
		label.Text = chest.Uid;
		labelBtn.Text = isFree ? "Claim!" : $"Buy for {chest.Price.ToStringIcon(false)}";
		
		icon.sprite = IconService.Current.GetSpriteById(chest.Uid);
	}
	
	public override void OnViewCloseCompleted()
	{
		if (isFree)
		{
			model.FreeChestLogic.Timer.OnExecute -= UpdateLabel;
			model.FreeChestLogic.Timer.OnComplete -= ChengeButtons;
		}
		
		if (isClick == false || reward == -1) return;
		
		var board = BoardService.Current.GetBoardById(0);
		var position = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1)[0];
		var piece = board.BoardLogic.GetPieceAt(position);
        
		var menu = piece.TouchReaction?.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
		var spawn = menu?.GetDefinition<TouchReactionDefinitionSpawnShop>();
        
		if(spawn == null) return;

		spawn.Reward = reward;
		reward = -1;
		model.FreeChestLogic.Timer.Start();
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
		var pos = board.BoardLogic.PositionsCache.GetRandomPositions(PieceType.Char1.Id, 1)[0];
		
		if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(pos, 1))
		{
			isClick = false;
			UIErrorWindowController.AddError("Free space not found");
			return;
		}
		
		if (isFree)
		{
			isClick = false;
			OnClickFree();
			return;
		}

		OnClickPaid();
	}
	
	private void OnClickFree()
	{
		if (model.FreeChestLogic.Timer.IsExecuteable())
		{
			UIErrorWindowController.AddError("Production of the resource is not complete!");
			return;
		}
		
		reward = chest.Piece;
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
			
			reward = chest.Piece;
			context.Controller.CloseCurrentWindow();
		});
	}
}