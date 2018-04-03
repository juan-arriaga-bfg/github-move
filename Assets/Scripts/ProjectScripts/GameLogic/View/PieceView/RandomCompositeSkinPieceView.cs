using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Skin
{
	public int Weight;
	public string Name;
	public Vector2 Offset;
	public float Scale = 1;
}

public class RandomCompositeSkinPieceView : PieceBoardElementView
{
	[SerializeField] private SpriteRenderer image;

	[SerializeField] private List<Skin> skins;

	private int totalWeight;
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
		if (totalWeight == 0) CalculationTotalWeight();
		if (totalWeight == 0) return;
		
		var skin = GetSkin();
		
		image.sprite = IconService.Current.GetSpriteById(skin.Name);
		image.transform.localPosition = skin.Offset;
		image.transform.localScale = Vector3.one * skin.Scale;
	}

	private Skin GetSkin()
	{
		var curWeight = 0;
		var randomValue = Random.Range(0, totalWeight + 1);
		
		foreach (var skin in skins)
		{
			curWeight += skin.Weight;
			if (curWeight >= randomValue)
			{
				return skin;
			}
		}

		return skins[0];
	}

	private void CalculationTotalWeight()
	{
		foreach (var skin in skins)
		{
			totalWeight += skin.Weight;
		}
	}
}