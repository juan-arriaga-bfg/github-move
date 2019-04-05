using System.Collections.Generic;
using System.Linq;
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

    private Dictionary<int, int> piecesReward;
    private List<CurrencyPair> currenciesReward;
    
    public override void Init()
    {
        base.Init();
	    
        var contentEntity = entity as UIDailyRewardElementEntity;

        isClick = false;
        
        piecesReward = CurrencyHelper.FiltrationRewards(contentEntity.Rewards, out currenciesReward);

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

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();

        if (!(entity is UIDailyRewardElementEntity contentEntity) || contentEntity.State != DailyRewardState.Current) return;
        
        var board = BoardService.Current.FirstBoard;
        var amount = piecesReward.Sum(pair => pair.Value);

        board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceReward(amount, true, out var position);
        
        CurrencyHelper.PurchaseAndProvideSpawn(piecesReward, new List<CurrencyPair>(), null, position, null, true, true);
    }

    private void OnClick()
    {
        if (isClick) return;

        isClick = true;
        
        var flyPosition = GetComponentInParent<Canvas>().worldCamera.WorldToScreenPoint(btnClaim.transform.position);
	    
        CurrencyHelper.PurchaseAsyncOnlyCurrency(currenciesReward, flyPosition, success =>
        {
            if (success == false) return;
            ProfileService.Instance.Manager.UploadCurrentProfile(false);
        });
        
        Analytics.SendPurchase("screen_dailyreward", $"item{transform.GetSiblingIndex()}", null, new List<CurrencyPair>(currenciesReward), false, true);
        context.Controller.CloseCurrentWindow();
    }
}
