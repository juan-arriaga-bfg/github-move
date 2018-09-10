using System.Collections.Generic;
using UnityEngine;

public class PartPiecesLogicComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private BoardController context;
    
    private readonly Dictionary<BoardPosition, ViewDefinitionComponent> bubble = new Dictionary<BoardPosition, ViewDefinitionComponent>();
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Add(List<BoardPosition> positions)
    {
        var index = positions.Count / 2;
        var position = positions[index];
        
        if(bubble.ContainsKey(position)) return;
        
        var piece = context.BoardLogic.GetPieceAt(position);
        var viewDef = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        
        if(viewDef == null) return;
        
        bubble.Add(position, viewDef);
        
        var view = viewDef.AddView(ViewType.Bubble) as BubbleView;
        
        view.SetData("Build Castle?", $"Send <sprite name={Currency.Worker.Name}>", OnClick);
        view.Change(true);
    }

    public void Remove(List<BoardPosition> positions)
    {
        var index = positions.Count / 2;
        var position = positions[index];
        
        ViewDefinitionComponent view;
        
        if(bubble.TryGetValue(position, out view) == false) return;
        
        bubble.Remove(position);
        view.AddView(ViewType.Bubble).Change(false);
    }

    private void OnClick(Piece piece)
    {
        var key = piece.CachedPosition.ToSaveString();
        var def = GameDataService.Current.PiecesManager.GetPieceDef(piece.PieceType + 2);
        
        if(context.WorkerLogic.Get(key, def.MatchConditionsDef.Delay) == false) return;
        
        var action = piece.Context.BoardLogic.MatchActionBuilder.GetMatchAction(new List<BoardPosition>(), piece.PieceType, piece.CachedPosition);
        
        if(action == null) return;
        
        piece.Context.ActionExecutor.AddAction(action);
    }
}