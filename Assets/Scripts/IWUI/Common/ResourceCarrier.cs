using UnityEngine;
using UnityEngine.UI;

public class ResourceCarrier : ResourceCarrierBase
{
    [SerializeField] private Image icon;
	[SerializeField] private Image iconShadow;

	public override IResourceCarrier Init(IResourceCarrierView view, int offset)
	{
		iconShadow.sprite = icon.sprite = IconService.Instance.Manager.GetSpriteById($"icon_{view.GetResourceId()}");
		
		return base.Init(view, offset);
	}
}