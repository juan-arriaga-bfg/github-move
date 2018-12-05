using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardsStoreComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public Func<Dictionary<int, int>> GetRewards;
    public Action OnComplete;
    
    public Vector2 BubbleOffset = new Vector3(0, 1.5f);
    
    public string Icon => PieceType.Parse(next);
    
    public bool IsTargetReplace;
    public bool IsComplete;
    
    private Piece context;
    
    private int next;
    private int defaulAmaunt;
    private Dictionary<int, int> rewards;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void InitInSave(BoardPosition position)
    {
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        var item = save?.GetRewardsSave(position);

        if (item == null) return;
        
        rewards = item.Reward;
        
        if (item.IsComplete) ShowBubble();
    }

    public RewardsSaveItem Save()
    {
        return rewards == null
            ? null
            : new RewardsSaveItem
            {
                Position = context.CachedPosition,
                Reward = rewards,
                RewardAmount = defaulAmaunt,
                IsComplete = IsComplete
            };
    }

    private void InitRewards(Dictionary<int, int> value = null)
    {
        if (rewards == null || rewards.Count == 0)
        {
            rewards = value;

            if (rewards == null)
            {
                rewards = GetRewards?.Invoke();
                defaulAmaunt = rewards.Sum(pair => pair.Value);
            } 
        }
        
        foreach (var key in rewards.Keys)
        {
            next = key;
            break;
        }
    }
    
    public void ShowBubble()
    {
        InitRewards();
        UpdateView(true);
    }
    
    public void GetInBubble()
    {
        UpdateView(false);
    }

    public void GetInWindow()
    {
        InitRewards();
        Scatter();
        IsComplete = true;
    }
    
    private void UpdateView(bool isShow)
    {
        if(context.ViewDefinition == null) return;
        
        var view = context.ViewDefinition.AddView(ViewType.RewardsBubble) as RewardsBubbleView;

        if (view == null || view.IsShow == isShow) return;
        
        if (isShow)
        {
            view.Ofset = BubbleOffset;
            view.SetOfset();
            view.OnClickAction = GetInBubble;
            context.Context.HintCooldown.AddView(view);
        }
        else
        {
            view.OnHide = Scatter;
            context.Context.HintCooldown.RemoweView(view);
        }
        
        IsComplete = isShow;
        view.Change(isShow);
    }

    private void Scatter()
    {
        context.Context.ActionExecutor.AddAction(new ScatterPiecesAction
        {
            IsTargetReplace = IsTargetReplace,
            From = context.CachedPosition,
            Pieces = rewards,
            OnComplete = OnComplete
        });
    }
}