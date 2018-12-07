using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyQuestWindowSequenceView : IWBaseMonoBehaviour
{
    public const int ITEMS_COUNT = 5;

    [SerializeField] private GameObject rewardItemPrefab;
    [SerializeField] private GameObject lineItemPrefab;

    private List<DailyQuestWindowRewardItemView> rewardItems;
    private List<DailyQuestWindowLineItemView> lineItems;

    public void Init()
    {
        if (rewardItems != null)
        {
            return;
        }
        
        rewardItemPrefab.SetActive(true);
        lineItemPrefab.SetActive(true);

        rewardItems = new List<DailyQuestWindowRewardItemView>();
        lineItems = new List<DailyQuestWindowLineItemView>();
        
        for (int i = 0; i < ITEMS_COUNT; i++)
        {
            var goReward = Instantiate(rewardItemPrefab);
            var goLine = Instantiate(lineItemPrefab);
            
            goReward.transform.SetParentAndReset(rewardItemPrefab.transform.parent);
            goLine.transform.SetParentAndReset(lineItemPrefab.transform.parent);
            
            rewardItems.Add(goReward.GetComponent<DailyQuestWindowRewardItemView>());
            lineItems.Add(goLine.GetComponent<DailyQuestWindowLineItemView>());
        }
        
        rewardItemPrefab.SetActive(false);
        lineItemPrefab.SetActive(false); 
    }
    
    public void SetValues(List<CurrencyPair> reward, int activeIndex)
    {
        if (reward.Count != ITEMS_COUNT)
        {
            throw new Exception($"DailyObjectiveSequenceView: reward.Count {reward.Count} != ITEMS_COUNT {ITEMS_COUNT}!");
        }
        
        for (int i = 0; i < ITEMS_COUNT; i++)
        {
            DailyQuestWindowLineItemView.State state;
            if (i == activeIndex)
            {
                state = DailyQuestWindowLineItemView.State.Fill;

                if (GameDataService.Current.QuestsManager.DailyQuest.IsClaimed())
                {
                    state = DailyQuestWindowLineItemView.State.Check;
                }
            }
            else if (i < activeIndex)
            {
                state = DailyQuestWindowLineItemView.State.Check;
            }
            else
            {
                state = DailyQuestWindowLineItemView.State.Empty;
            }
            
            rewardItems[i].Init(reward[i], state == DailyQuestWindowLineItemView.State.Empty || state == DailyQuestWindowLineItemView.State.Fill);
            
            lineItems[i].Init(state);
        }
    }
}