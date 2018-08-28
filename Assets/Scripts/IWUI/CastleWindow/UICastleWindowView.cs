using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class UICastleWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private GameObject itemPattern;
    [SerializeField] private NSText upgrade;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private RectTransform content;
    
    private List<GameObject> items = new List<GameObject>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UICastleWindowModel;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        upgrade.Text = windowModel.UpgradeMessage;
        
        var chests = windowModel.Chests;
        
        foreach (var chest in chests)
        {
            var item = Instantiate(itemPattern, itemPattern.transform.parent).GetComponent<UICastleItem>();
            item.Init(chest);
            items.Add(item.gameObject);
        }
        
        itemPattern.SetActive(false);
        
        content.anchoredPosition = new Vector2(-375, 0);
        scroll.enabled = false;
        
        DOTween.Kill(content);
        content.DOAnchorPosX(0, 1.5f)
            .SetEase(Ease.InOutBack)
            .SetId(content)
            .OnComplete(() =>
            {
                scroll.enabled = true;
            });
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UICastleWindowModel;
        
        DOTween.Kill(scroll);
    }
    
    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        itemPattern.SetActive(true);

        foreach (var item in items)
        {
            Destroy(item);
        }
        
        items = new List<GameObject>();
        
        var windowModel = Model as UICastleWindowModel;
        
        windowModel.Spawn();
    }
    
    public void OnClick()
    {
        Controller.CloseCurrentWindow();
    }
}