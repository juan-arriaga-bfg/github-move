using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIMarketTaskItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private NSText reward;
    [SerializeField] private GameObject check;

    public void Init(List<CurrencyPair> rewards, bool isComplete)
    {
        var str = new StringBuilder();
        
        for (var i = 0; i < rewards.Count; i++)
        {
            var pair = rewards[i];
            var lastStr = i < rewards.Count ? "\n" : "";

            str.Append(string.Format("+{0}{1}", pair.ToStringIcon(false), lastStr));
        }

        reward.Text = str.ToString();
        check.SetActive(isComplete);
    }

    public string GetString()
    {
        return reward.Text.Replace("\n", " ");
    }
}