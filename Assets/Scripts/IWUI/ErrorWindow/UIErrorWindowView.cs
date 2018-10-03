using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIErrorWindowView : IWUIWindowView
{
    [SerializeField] private GameObject pattern;

    private float duration = 2f;
    private int index;

    public override float DefaultDelayOnShow => 0f;
    public override float DefaultDelayOnClose => 0f;

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        Next();
    }
    
    public void Next()
    {
        var windowModel = Model as UIErrorWindowModel;

        var str = windowModel.Messages[0];
        var item = Instantiate(pattern, pattern.transform.parent);
        
        index++;
        windowModel.Messages.RemoveAt(0);
        
        item.SetActive(true);
        item.GetComponent<NSText>().Text = str;
        
        var message = item.GetComponent<TextMeshProUGUI>();
        var rect = item.GetComponent<RectTransform>();
        var sequence = DOTween.Sequence();
        
        sequence.Insert(0, message.DOFade(1, duration/3));
        sequence.Insert(0, rect.DOAnchorPosY(50, duration));
        sequence.Insert((duration/3)*2, message.DOFade(0, duration/3));
        sequence.InsertCallback(duration, () =>
        {
            index--;
            Destroy(item);
            
            if (index != 0) return;
            Controller.CloseCurrentWindow();
        });
    }
}
