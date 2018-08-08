using System.Collections.Generic;

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
        var view = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        
        if(view == null) return;
        
        bubble.Add(position, view);
        view.AddView(ViewType.Bubble).Change(true);
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
}