using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UINextLevelWindowView : UIGenericWindowView
{
    [SerializeField] private NSText title;
    [SerializeField] private NSText message;
    [SerializeField] private NSText rewards;
    [SerializeField] private NSText header;
    
    [SerializeField] private GameObject headerObj;
    [SerializeField] private CanvasGroup headerCanvas;
    
    [SerializeField] private Transform tapToContinueAnchor;
    
    private int tapAnimationId = Animator.StringToHash("Tap");
    
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    private TapToContinueTextViewController tapToContinue;
    
    private int tapCount;
    private bool isClick;


    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UINextLevelWindowModel;
        NSAudioService.Current.Play(SoundId.PopupNewLevel);
        title.Text = windowModel.Title;
        message.Text = windowModel.Mesage;
        rewards.Text = windowModel.Rewards;
        header.Text = windowModel.Header;
        
        headerObj.SetActive(false);
        
        tapCount = 0;
        headerCanvas.alpha = 0;

        isClick = true;
        
        Fill(null, content);
    }
    
    public override void AnimateShow()
    {
        base.AnimateShow();
        InitTapToContinue(1f);
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        isClick = false;
        
        if(btnBackLayer != null) btnBackLayer.ToState(GenericButtonState.Active).OnClick(OnClick);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UINextLevelWindowModel windowModel = Model as UINextLevelWindowModel;
    }

    public override void OnViewCloseCompleted()
    {
        var manager = GameDataService.Current.LevelsManager;
        
        CurrencyHelper.Purchase(manager.Rewards, null, new Vector2(Screen.width/2, Screen.height/2));
        CurrencyHelper.Purchase(Currency.Level.Name, 1, Currency.Experience.Name, manager.Price);
        CurrencyHelper.Purchase(Currency.EnergyLimit.Name, 1);
        
        var currentValue = ProfileService.Current.GetStorageItem(Currency.Energy.Name).Amount;
        var limitValue = ProfileService.Current.GetStorageItem(Currency.EnergyLimit.Name).Amount;
        var diff = limitValue - currentValue;
        
        if (diff > 0) CurrencyHelper.Purchase(Currency.Energy.Name, diff);
            
        GameDataService.Current.QuestsManager.StartNewQuestsIfAny();
        GameDataService.Current.LevelsManager.UpdateSequence();
        
        // todo: temp code, remove it
        if (GameDataService.Current.LevelsManager.Level == 6)
        {
            if (GameDataService.Current.QuestsManager.DailyQuest == null)
            {
                GameDataService.Current.QuestsManager.StartNewDailyQuest();
            }
        }
        
        base.OnViewCloseCompleted();
    }
    
    private void InitTapToContinue(float delay)
    {
        if (tapToContinue == null)
        {
            var pool = UIService.Get.PoolContainer;
            tapToContinue = pool.Create<TapToContinueTextViewController>(R.TapToContinueTextView);
            tapToContinue.transform.SetParent(tapToContinueAnchor, false);
            tapToContinue.transform.localPosition = Vector3.zero;
        }
        
        tapToContinue.Hide(false);
        tapToContinue.Show(true, delay);
    }
    
    private void CreateCards()
    {
        var windowModel = Model as UINextLevelWindowModel;
        var recipes = windowModel.Recipes;
        var views = new List<IUIContainerElementEntity>(recipes.Count);
        
        for (var i = 0; i < recipes.Count; i++)
        {
            var def = recipes[i];
            
            var entity = new UINextLevelElementEntity
            {
                ContentId = def.Uid,
                LabelText = LocalizationService.Get($"order.name.{def.Uid}", $"order.name.{def.Uid}"),
                Delay = 0.6f + 0.1f*i,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        Fill(views, content);
        
        DOTween.Sequence()
            .InsertCallback(0.6f, () => headerObj.SetActive(true))
            .InsertCallback(0.62f, () => headerCanvas.alpha = 1)
            .InsertCallback(0.6f + 0.1f*recipes.Count, () => isClick = false);
    }
    
    private void OnClick()
    {
        if(isClick) return;

        isClick = true;
        
        switch (tapCount)
        {
            case 0:
                
                
                for (var i = 0; i < viewAnimators.Count; i++)
                {
                    viewAnimators[i].SetTrigger(tapAnimationId);
                }
                
                var windowModel = Model as UINextLevelWindowModel;
                var recipes = windowModel.Recipes;
                
                if (recipes.Count == 0)
                {
                    Controller.CloseCurrentWindow();
                    return;
                }
                
                NSAudioService.Current.Play(SoundId.PopupNewRecipe);
                CreateCards();
                break;
            case 1:
                Controller.CloseCurrentWindow();
                headerCanvas.DOFade(0, 0.2f).OnComplete(() => headerObj.SetActive(false));
                break;
            default:
                return;
        }

        tapCount++;
    }
}
