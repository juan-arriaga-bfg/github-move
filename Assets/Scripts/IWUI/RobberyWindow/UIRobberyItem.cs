using DG.Tweening;
using UnityEngine;

public class UIRobberyItem : MonoBehaviour
{
    [SerializeField] private NSText targetLabel;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform icon;
    [SerializeField] private GameObject labelGo;

    private int target;

    public void Decoration(int value, int max)
    {
        target = value;
        targetLabel.Text = value.ToString();
        rectTransform.anchoredPosition = new Vector2(455 * (value / (float) max), rectTransform.anchoredPosition.y);
        labelGo.SetActive(true);
    }

    public void Check(int value)
    {
        if(target > value) return;

        var y = icon.anchoredPosition.y;

        DOTween.Sequence().SetId(icon).SetEase(Ease.InOutSine).SetLoops(int.MaxValue)
            .Insert(0f, icon.DOScale(new Vector3(1.2f, 0.7f), 0.1f))
            .Insert(0.1f, icon.DOScale(new Vector3(0.7f, 1.2f), 0.1f))
            .Insert(0.15f, icon.DOAnchorPosY(y + 15, 0.3f))
            .Insert(0.2f, icon.DOScale(Vector3.one, 0.1f))
            .Insert(0.45f, icon.DOAnchorPosY(y, 0.2f))
            .Insert(0.65f, icon.DOScale(new Vector3(1.1f, 0.9f), 0.1f))
            .Insert(0.75f, icon.DOScale(Vector3.one, 0.1f))
            .AppendInterval(1f);
        
        labelGo.SetActive(false);
    }

    private void OnDestroy()
    {
        DOTween.Kill(icon);
    }
}