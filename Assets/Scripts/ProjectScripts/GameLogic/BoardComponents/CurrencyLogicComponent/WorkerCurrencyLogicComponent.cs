using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WorkerCurrencyLogicComponent : LimitCurrencyLogicComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private List<KeyValuePair<BoardPosition, TimerComponent>> completeTimesList = new List<KeyValuePair<BoardPosition, TimerComponent>>();
    
    private BoardController context;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
        
        targetItem = ProfileService.Current.Purchases.GetStorageItem(Currency.Worker.Name);
        limitItem = ProfileService.Current.Purchases.GetStorageItem(Currency.WorkerLimit.Name);
        
        base.OnRegisterEntity(entity);
    }

    protected override void InitInSave()
    {
        var save = ProfileService.Current.GetComponent<CurrencySaveComponent>(CurrencySaveComponent.ComponentGuid);
        
        if(save == null) return;

        completeTimesList = new List<KeyValuePair<BoardPosition, TimerComponent>>();
        
        if(string.IsNullOrEmpty(save.WorkerUnlockDelay)) return;
        
        var workers = save.WorkerUnlockDelay.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

        foreach (var worker in workers)
        {
            var timerElement = new KeyValuePair<BoardPosition, TimerComponent>(BoardPosition.Parse(worker), null);
            completeTimesList.Add(timerElement);
        }
    }

    public string Save()
    {
        var str = new StringBuilder();
        
        if(completeTimesList.Count == 0) return string.Empty;

        foreach (var completeTime in completeTimesList)
        {
            str.AppendFormat("{0};", completeTime.Key.ToSaveString());
        }

        return str.ToString();
    }
    
    public void Init(BoardPosition id, TimerComponent timer)
    {
        foreach (var pair in completeTimesList)
        {
            if (pair.Key.Equals(id) == false) continue;
            
            completeTimesList.Remove(pair);
            completeTimesList.Add(new KeyValuePair<BoardPosition, TimerComponent>(id, timer));
            
            return;
        }
    }
    
    public bool Get(BoardPosition id, TimerComponent timer)
    {
        if (CurrencyHellper.IsCanPurchase(targetItem.Currency, 1) == false)
        {
            completeTimesList.Sort((a, b) => a.Value.CompleteTime.CompareTo(b.Value.CompleteTime));
            
            var select = completeTimesList[0];
            
            UIErrorWindowController.AddError("All workers are busy!");
            context.HintCooldown.Step(select.Key);
            
            select.Value.View.SetState(TimerViewSate.Select);

            foreach (var item in completeTimesList)
            {
                if(select.Value == item.Value) continue;
                item.Value.View.SetState(TimerViewSate.Hide);
            }
            
            return false;
        }
        
        var isSuccess = false;
        
        CurrencyHellper.Purchase(Currency.Mine.Name, 1, targetItem.Currency, 1, success =>
        {
            isSuccess = success;

            if (isSuccess == false) return;
            
            completeTimesList.Add(new KeyValuePair<BoardPosition, TimerComponent>(id, timer));
        });
        
        return isSuccess;
    }

    public bool Replace(BoardPosition oldKey, BoardPosition newKey)
    {
        foreach (var pair in completeTimesList)
        {
            if (pair.Key.Equals(oldKey) == false) continue;
            
            completeTimesList.Remove(pair);
            completeTimesList.Add(new KeyValuePair<BoardPosition, TimerComponent>(newKey, pair.Value));
            
            return true;
        }
        
        return false;
    }
    
    public bool Return(BoardPosition id)
    {
        Debug.Log($"[WorkerCurrencyLogicComponent] => Return: {id}");
        
        foreach (var pair in completeTimesList)
        {           
            if (pair.Key.Equals(id) == false) continue;
            
            completeTimesList.Remove(pair);
            Add(1);
            
            // Should be after if (pair.Key.Equals(id) == false) continue; to avoid event raising for every (even not completed) timers
            BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.WorkerUsed, this);
            
            return true;
        }
        
        return false;
    }

    public bool Check(BoardPosition id)
    {
        foreach (var pair in completeTimesList)
        {           
            if (pair.Key.Equals(id) == false) continue;
            
            return true;
        }
        
        return false;
    }

    public bool SetExtra(Piece worker, BoardPosition targetPosition)
    {
        if (worker.PieceType != PieceType.Worker1.Id || context.BoardLogic.IsEmpty(targetPosition)) return false;

        var target = context.BoardLogic.GetPieceAt(targetPosition);
        var def = PieceType.GetDefById(target.PieceType);

        if (def.Filter.Has(PieceTypeFilter.WorkPlace) == false || !CheckLock(target)) return false;
        
        if (!CheckLife(target) && !CheckPieceState(target) && !context.PartPiecesLogic.Work(target)) return false;
        
        context.ActionExecutor.AddAction(new CollapsePieceToAction
        {
            To = targetPosition,
            Positions = new List<BoardPosition> {worker.CachedPosition},
        });
        
        return true;
    }

    private bool CheckLock(Piece target)
    {
        if (!target.Context.Pathfinder.CanPathToCastle(target))
        {
            UIErrorWindowController.AddError("Can't make this action!");
            return false;
        }

        return true;
    }

    private bool CheckLife(Piece target)
    {
        var life = target.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);
        
        if (life == null) return false;

        if (life.Locker.IsLocked)
        {
            UIErrorWindowController.AddError(life.IsFilled ? "Can't make this action!" : "Someone is working here already!");
            return false;
        }

        if (life.IsUseCooldown && life.Timer.IsExecuteable())
        {
            UIErrorWindowController.AddError("It's not ready yet!");
            return false;
        }

        return life.Damage(true);
    }

    private bool CheckPieceState(Piece target)
    {
        var state = target.GetComponent<PieceStateComponent>(PieceStateComponent.ComponentGuid);

        if (state == null) return false;

        if (state.State == BuildingState.InProgress || state.State == BuildingState.Complete)
        {
            UIErrorWindowController.AddError("Someone is working here already!");
            return false;
        }
        
        state.Work(true);
        return true;
    }
}