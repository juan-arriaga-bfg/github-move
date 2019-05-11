using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardsStoreComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public Func<Dictionary<int, int>> GetRewards;
    public Action<bool> OnComplete;
    
    public string Icon => PieceType.Parse(next);
    public bool IsHighlight => rewards != null && rewards.Sum(pair => pair.Value) < defaultAmount;
    
    public bool IsTargetReplace;
    public bool IsComplete;
    public bool IsScatter;
    public bool IsSingle;
    
    private Piece context;
    
    private int next;
    private int defaultAmount;
    private Dictionary<int, int> rewards;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as Piece;
        GameDataService.Current.CharactersManager.OnUpdateSequence += UpdateSequence;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        GameDataService.Current.CharactersManager.OnUpdateSequence -= UpdateSequence;
    }

    public void InitInSave(BoardPosition position)
    {
        var save = ProfileService.Current.GetComponent<FieldDefComponent>(FieldDefComponent.ComponentGuid);
        var item = save?.GetRewardsSave(position);

        if (item == null) return;
        
        rewards = item.Reward;
        defaultAmount = item.RewardAmount;
        
        if (item.IsComplete) ShowBubble();
    }

    private void UpdateSequence()
    {
        if (rewards == null || rewards.Count == 0) return;
        
        var definition = context.Context.BoardLogic.MatchDefinition;
        var replace = new List<int>();

        foreach (var key in rewards.Keys)
        {
            var last = definition.GetLast(key);
            var def = PieceType.GetDefById(last);

            if (def.Filter.Has(PieceTypeFilter.Character) == false
                || GameDataService.Current.CharactersManager.Characters.Contains(last)) continue;
            
            replace.Add(key);
        }

        foreach (var key in replace)
        {
            if (rewards.ContainsKey(CharactersDataManager.ReplacePiece.Id) == false)
            {
                rewards.Add(CharactersDataManager.ReplacePiece.Id, 0);
            }

            rewards[CharactersDataManager.ReplacePiece.Id] += rewards[key];
            rewards.Remove(key);
        }

        InitRewards();
        
        var view = UpdateView(IsComplete);

        if (view != null) view.UpdateIcon();
    }
    
    public RewardsSaveItem Save()
    {
        return rewards == null
            ? null
            : new RewardsSaveItem
            {
                Position = context.CachedPosition,
                Reward = rewards,
                RewardAmount = defaultAmount,
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
                defaultAmount = rewards.Sum(pair => pair.Value);
            } 
        }
        
        foreach (var key in rewards.Keys)
        {
            var def = PieceType.GetDefById(key);

            if (def.Filter.Has(PieceTypeFilter.Obstacle)) continue;
            
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
        if(CheckOutOfCells()) return;
        
        UpdateView(false);
    }

    public void GetInWindow()
    {
        InitRewards();
        
        if(CheckOutOfCells()) return;
        
        if(PieceType.GetDefById(context.PieceType).Filter.HasFlag(PieceTypeFilter.Chest)) NSAudioService.Current.Play(SoundId.ChestOpen);
        
        Scatter();
    }
    
    public void FullDrop()
    {
        InitRewards();
        
        if(CheckOutOfCellsDropFullReward()) return;
        
        if(PieceType.GetDefById(context.PieceType).Filter.HasFlag(PieceTypeFilter.Chest)) NSAudioService.Current.Play(SoundId.ChestOpen);
        
        Scatter(false);
    }
    
    public bool CheckOutOfCellsDropFullReward()
    {
        InitRewards();
        
        if (IsScatter) return true;
        
        var current = rewards.Sum(pair => pair.Value);
        
        if (IsTargetReplace) current = Mathf.Max(0, current - (context.Multicellular?.Mask.Count ?? 1));
        if (IsSingle && current > 0) current = 1;
        
        var cells = new List<BoardPosition>();

        if (current == 0 ||
            context.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(context.CachedPosition, cells,
                current, 0.1f))
        {
            if (cells.Count >= current)
            {
                return false;
            }
        }
        
        UIErrorWindowController.AddNoFreeSpaceError();
        return true;
    }
    
    public bool CheckOutOfCells()
    {
        if (IsScatter) return true;
        
        var current = rewards.Sum(pair => pair.Value);
        
        if (IsTargetReplace) current = Mathf.Max(0, current - (context.Multicellular?.Mask.Count ?? 1));
        if (IsSingle && current > 0) current = 1;
        
        var cells = new List<BoardPosition>();
        
        if (current == 0 || context.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(context.CachedPosition, cells, current, 0.1f)) return false;
        
        UIErrorWindowController.AddNoFreeSpaceError();
        return true;
    }
    
    private RewardsBubbleView UpdateView(bool isShow)
    {
        if (context.ViewDefinition == null) return null;
        
        var view = context.ViewDefinition.AddView(ViewType.RewardsBubble) as RewardsBubbleView;

        if (view == null || view.IsShow == isShow) return view;
        
        if (isShow)
        {
            view.OnClickAction = GetInBubble;
            context.Context.HintCooldown.AddView(view);
        }
        else
        {
            IsScatter = true;
            Scatter();
            context.Context.HintCooldown.RemoweView(view);
        }
        
        IsComplete = isShow;
        view.Change(isShow);
        return view;
    }

    private void Scatter(bool isSetRewardToComplete = true)
    {
        var filter = PieceType.GetDefById(context.PieceType).Filter;
        
        context.Context.ActionExecutor.AddAction(new ScatterPiecesAction
        {
            IsSingle = IsSingle,
            IsTargetReplace = IsTargetReplace,
            From = context.CachedPosition,
            Pieces = rewards,
            IsSetRewardToComplete = isSetRewardToComplete,
            OnComplete = value =>
            {
                IsScatter = false;
                OnComplete(value);
                context.Context.TutorialLogic.Update();
            },
            RewardEffect = filter.Has(PieceTypeFilter.Chest) || filter.Has(PieceTypeFilter.Obstacle)
        });
    }
}