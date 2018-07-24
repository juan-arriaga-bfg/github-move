using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionWarningView : UIBoardView
{
	[SerializeField] private GameObject shine;
	[SerializeField] private GameObject warning;
	
	[SerializeField] private Image icon;

	private ProductionComponent production;
	
	protected override ViewType Id
	{
		get { return ViewType.ProductionWarning; }
	}
	
	public override Vector3 Ofset
	{
		get { { return new Vector3(0, 1.3f); } }
	}
	
	public override void SetOfset()
	{
		CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
	}

	public override void Init(Piece piece)
	{
		base.Init(piece);

		production = piece.GetComponent<ProductionComponent>(ProductionComponent.ComponentGuid);
        
		Priority = defaultPriority = 5;
		
		icon.sprite = IconService.Current.GetSpriteById(production.Target);
	}

	protected override void UpdateView()
	{
		shine.SetActive(production.State == ProductionState.Completed);
		warning.SetActive(production.State == ProductionState.Full);
	}
}