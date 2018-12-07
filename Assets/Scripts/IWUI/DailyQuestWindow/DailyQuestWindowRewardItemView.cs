using UnityEngine;
using UnityEngine.UI;

public class DailyQuestWindowRewardItemView : IWBaseMonoBehaviour
{
    [SerializeField] private NSText lblCount;
    [SerializeField] private Image image;
    [SerializeField] private Material disabledMaterial;

    public void Init(CurrencyPair reward, bool enabled)
    {
        lblCount.Text = reward.Amount > 1 && enabled ? "x" + reward.Amount : string.Empty;
        image.sprite = reward.GetIcon();
        image.SetNativeSize();
        image.material = enabled ? null : disabledMaterial;
    }
}