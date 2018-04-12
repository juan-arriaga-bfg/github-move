using System.Collections.Generic;
using UnityEngine;

public class ReproductionLogicComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	
    public override int Guid
    {
        get { return ComponentGuid; }
    }
    
    private TimerComponent timer = new TimerComponent();
    private readonly HashSet<ReproductionPieceComponent> items = new HashSet<ReproductionPieceComponent>();

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

    private void Reproduction()
    {
        if (Random.Range(0, 101) > PiecesDataManager.ReproductionChance)
        {
            Step();
            return;
        }
        
        var free = new List<ReproductionPieceComponent>();

        foreach (var item in items)
        {
            if(item.CheckFreePosition() == false) continue;
            
            free.Add(item);
        }

        if (free.Count == 0)
        {
            Step();
            return;
        }
        
        free[Random.Range(0, free.Count)].Reproduction();
        Waiting();
    }
    
    public void Add(ReproductionPieceComponent item)
    {
        items.Add(item);
        timer.IsPaused = items.Count == 0;
    }

    public void Remove(ReproductionPieceComponent item)
    {
        items.Remove(item);
        timer.IsPaused = items.Count == 0;
    }
}
