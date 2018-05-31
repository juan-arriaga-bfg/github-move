using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProductionView : UIBoardView, IBoardEventListener
{
	[SerializeField] private GameObject button;
	[SerializeField] private GameObject pattern;
	
	[SerializeField] private Image icon;

	private ProductionComponent production;
	private List<ProductionViewItem> items = new List<ProductionViewItem>();
	
	protected override ViewType Id
	{
		get { return ViewType.Production; }
	}

	public override Vector3 Ofset
	{
		get { { return new Vector3(0, -0.5f); } }
	}
	
	public override void SetOfset()
	{
		CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
	}

	public override void Init(Piece piece)
	{
		base.Init(piece);
		
		production = piece.GetComponent<ProductionComponent>(ProductionComponent.ComponentGuid);
        
		Priority = defaultPriority = 3;
		
		foreach (var pair in production.Storage)
		{
			var item = Instantiate(pattern, pattern.transform.parent).GetComponent<ProductionViewItem>();
			
			item.Init(pair.Key, pair.Value.Value, pair.Value.Key);
			items.Add(item);
		}
		
		icon.sprite = IconService.Current.GetSpriteById(production.Target);
		
		pattern.SetActive(false);
		Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceMenu);
	}
	
	public override void ResetViewOnDestroy()
	{
		Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceMenu);
		
		foreach (var item in items)
		{
			Destroy(item.gameObject);
		}
		
		items = new List<ProductionViewItem>();
		pattern.SetActive(true);
		
		base.ResetViewOnDestroy();
	}

	protected override void UpdateView()
	{
		button.SetActive(production.State == ProductionState.Full);

		var index = 0;
		
		foreach (var pair in production.Storage)
		{
			items[index].Init(pair.Key, pair.Value.Value, pair.Value.Key);
			index++;
		}
	}

	public void OnClick()
	{
		DOTween.Sequence().AppendInterval(0.1f).OnComplete(() =>
		{
			Priority = defaultPriority;
			production.Start();
		});
	}

	public void OnBoardEvent(int code, object context)
	{
		if (code != GameEventsCodes.ClosePieceMenu || context is ProductionComponent && ((ProductionComponent) context) == production) return;
		
		Priority = defaultPriority;

		if (production.State == ProductionState.InProgress)
		{
			Change(false);
			return;
		}
		
		production.Change();
	}
}