using System.Collections.Generic;
using UnityEngine;

public class CastleUpgradeView : UIBoardView, IBoardEventListener
{
	[SerializeField] private NSText label;
	[SerializeField] private GameObject patern;
	[SerializeField] private GameObject button;
	[SerializeField] private GameObject target;
	
	private List<CastleUpgradeItem> items = new List<CastleUpgradeItem>();

	private TouchReactionDefinitionUpgradeCastle reaction;

	private PieceDef def;
	private bool isComplete;

	public override Vector3 Ofset
	{
		get { return new Vector3(0, 0.7f); }
	}
	
	public override int Priority
	{
		get { return 1; }
	}
	
	protected override void SetOfset()
	{
		CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
	}
	
	public override void Init(Piece piece)
	{
		base.Init(piece);

		label.Text = "Upgrade";
		isComplete = false;
		
		var touchReaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
        
		if(touchReaction == null) return;

		var menu = touchReaction.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
		
		if(menu == null) return;
        
		reaction = menu.GetDefinition<TouchReactionDefinitionUpgradeCastle>();
		
		if(reaction == null) return;
		
		reaction.OnClick = OnClick;
		
		Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceMenu);
		Context.Context.BoardEvents.AddListener(this, GameEventsCodes.CreatePiece);
        
		def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(piece.PieceType);
		
		foreach (var item in def.UpgradePrices)
		{
			items.Add(CreateItem(item));
		}
		
		patern.SetActive(false);
	}

	public override void UpdateVisibility(bool isVisible)
	{
//		viewGo.SetActive(Priority < 0 || isVisible);
		target.SetActive((Priority < 0 || isVisible) && !isComplete);
		button.SetActive((Priority < 0 || isVisible) && isComplete);
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
		
		foreach (var item in items)
		{
			Destroy(item.gameObject);
		}

		items = new List<CastleUpgradeItem>();
		
		patern.SetActive(true);
		
		Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceMenu);
		Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.CreatePiece);
	}
	
	private void OnClick()
	{
		Context.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);
		
		reaction.isOpen = !reaction.isOpen;
		Change(reaction.isOpen);
	}
	
	public void Upgrade()
	{
		reaction.Upgrade(def, Context);
		reaction.isOpen = false;
		Change(false);
	}
	
	public void OnBoardEvent(int code, object context)
	{
		switch (code)
		{
			case GameEventsCodes.ClosePieceMenu:
				if ((context as CastleUpgradeView) == this || reaction.isOpen == false) return;

				reaction.isOpen = false;
				Change(false);
				break;
			case GameEventsCodes.CreatePiece:

				foreach (var item in items)
				{
					if (item.IsComplete == false) return;
				}

				if (isComplete) return;

				isComplete = true;
				OnClick();
				break;
		}
	}

	private CastleUpgradeItem CreateItem(CurrencyPair price)
	{
		var go = Instantiate(patern, patern.transform.parent);
		var item = go.GetComponent<CastleUpgradeItem>();
		
		item.Init(price);
		return item;
	}
}