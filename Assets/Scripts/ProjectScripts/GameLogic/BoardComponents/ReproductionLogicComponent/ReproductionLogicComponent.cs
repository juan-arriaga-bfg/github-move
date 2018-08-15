using System.Collections.Generic;
using UnityEngine;

public class ReproductionLogicComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private readonly TimerComponent timer = new TimerComponent();
    private readonly HashSet<ReproductionPieceComponent> items = new HashSet<ReproductionPieceComponent>();
    private HashSet<ReproductionPieceComponent> stack = new HashSet<ReproductionPieceComponent>();

    public override void OnRegisterEntity(ECSEntity entity)
    {
        RegisterComponent(timer);
        Waiting();
    }

    private void Waiting()
    {
        timer.Delay = PiecesDataManager.ReproductionDelay;
        timer.OnComplete = Reproduction;
        timer.Start();
    }
    
    private void Step()
    {
        timer.Delay = PiecesDataManager.ReproductionStepDelay;
        timer.OnComplete = Reproduction;
        timer.Start();
    }

    public void Restart()
    {
        stack = new HashSet<ReproductionPieceComponent>();

        foreach (var item in items)
        {
            stack.Add(item);
        }
        
        Waiting();
    }

    private void Reproduction()
    {
        if (Random.Range(0, 101) > PiecesDataManager.ReproductionChance)
        {
            Step();
            return;
        }
        
        var free = new List<ReproductionPieceComponent>();

        foreach (var item in stack)
        {
            if(item.Check() == false) continue;
            
            free.Add(item);
        }

        if (free.Count == 0)
        {
            Step();
            return;
        }
        
        var index = Random.Range(0, free.Count);
        free[index].Reproduction();

        stack.Remove(free[index]);
        Waiting();
    }
    
    public void Add(ReproductionPieceComponent item)
    {
        items.Add(item);
        stack.Add(item);
        timer.IsPaused = items.Count == 0;
    }

    public void Remove(ReproductionPieceComponent item)
    {
        items.Remove(item);
        stack.Remove(item);
        timer.IsPaused = items.Count == 0;
    }
}