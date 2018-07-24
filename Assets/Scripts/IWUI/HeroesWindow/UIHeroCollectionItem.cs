using UnityEngine;
using UnityEngine.UI;

public class UIHeroCollectionItem : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private Image back;
	
	[SerializeField] private GameObject check;
	[SerializeField] private GameObject question;

	private bool isCollect;
	
	public void Decoration(string currency, bool isUnlock)
	{
		icon.sprite = IconService.Current.GetSpriteById(currency);
		icon.color = new Color(1, 1, 1, isUnlock ? 1 : 0.3f);
		
		isCollect = ProfileService.Current.GetStorageItem(currency).Amount > 0;
		
		check.SetActive(isCollect && isUnlock);
		question.SetActive(!isCollect && isUnlock);
		back.sprite = IconService.Current.GetSpriteById(string.Format("ramka_item{0}", isUnlock ? "_active" : ""));
	}
	
	public void OnClick()
	{
        if(isCollect) return;
		
		Debug.LogError("Tadam!!!!!!!!");
	}
}