using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Skin
{
	public int Weight;
	public string Name;
	public Vector2 Offset;
	public float Scale = 1;
}

public abstract class PieceSkin : IWBaseMonoBehaviour
{
	[SerializeField] private SpriteRenderer image;
	[SerializeField] protected List<Skin> skins;

	public virtual void Init(int value)
	{
		UpdateView(value);
	}

	public virtual void UpdateView()
	{
	}
	
	protected void UpdateView(int index)
	{
		var skin = skins[index];
        
		image.sprite = IconService.Current.GetSpriteById(skin.Name);
		image.transform.localPosition = skin.Offset;
		image.transform.localScale = Vector3.one * skin.Scale;
	}
}
