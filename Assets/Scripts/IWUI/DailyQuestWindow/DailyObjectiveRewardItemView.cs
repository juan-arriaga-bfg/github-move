using UnityEngine;
using UnityEngine.UI;

public class DailyObjectiveRewardItemView : IWBaseMonoBehaviour
{
    [SerializeField] private NSText lblCount;
    [SerializeField] private Image image;

    public void Init(CurrencyPair reward)
    {
        lblCount.Text = reward.Amount > 1 ? "x" + reward.Amount : string.Empty;
        image.sprite = reward.GetIcon();
        image.SetNativeSize();
    }
}