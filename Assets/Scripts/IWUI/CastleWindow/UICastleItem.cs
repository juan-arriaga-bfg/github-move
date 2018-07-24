using UnityEngine;
using UnityEngine.UI;

public class UICastleItem : MonoBehaviour
{
	[SerializeField] private Image iconTop;
	[SerializeField] private Image iconBottom;
	
	[SerializeField] private NSText label;
	[SerializeField] private NSText labelBtn;
	[SerializeField] private NSText labelTimer;
	
	[SerializeField] private GameObject timer;
	[SerializeField] private GameObject button;

	private ChestDef chest;
	private bool isFree;
	private UICastleWindowModel model;
    
	public void Init(ChestDef def)
	{
		chest = def;
		
		model = UIService.Get.GetCachedModel<UICastleWindowModel>(UIWindowType.CastleWindow);

		isFree = model.Storage.SpawnPiece == chest.Piece;
		
		if (isFree)
		{
			model.Storage.Timer.OnExecute += UpdateLabel;
			model.Storage.Timer.OnComplete += ChengeButtons;
		}

		ChengeButtons();
		
		label.Text = chest.Uid;
		labelBtn.Text = isFree ? "Claim!" : string.Format("Buy for {0}", chest.Price.ToStringIcon(false));
		
		var key = GetChestIcon();

		iconTop.sprite = IconService.Current.GetSpriteById(string.Format("chest_{0}_2", key));
		iconBottom.sprite = IconService.Current.GetSpriteById(string.Format("chest_{0}_1", key));
	}

	private void OnDisable()
	{
		if(isFree == false) return;

		model.Storage.Timer.OnExecute -= UpdateLabel;
		model.Storage.Timer.OnComplete -= ChengeButtons;
	}

	private void UpdateLabel()
	{
		labelTimer.Text = model.Storage.Timer.GetTimeLeftText(null);
	}

	private void ChengeButtons()
	{
		var isActive = isFree && model.Storage.Timer.IsExecuteable();
		
		timer.SetActive(isActive);
		button.SetActive(!isActive);
	}

	public void OnClick()
	{
		if (isFree)
		{
			OnClickFree();
			return;
		}

		OnClickPaid();
	}
	
	private void OnClickFree()
	{
		if (model.Storage.Timer.IsExecuteable())
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
			if(success == false) return;
			
			model.ChestReward = chest.Piece;
			UIService.Get.CloseWindow(UIWindowType.CastleWindow, true);
		});
	}

	private string GetChestIcon()
	{
		var key = chest.Piece;
		
		if (key == PieceType.ChestA3.Id)
		{
			return "2";
		}
		
		if (key == PieceType.ChestX3.Id)
		{
			return "3";
		}
		
		if (key == PieceType.ChestC3.Id)
		{
			return "4";
		}

		return "1";
	}
}