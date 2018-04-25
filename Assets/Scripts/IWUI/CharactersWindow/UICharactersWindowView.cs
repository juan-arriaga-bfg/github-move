using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICharactersWindowView : IWUIWindowView
{
    [SerializeField] private NSText wakeUpLabel;
    [SerializeField] private GameObject pattern;
    [SerializeField] private Button wakeUp;
    
    [SerializeField] private RectTransform body;

    private bool isOpen;
    private List<UICharactersItem> items;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        body.DOAnchorPosY(-690, 0f).SetId(body);
    }
    
    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UICharactersWindowModel;
    }

    private void Decoration()
    {
        var windowModel = Model as UICharactersWindowModel;
        wakeUpLabel.Text = windowModel.WakeUpText;

        var heroes = windowModel.Heroes;

        if (heroes.Count == 0)
        {
            pattern.GetComponent<UICharactersItem>().Decoration(null);
            return;
        }

        foreach (var hero in heroes)
        {
            var item = Instantiate(pattern, pattern.transform.parent).GetComponent<UICharactersItem>();
            
            item.Decoration(hero);
        }
        
        pattern.SetActive(false);
    }

    private void Clear()
    {
        if(items == null) return;

        foreach (var item in items)
        {
            Destroy(item.gameObject);
        }
        
        pattern.SetActive(true);
    }

    public void OnClick()
    {
        DOTween.Kill(body, true);
        
        isOpen = !isOpen;
        
        if(isOpen) Decoration();
        
        body.DOAnchorPosY(isOpen ? -465 : -690, 0.3f).SetId(body).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                if (isOpen == false)
                {
                    Clear();
                }
            });
    }
    
    public void OnClickWakeUp()
    {
        if(isOpen == false) return;
    }
    
    public void OnClickNeedMore()
    {
        if(isOpen == false) return;
        
        UIService.Get.ShowWindow(UIWindowType.HeroesWindow);
        OnClick();
    }
}
