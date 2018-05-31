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
    [SerializeField] private UICharactersFlyingItem flyItem;

    private const int open = 80;
    private const int close = -150;

    private bool isOpen;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        body.DOAnchorPosY(close, 0f).SetId(body);

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
                body.DOAnchorPosY(open, 0f).SetId(body);
                UpdateDecoration();
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
        var count = Mathf.Max(3, heroes.Count);
        
        for (var i = 0; i < count; i++)
        {
            var hero = i >= heroes.Count ? null : heroes[i];
            var item = Instantiate(pattern, pattern.transform.parent).GetComponent<UICharactersItem>();

            item.Decoration(hero);
            windowModel.Items.Add(item);
            
            if (hero != null && hero.IsSleep) isSleep = true;
        }
        
        pattern.SetActive(false);
        wakeUp.interactable = isSleep;
    }

    private void Clear()
    {
        var windowModel = Model as UICharactersWindowModel;
        
        if(windowModel.Items == null) return;

        foreach (var item in windowModel.Items)
        {
            Destroy(item.gameObject);
        }
        
        windowModel.Items = new List<UICharactersItem>();
        
        pattern.SetActive(true);
    }

    public void OnClick()
    {
        if(isOpen && UIService.Get.GetShowedWindowByName(UIWindowType.RobberyWindow) != null) return;
        
        DOTween.Kill(body, true);
        
        isOpen = !isOpen;
        
        if(isOpen) Decoration();
        
        body.DOAnchorPosY(isOpen ? open : close, 0.3f).SetId(body).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                if (isOpen == false)
                {
                    Clear();
                }
            });
    }

    public void Fly(Hero hero)
    {
        flyItem.Fly(hero);
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
