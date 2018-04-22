using UnityEngine;
using UnityEngine.UI;

public class UIHeroChargerItem : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private NSText label;
	
	public void Decoration(CurrencyPair charger)
	{
		var amount = ProfileService.Current.GetStorageItem(charger.Currency).Amount;
		var isCollect = amount > charger.Amount;
		var color = isCollect ? "" : "<color=#F16621>{2}</color>";
		
		icon.sprite = IconService.Current.GetSpriteById(charger.Currency);
		
		label.Text = string.Format("{0}/{1}", color, charger.Amount, amount);
		label.gameObject.SetActive(isCollect);
	}
}