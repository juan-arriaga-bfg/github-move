using UnityEngine;
using UnityEngine.UI;

public class UIHeroChargerItem : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private Image back;
	
	[SerializeField] private NSText label;
	
	public void Decoration(CurrencyPair charger, bool isUnlock, bool isCollect)
	{
		if (charger == null)
		{
			icon.gameObject.SetActive(false);
			label.gameObject.SetActive(false);
			return;
		}
		
		var amount = ProfileService.Current.GetStorageItem(charger.Currency).Amount;
		var color = amount >= charger.Amount ? amount.ToString() : string.Format("<color=#F16621>{0}</color>", amount);
		
		icon.gameObject.SetActive(true);
		icon.sprite = IconService.Current.GetSpriteById(charger.Currency);
		back.sprite = IconService.Current.GetSpriteById(string.Format("ramka_item{0}", isUnlock ? "_active" : ""));
		
		label.Text = string.Format("{0}/{1}", color, charger.Amount);
		label.gameObject.SetActive(!isCollect && isUnlock);
	}
}