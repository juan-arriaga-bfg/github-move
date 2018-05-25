using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIMarketTaskItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText reward;
    [SerializeField] private GameObject check;

    public void Init(string character, Dictionary<int, int> result, bool isComplete)
    {
        var amount = 0;

        foreach (var pair in result)
        {
            var def = GameDataService.Current.PiecesManager.GetPieceDef(pair.Key);
                
            if(def == null) continue;

            amount += def.SpawnResources.Amount * pair.Value;
        }
        
        icon.sprite = IconService.Current.GetSpriteById(character);
        reward.Text = string.Format("+{0}<sprite name={1}>", amount, Currency.Coins.Name);
        check.SetActive(isComplete);
    }

    public string GetString()
    {
        return reward.Text;
    }
}