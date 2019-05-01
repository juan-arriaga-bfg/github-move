﻿using System.Collections.Generic;
using System.Linq;
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
    
    public override void Init()
    {
        base.Init();
        
        var contentEntity = entity as UIEventElementEntity;

        var isNormalActive = contentEntity.GameStep.NormalRewards != null && contentEntity.GameStep.NormalRewards.Count > 0;
        var isPremiumActive = contentEntity.GameStep.PremiumRewards != null && contentEntity.GameStep.PremiumRewards.Count > 0;
        var isPremiumPaid = GameDataService.Current.EventGameManager.IsPremium(EventGameType.OrderSoftLaunch);
        
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
        
        if (isPremiumPaid == false) Sepia(premiumSprites, true);
        
        IsComplete = Index < GameDataService.Current.EventGameManager.Step;
        
        check.SetActive(IsComplete);
        normalShine.SetActive(IsComplete && contentEntity.GameStep.IsNormalClaimed == false);
        premiumShine.SetActive(isPremiumPaid && IsComplete && contentEntity.GameStep.IsPremiumClaimed == false);
        
        btnNormal.gameObject.SetActive(normalShine.activeSelf);
        btnPremium.gameObject.SetActive(premiumShine.activeSelf);
        
        CheckClaim();
    }

    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        btnNormal.ToState(GenericButtonState.Active).OnClick(OnNormalClick);
        btnPremium.ToState(GenericButtonState.Active).OnClick(OnPremiumClick);
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
        
        contentEntity.GameStep.IsNormalClaimed = true;
        Claim(contentEntity.GameStep.NormalRewards, btnNormal.transform.position);
    }
    
    private void OnPremiumClick()
    {
        var contentEntity = entity as UIEventElementEntity;
        
        contentEntity.GameStep.IsPremiumClaimed = true;
        Claim(contentEntity.GameStep.PremiumRewards, btnPremium.transform.position);
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