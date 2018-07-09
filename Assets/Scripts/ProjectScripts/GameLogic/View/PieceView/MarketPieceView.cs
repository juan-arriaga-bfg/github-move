using System.Collections.Generic;
using UnityEngine;

public class MarketPieceView : PieceBoardElementView
{
    [SerializeField] private List<GameObject> items;
    [SerializeField] private List<GameObject> checks;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

//        Piece.Context.ProductionLogic.OnUpdate += UpdateView;
        UpdateView();
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
//        Piece.Context.ProductionLogic.OnUpdate -= UpdateView;
    }

    public override void UpdateView()
    {
        var tasks = GameDataService.Current.TasksManager.Tasks;

        for (var i = 0; i < items.Count; i++)
        {
            var isActive = i < tasks.Count;
            
            items[i].SetActive(isActive);
            
            if(isActive == false) continue;
            
            checks[i].SetActive(tasks[i].IsComplete);
        }
    }
}