using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UITavernTaskItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText reward;

    public void Init(List<CurrencyPair> rewards)
    {
        var str = new StringBuilder();
        
        for (var i = 0; i < rewards.Count; i++)
        {
            var pair = rewards[i];
            
            var spriteStr = pair.Currency == Currency.Energy.Name
                ? "Exp"
                : string.Format("<sprite name={0}>", pair.Currency);

            var lastStr = i < rewards.Count ? "\n" : "";

            str.Append(string.Format("+{0}{1}{2}", pair.Amount, spriteStr, lastStr));
        }

        reward.Text = str.ToString();
    }
}