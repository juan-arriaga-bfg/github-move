using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIErrorWindowView : IWUIWindowView
{
    [SerializeField] private GameObject pattern;

    private bool isAnimated;
    private float duration = 2f;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIErrorWindowModel;
        
        if(isAnimated) return;
        
        isAnimated = true;
        
        Next();
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UIErrorWindowModel;
        
    }

    private void Next()
    {
        var windowModel = Model as UIErrorWindowModel;

        if (windowModel.Messages.Count == 0)
        {
            isAnimated = false;
            Controller.CloseCurrentWindow();
            return;
        }

        var str = "";
        var item = Instantiate(pattern, pattern.transform.parent);

        foreach (var key in windowModel.Messages.Keys)
        {
            str = key;
            break;
        }
        
        windowModel.Messages.Remove(str);
        
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
            Destroy(item);
            Next();
        });
    }
}
