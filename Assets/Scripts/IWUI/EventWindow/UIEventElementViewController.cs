using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;
using UnityEngine;
using UnityEngine.UI;

public class UIEventElementViewController : UIContainerElementViewController
{
    [IWUIBinding("#Check")] private GameObject check;
    
    [IWUIBinding("#Normal")] private GameObject normal;
    [IWUIBinding("#Premium")] private GameObject premium;
    
    [IWUIBinding("#NormalShine")] private GameObject normalShine;
    [IWUIBinding("#PremiumShine")] private GameObject premiumShine;
    
    [IWUIBinding("#NormalCheck")] private GameObject normalCheck;
    [IWUIBinding("#PremiumCheck")] private GameObject premiumCheck;
    
    [IWUIBinding("#NormalAnchor")] private Transform normalAnchor;
    [IWUIBinding("#PremiumAnchor")] private Transform premiumAnchor;
    
    [IWUIBinding("#NormalLabel")] private NSText normalLabel;
    [IWUIBinding("#PremiumLabel")] private NSText premiumLabel;
    
    [IWUIBinding("#NormalButton")] private UIButtonViewController btnNormal;
    [IWUIBinding("#PremiumButton")] private UIButtonViewController btnPremium;

    private Transform normalIcon;
    private Transform premiumIcon;
    
    private List<Image> normalSprites;
    private List<Image> premiumSprites;
    
    private Material lockMaterial;
    private Material unlockMaterial;

    public bool IsComplete;

    private EventGame eventGame;
    
    public override void Init()
    {
        base.Init();
        
        BoardService.Current.FirstBoard.BoardLogic.EventGamesLogic.GetEventGame(EventGameType.OrderSoftLaunch, out eventGame);
        
        var contentEntity = entity as UIEventElementEntity;

        var isNormalActive = contentEntity.GameStep.IsNormalIgnored == false;
        var isPremiumActive = contentEntity.GameStep.IsPremiumIgnored == false;
        
        normal.SetActive(isNormalActive);
        premium.SetActive(isPremiumActive);

        if (isNormalActive)
        {
            normalLabel.Text = $"x{contentEntity.GameStep.NormalRewards[0].Amount}";
            CreateIcon(ref normalIcon, normalAnchor, ref normalSprites, contentEntity.GameStep.NormalRewards[0].Currency);
        }
        
        if (isPremiumActive)
        {
            premiumLabel.Text = $"x{contentEntity.GameStep.PremiumRewards[0].Amount}";
            CreateIcon(ref premiumIcon, premiumAnchor, ref premiumSprites, contentEntity.GameStep.PremiumRewards[0].Currency);
        }
        
        if (eventGame.IsPremium == false) Sepia(premiumSprites, true);
        
        IsComplete = Index < eventGame.Step;
        
        check.SetActive(IsComplete);
        normalShine.SetActive(isNormalActive && IsComplete && contentEntity.GameStep.IsNormalClaimed == false);
        premiumShine.SetActive(isPremiumActive && eventGame.IsPremium && IsComplete && contentEntity.GameStep.IsPremiumClaimed == false);
        
        btnNormal.gameObject.SetActive(normalShine.activeSelf);
        
        CheckClaim();
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        btnNormal.ToState(GenericButtonState.Active).OnClick(OnNormalClick);
        btnPremium.ToState(GenericButtonState.Active).OnClick(OnPremiumClick);
    }

    public override void OnViewClose(IWUIWindowView context)
    {
        if (entity is UIEventElementEntity && (eventGame.State == EventGameState.Complete || eventGame.State == EventGameState.Claimed))
        {
            OnNormalClick();
            ClaimPremiumReward();
        }
        
        base.OnViewClose(context);
    }

    public override void OnViewCloseCompleted()
    {
        Return(ref normalIcon, ref normalSprites);
        Return(ref premiumIcon, ref premiumSprites);
        
        base.OnViewCloseCompleted();
    }

    private void OnNormalClick()
    {
        var contentEntity = entity as UIEventElementEntity;
        
        if (btnNormal.gameObject.activeSelf == false || contentEntity.GameStep.IsNormalClaimed) return;
        
        contentEntity.GameStep.IsNormalClaimed = true;
        Claim(contentEntity.GameStep.NormalRewards, btnNormal.transform.position);
        Analytics.SendPurchase("screen_event", $"stage{Index + 1}", null, new List<CurrencyPair>(contentEntity.GameStep.NormalRewards), false, false);
    }
    
    private void OnPremiumClick()
    {
        var contentEntity = entity as UIEventElementEntity;

        if (contentEntity.GameStep.IsPremiumIgnored == false)
        {
            UIService.Get.ShowWindow(UIWindowType.EventSubscriptionWindow);
            return;
        }
        
        ClaimPremiumReward();
    }

    private void ClaimPremiumReward()
    {
        var contentEntity = entity as UIEventElementEntity;

        if (premiumShine.activeSelf == false || contentEntity.GameStep.IsPremiumClaimed) return;
        
        contentEntity.GameStep.IsPremiumClaimed = true;
        Claim(contentEntity.GameStep.PremiumRewards, btnPremium.transform.position);
        Analytics.SendPurchase("screen_event", $"stage{Index + 1}_premium", null, new List<CurrencyPair>(contentEntity.GameStep.PremiumRewards), false, false);
    }

    private void Claim(List<CurrencyPair> reward, Vector3 position)
    {
        var flyPosition = GetComponentInParent<Canvas>().worldCamera.WorldToScreenPoint(position);
        CurrencyHelper.PurchaseAndProvideSpawn(reward, null, null, flyPosition, null, false, true);
        
        CheckClaim();
        context.Controller.CloseCurrentWindow();
    }
    
    private void CheckClaim()
    {
        var contentEntity = entity as UIEventElementEntity;
        var isNormalClaimed = contentEntity.GameStep.IsNormalClaimed;
        var isPremiumClaimed = contentEntity.GameStep.IsPremiumClaimed;
        
        normalCheck.SetActive(isNormalClaimed);
        premiumCheck.SetActive(isPremiumClaimed);
        
        if (isNormalClaimed) Sepia(normalSprites, true);
        if (isPremiumClaimed) Sepia(premiumSprites, true);
        
        var views = ResourcesViewManager.Instance.GetViewsById(Currency.Token.Name);

        if (views == null) return;
        
        foreach (UITokensPanelViewController carrierView in views)
        {
            if (carrierView != null) carrierView.UpdateMark();
        }
    }
    
    private void CreateIcon(ref Transform current, Transform parent, ref List<Image> sprites, string id)
    {
        if (current != null) Return(ref current, ref sprites);
        if (string.IsNullOrEmpty(id)) return;
        
        current = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        current.SetParentAndReset(parent);
        
        sprites = current.GetComponentsInChildren<Image>().ToList();
    }
    
    private void Sepia(List<Image> sprites, bool value)
    {
        if (sprites == null) return;
            
        if (lockMaterial == null) lockMaterial = (Material) ContentService.Current.GetObjectByName("UiSepia");
        if (unlockMaterial == null) unlockMaterial = sprites.Count > 0 ? sprites[0].material : lockMaterial;

        foreach (var sprite in sprites)
        {
            sprite.material = value ? lockMaterial : unlockMaterial;
            sprite.color = new Color(1, 1, 1, value ? 0.5f : 1); 
        }
    }

    private void Return(ref Transform current, ref List<Image> sprites)
    {
        Sepia(sprites, false);
        
        if(current != null) UIService.Get.PoolContainer.Return(current.gameObject);
        
        current = null;
        sprites = null;
    }
}