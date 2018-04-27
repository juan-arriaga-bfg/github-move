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
    private List<UICharactersItem> items = new List<UICharactersItem>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        body.DOAnchorPosY(-690, 0f).SetId(body);

        UIService.Get.OnShowWindowEvent += OnShowOthers;
        UIService.Get.OnCloseWindowEvent += OnCloseOthers;
    }
    
    public override void OnViewClose()
    {
        base.OnViewClose();
        
        var windowModel = Model as UICharactersWindowModel;
        UIService.Get.OnShowWindowEvent -= OnShowOthers;
    }

    public void OnShowOthers(IWUIWindow window)
    {
        if (window.CurrentView is UIRobberyWindowView)
        {
            if (isOpen)
            {
                body.DOAnchorPosY(-465, 0f).SetId(body);
            }
            else
            {
                OnClick();
            }
            return;
        }
        
        if(isOpen) OnClick();
    }

    public void OnCloseOthers(IWUIWindow window)
    {
        if (window.CurrentView is UIHeroesWindowView)
        {
            UpdateDecoration();
        }
        
        if (!(window.CurrentView is UIRobberyWindowView)) return;
        if(isOpen) OnClick();
    }

    public void UpdateDecoration()
    {
        Clear();
        Decoration();
    }
    
    private void Decoration()
    {
        var windowModel = Model as UICharactersWindowModel;
        wakeUpLabel.Text = windowModel.WakeUpText;

        var heroes = windowModel.Heroes;
        var isSleep = false;
        
        if (heroes.Count == 0)
        {
            pattern.GetComponent<UICharactersItem>().Decoration(null);
            wakeUp.interactable = false;
            return;
        }

        foreach (var hero in heroes)
        {
            var item = Instantiate(pattern, pattern.transform.parent).GetComponent<UICharactersItem>();
            
            item.Decoration(hero);
            items.Add(item);

            if (hero.IsSleep) isSleep = true;
        }
        
        pattern.SetActive(false);
        wakeUp.interactable = isSleep;
    }

    private void Clear()
    {
        if(items == null) return;

        foreach (var item in items)
        {
            Destroy(item.gameObject);
        }
        
        items = new List<UICharactersItem>();
        
        pattern.SetActive(true);
    }

    public void OnClick()
    {
        if(isOpen && UIService.Get.GetShowedWindowByName(UIWindowType.RobberyWindow) != null) return;
        
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
        
        var windowModel = Model as UICharactersWindowModel;

        CurrencyHellper.Purchase("WakeUp", 1, windowModel.WakeUpPrice, succees =>
        {
            if(succees == false) return;
            
            var heroes = windowModel.Heroes.FindAll(hero => hero.IsSleep);

            foreach (var hero in heroes)
            {
                hero.WakeUp();
            }

            var window = UIService.Get.GetShowedWindowByName(UIWindowType.RobberyWindow);

            UpdateDecoration();
            if (window != null) (window.CurrentView as UIRobberyWindowView).Decoration();
        });
    }
    
    public void OnClickNeedMore()
    {
        if(isOpen == false) return;
        
        UIService.Get.ShowWindow(UIWindowType.HeroesWindow);
    }
}
