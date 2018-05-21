using UnityEngine;
using UnityEngine.UI;

public class UIMarketTargetItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText target;
    [SerializeField] private GameObject check;
    
    public void Init(CurrencyPair pair)
    {
        var isComplete = CurrencyHellper.IsCanPurchase(pair);
        var current = ProfileService.Current.GetStorageItem(pair.Currency).Amount;
        
        icon.sprite = IconService.Current.GetSpriteById(pair.Currency);
        target.Text = string.Format("<color=#{0}><size=32>{1}</size></color>/{2}", isComplete ? "FFFFFF" : "FE4704", current, pair.Amount);
        check.SetActive(isComplete);
    }
}