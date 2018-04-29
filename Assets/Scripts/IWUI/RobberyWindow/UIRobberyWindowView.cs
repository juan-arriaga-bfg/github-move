using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRobberyWindowView : UIGenericPopupWindowView
{
    [SerializeField] private NSText btnSendLabel;
    [SerializeField] private NSText btnClaimLabel;
    
    [SerializeField] private Button btnSend;
    [SerializeField] private Button btnClaim;
    
    [SerializeField] private GameObject dot;
    
    [SerializeField] private UIRobberyItem mainChest;
    
    [SerializeField] private Image round;
    [SerializeField] private Image chest;
    
    [SerializeField] private RectTransform line;
    
    private bool isClaimReward;

    private List<UIRobberyItem> items;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIService.Get.CloseWindow(UIWindowType.CharactersWindow);
        UIService.Get.ShowWindow(UIWindowType.CharactersWindow);

        isClaimReward = false;
        
        var windowModel = Model as UIRobberyWindowModel;
        var steps = windowModel.Enemy.Def.Steps;

        windowModel.To = round.GetComponent<RectTransform>().anchoredPosition;
        
        SetTitle(windowModel.Title);
        SetMessage(windowModel.Message);

        btnSendLabel.Text = windowModel.SendText;
        btnClaimLabel.Text = windowModel.ClaimText;

        var max = steps[steps.Count - 1].Amount;
        
        mainChest.Decoration(max, max);

        for (var i = 0; i < steps.Count - 1; i++)
        {
            if(steps[i].Amount <= windowModel.Enemy.Damage) continue;
            
            var item = Instantiate(dot, dot.transform.parent);
            
            item.GetComponent<UIRobberyItem>().Decoration(steps[i].Amount, max);
        }
        
        dot.SetActive(false);

        items = GetComponentsInChildren<UIRobberyItem>().ToList();
        line.sizeDelta = new Vector2(Mathf.Clamp(455 * windowModel.Enemy.Progress, 0, 455), line.sizeDelta.y);
        round.color = new Color(1, 1, 1, 0);
        
        Decoration();
        
        UIService.Get.OnCloseWindowEvent += OnCloseOthers;
    }
    
    public void OnCloseOthers(IWUIWindow window)
    {
        if (!(window.CurrentView is UIHeroesWindowView)) return;
        Decoration();
    }
    
    public void Decoration()
    {
        var windowModel = Model as UIRobberyWindowModel;
        
        DOTween.Kill(line);
        
        var max = windowModel.Enemy.IsComplete ? 530 : 455;
        var value = Mathf.Clamp(max * windowModel.Enemy.Progress, 0, max);
        DOTween.To(() => line.sizeDelta.x, (v) => { line.sizeDelta = new Vector2(v, line.sizeDelta.y); }, value, 0.5f).SetId(line);
        
        var isNone = windowModel.Enemy.ActiveReward == PieceType.None.Id;

        btnSend.interactable = isNone && windowModel.IsAllSleep == false;
        btnClaim.interactable = !isNone;
        
        chest.sprite = IconService.Current.GetSpriteById(string.Format("Chest_{0}active", windowModel.Enemy.IsComplete ? "" : "not_"));
        
        foreach (var item in items)
        {
            item.Check(windowModel.Enemy.Damage);
        }
    }
    
    public override void OnViewCloseCompleted()
    {
        DOTween.Kill(round);
        
        if(isClaimReward == false) return;

        isClaimReward = false;
        
        var windowModel = Model as UIRobberyWindowModel;
        
        windowModel.View.SpawnReward();
        
        windowModel.Enemy = null;

        foreach (var item in items)
        {
            if(item.gameObject == dot || item == mainChest) continue;
            
            Destroy(item.gameObject);
        }
        
        items = new List<UIRobberyItem>();
        
        dot.SetActive(true);
    }

    public void OnClickSend()
    {
        var windowModel = Model as UIRobberyWindowModel;

        windowModel.Attack();
    }

    public void Attack(Hero hero)
    {
        if(hero == null) return;
        
        var windowModel = Model as UIRobberyWindowModel;
        
        windowModel.Enemy.SetDamage(hero.GetAbilityValue(AbilityType.Power));
        windowModel.Enemy.ActivateReward();
        windowModel.View.UpdateFill();
        round.color = windowModel.Enemy.IsComplete ? new Color(1, 1, 1, 0) : new Color(1, 0, 0, 0);;
        
        hero.Sleep();
        
        DOTween.Kill(round);

        DOTween.Sequence().SetId(round)
            .Append(round.DOFade(1, 0.4f))
            .Append(round.DOFade(0, 0.15f))
            .SetLoops(windowModel.Enemy.IsComplete ? int.MaxValue : 2);
        
        Decoration();
        
        var window = UIService.Get.GetTopWindow().CurrentView as UICharactersWindowView;
        
        window.UpdateDecoration();
    }
    
    public void OnClickClaim()
    {
        var windowModel = Model as UIRobberyWindowModel;
        
        if(windowModel.Enemy.ActiveReward == PieceType.None.Id) return;
        
        isClaimReward = true;
        Controller.CloseCurrentWindow();
    }
    
    public void OnClickQuestion()
    {
        
    }
}