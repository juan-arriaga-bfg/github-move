using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardsStoreComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public Action OnHideBubble;
    public Func<Dictionary<int, int>> GetRewards;
    
    public Vector2 BubbleOffset = new Vector3(0, 1.5f);
    
    private Piece context;
    
    public string Icon => PieceType.Parse(next);

    private int next;
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
        InitRewards();
    }

    private void InitRewards(Dictionary<int, int> value = null)
    {
        if(rewards == null || rewards.Count == 0) rewards = value ?? GetRewards?.Invoke();
        
        foreach (var key in rewards.Keys)
        {
            next = key;
            break;
        }
    }
    
    public void Show()
    {
        InitRewards();
        UpdateView(true);
    }

    public void Get()
    {
        UpdateView(false);
    }
    
    private void UpdateView(bool isShow)
    {
        if(context.ViewDefinition == null) return;
        
        var view = context.ViewDefinition.AddView(ViewType.StorageState);
        
        if (isShow)
        {
            view.Ofset = BubbleOffset;
            view.SetOfset();
            context.Context.HintCooldown.AddView(view);
        }
        else
        {
            view.OnHide = OnHideBubble;
            context.Context.HintCooldown.RemoweView(view);
        }
        
        view.Change(isShow);
    }
}