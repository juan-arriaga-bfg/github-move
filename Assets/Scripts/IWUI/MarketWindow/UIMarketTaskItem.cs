using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIMarketTaskItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText reward;

    public void Init(List<CurrencyPair> rewards)
    {
        var str = new StringBuilder();
        
        for (var i = 0; i < rewards.Count; i++)
        {
            var pair = rewards[i];
            var lastStr = i < rewards.Count ? "\n" : "";

            str.Append(string.Format("+{0}{1}", pair.ToStringIcon(false), lastStr));
        }

        reward.Text = str.ToString();
    }

    public string GetString()
    {
        return reward.Text.Replace("\n", " ");
    }
}