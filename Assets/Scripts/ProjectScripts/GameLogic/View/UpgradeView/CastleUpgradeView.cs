using System.Collections.Generic;
using UnityEngine;

public class CastleUpgradeView : UIBoardView, IBoardEventListener
{
	[SerializeField] private NSText label;
	[SerializeField] private GameObject patern;
	[SerializeField] private GameObject button;
	[SerializeField] private GameObject target;
	
	private List<CastleUpgradeItem> items = new List<CastleUpgradeItem>();

	private CastleUpgradeComponent upgrade;
	
	public override Vector3 Ofset
	{
		get { return new Vector3(0, 1.5f); }
	}
	
	protected override void SetOfset()
	{
		CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
	}
	
	public override void Init(Piece piece)
	{
		base.Init(piece);
		
		priority = 2;
		label.Text = "Upgrade";
		
		upgrade = piece.GetComponent<CastleUpgradeComponent>(CastleUpgradeComponent.ComponentGuid);
		
		if(upgrade == null) return;
		
		Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceMenu);
		
		foreach (var quest in upgrade.Prices)
		{
			items.Add(CreateItem(quest));
		}
		
		patern.SetActive(false);
	}

	protected override void UpdateView()
	{
		var isComplete = upgrade.Check();
		
		target.SetActive(!isComplete);
		button.SetActive(isComplete);
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
	}
	
	public void Upgrade()
	{
		var definition = Context.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);

		if (definition == null) return;
        
		definition.Touch(Context.CachedPosition);
	}
	
	public void OnBoardEvent(int code, object context)
	{
		if(code != GameEventsCodes.ClosePieceMenu || context is Piece && (Piece)context == Context) return;
		
		Change(false);
	}

	private CastleUpgradeItem CreateItem(Quest quest)
	{
		var go = Instantiate(patern, patern.transform.parent);
		var item = go.GetComponent<CastleUpgradeItem>();
		
		item.Init(quest);
		return item;
	}
}