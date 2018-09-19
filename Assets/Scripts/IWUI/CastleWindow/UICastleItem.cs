using UnityEngine;
using UnityEngine.UI;

public class UICastleItem : MonoBehaviour
{
	[SerializeField] private Image icon;
	
	[SerializeField] private NSText label;
	[SerializeField] private NSText labelBtn;
	[SerializeField] private NSText labelTimer;
	
	[SerializeField] private GameObject timer;
	[SerializeField] private GameObject button;

	private ChestDef chest;
	private bool isFree;
	private bool isClick;
	private UICastleWindowModel model;
    
	public void Init(ChestDef def)
	{
		chest = def;

		isClick = false;
		
		model = UIService.Get.GetCachedModel<UICastleWindowModel>(UIWindowType.CastleWindow);

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

	private void OnDisable()
	{
		if(isFree == false) return;

		model.FreeChestLogic.Timer.OnExecute -= UpdateLabel;
		model.FreeChestLogic.Timer.OnComplete -= ChengeButtons;
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
		
		model.ChestReward = chest.Piece;
		UIService.Get.CloseWindow(UIWindowType.CastleWindow, true);
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
			
			model.ChestReward = chest.Piece;
			UIService.Get.CloseWindow(UIWindowType.CastleWindow, true);
		});
	}
}