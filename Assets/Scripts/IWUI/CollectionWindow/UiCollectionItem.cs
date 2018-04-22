using UnityEngine;
using UnityEngine.UI;

public class UiCollectionItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    
    public void Decoration(string currency)
    {
        icon.sprite = IconService.Current.GetSpriteById(currency);
    }
}