using System.Collections.Generic;
using BfgAnalytics;
using UnityEngine;

public enum DailyRewardState
{
    Lock,
    Current,
    Claimed
}

public class UIDailyRewardElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#RewardLabel")] private NSText rewardLabel;
    [IWUIBinding("#ClaimButtonLabel")] private NSText btnClaimLabel;
    [IWUIBinding("#LockButtonLabel")] private NSText btnLockLabel;
    
    [IWUIBinding("#ClaimButton")] private UIButtonViewController btnClaim;
    
    [IWUIBinding("#Unlock")] private GameObject unlockObj;
    [IWUIBinding("#Lock")] private GameObject lockObj;
    
    [IWUIBinding("#Check")] private GameObject checkObj;
    [IWUIBinding("#LockBackground")] private GameObject lockBack;
    
    private bool isClick;

    public override void Init()
    {
        base.Init();
	    
        var contentEntity = entity as UIDailyRewardElementEntity;

        isClick = false;
        
        unlockObj.SetActive(contentEntity.State == DailyRewardState.Current);
        lockObj.SetActive(contentEntity.State != DailyRewardState.Current);
        lockBack.SetActive(contentEntity.State == DailyRewardState.Claimed);
        checkObj.SetActive(contentEntity.State == DailyRewardState.Claimed);

        rewardLabel.Text = contentEntity.RewardsText;
        btnClaimLabel.Text = LocalizationService.Get("common.button.claim", "common.button.claim");
        
        btnLockLabel.Text = contentEntity.State == DailyRewardState.Claimed
            ? LocalizationService.Get("window.dailyReward.item.claimed", "window.dailyReward.item.claimed")
            : LocalizationService.Get("window.dailyReward.item.locked", "window.dailyReward.item.locked");
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
		
        btnClaim
            .ToState(GenericButtonState.Active)
            .OnClick(OnClick);
    }

    private void OnClick()
    {
        if (isClick) return;

        isClick = true;
        
        var contentEntity = entity as UIDailyRewardElementEntity;
        var flyPosition = GetComponentInParent<Canvas>().worldCamera.WorldToScreenPoint(btnClaim.transform.position);

        CurrencyHelper.PurchaseAndProvideSpawn(contentEntity.Rewards, null, null, flyPosition, null, false, true);
        Analytics.SendPurchase("screen_dailyreward", $"day{transform.GetSiblingIndex()}", null, new List<CurrencyPair>(contentEntity.Rewards), false, false);
        GameDataService.Current.DailyRewardManager.ClaimCurrentDay();
        
        context.Controller.CloseCurrentWindow();
    }
}
