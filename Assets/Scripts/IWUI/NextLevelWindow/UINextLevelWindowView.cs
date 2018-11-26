using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UINextLevelWindowView : UIGenericWindowView
{
    [SerializeField] private NSText title;
    [SerializeField] private NSText message;
    [SerializeField] private NSText rewards;
    [SerializeField] private NSText header;
    
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject headerObj;
    [SerializeField] private CanvasGroup headerCanvas;
    
    [SerializeField] private Transform tapToContinueAnchor;
    
    private int tapAnimationId = Animator.StringToHash("Tap");
    
    private readonly List<UIRecipeCard> cards = new List<UIRecipeCard>();
    
    private TapToContinueTextViewController tapToContinue;
    
    private int tapCount;
    private bool isClick;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UINextLevelWindowModel;
        
        title.Text = windowModel.Title;
        message.Text = windowModel.Mesage;
        rewards.Text = windowModel.Rewards;
        header.Text = windowModel.Header;
        
        headerObj.SetActive(false);
        cardPrefab.SetActive(false);
        
        tapCount = 0;
        headerCanvas.alpha = 0;

        isClick = true;
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
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UINextLevelWindowModel windowModel = Model as UINextLevelWindowModel;
    }

    public override void OnViewCloseCompleted()
    {
        var manager = GameDataService.Current.LevelsManager;
        
        CurrencyHellper.Purchase(Currency.Level.Name, 1, Currency.Experience.Name, manager.Price);
        CurrencyHellper.Purchase(Currency.EnergyLimit.Name, 1);
        CurrencyHellper.Purchase(manager.Rewards, null, new Vector2(Screen.width/2, Screen.height/2));
       
        var currentValue = ProfileService.Current.GetStorageItem(Currency.Energy.Name).Amount;
        var limitValue = ProfileService.Current.GetStorageItem(Currency.EnergyLimit.Name).Amount;
        var diff = limitValue - currentValue;
        
        if (diff > 0) CurrencyHellper.Purchase(Currency.Energy.Name, diff);
            
        GameDataService.Current.QuestsManager.StartNewQuestsIfAny();
        GameDataService.Current.LevelsManager.UpdateSequence();
        
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

        for (var i = 0; i < recipes.Count; i++)
        {
            var recipe = recipes[i];
            var card = UIService.Get.PoolContainer.Create<UIRecipeCard>(cardPrefab);
            
            card.gameObject.SetActive(true);
            card.transform.SetParent(cardPrefab.transform.parent, false);
            card.CachedTransform.SetAsLastSibling();
            card.Init(recipe.Uid, LocalizationService.Get($"order.name.{recipe.Uid}", $"order.name.{recipe.Uid}"));
            card.AddAnimation(0.6f + 0.1f*i);
            cards.Add(card);
        }
        
        DOTween.Sequence()
            .InsertCallback(0.6f, () => headerObj.SetActive(true))
            .InsertCallback(0.62f, () => headerCanvas.alpha = 1)
            .InsertCallback(0.6f + 0.1f*recipes.Count, () => isClick = false);
    }
    
    public void OnClick()
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
                
                CreateCards();
                break;
            case 1:
                Controller.CloseCurrentWindow();
                
                foreach (var card in cards)
                {
                    card.RemoveAnimation(0, () => UIService.Get.PoolContainer.Return(card.gameObject));
                }
        
                cards.Clear();

                headerCanvas.DOFade(0, 0.2f).OnComplete(() => headerObj.SetActive(false));
                break;
            default:
                return;
        }

        tapCount++;
    }
}
