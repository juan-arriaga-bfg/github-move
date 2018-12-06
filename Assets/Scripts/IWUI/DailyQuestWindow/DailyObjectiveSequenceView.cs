using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyObjectiveSequenceView : IWBaseMonoBehaviour
{
    public const int ITEMS_COUNT = 5;

    [SerializeField] private GameObject rewardItemPrefab;
    [SerializeField] private GameObject lineItemPrefab;

    private List<DailyObjectiveRewardItemView> rewardItems;
    private List<DailyObjectiveLineItemView> lineItems;

    public void Init()
    {
        if (rewardItems != null)
        {
            return;
        }
        
        rewardItemPrefab.SetActive(true);
        lineItemPrefab.SetActive(true);

        rewardItems = new List<DailyObjectiveRewardItemView>();
        lineItems = new List<DailyObjectiveLineItemView>();
        
        for (int i = 0; i < ITEMS_COUNT; i++)
        {
            var goReward = Instantiate(rewardItemPrefab, rewardItemPrefab.transform);
            var goLine = Instantiate(lineItemPrefab, lineItemPrefab.transform);
            
            rewardItems.Add(goReward.GetComponent<DailyObjectiveRewardItemView>());
            lineItems.Add(goLine.GetComponent<DailyObjectiveLineItemView>());
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
            rewardItems[i].Init(reward[i]);

            DailyObjectiveLineItemView.State state;
            if (i == activeIndex)
            {
                state = DailyObjectiveLineItemView.State.Fill;
            }
            else if (i < activeIndex)
            {
                state = DailyObjectiveLineItemView.State.Check;
            }
            else
            {
                state = DailyObjectiveLineItemView.State.Empty;
            }
            
            lineItems[i].Init(state);
        }
    }
}