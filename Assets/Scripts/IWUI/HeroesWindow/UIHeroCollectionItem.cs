using UnityEngine;
using UnityEngine.UI;

public class UIHeroCollectionItem : MonoBehaviour
{
	[SerializeField] private Image icon;
	
	[SerializeField] private GameObject check;
	[SerializeField] private GameObject question;

	private bool isCollect;
	
	public void Decoration(string currency)
	{
		icon.sprite = IconService.Current.GetSpriteById(currency);
		
		isCollect = ProfileService.Current.GetStorageItem(currency).Amount > 0;
		
		check.SetActive(isCollect);
		question.SetActive(!isCollect);
	}
	
	public void OnClick()
	{
        if(isCollect) return;
		
		Debug.LogError("Tadam!!!!!!!!");
	}
}