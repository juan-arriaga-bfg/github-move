using System.Collections.Generic;

public class TouchReactionDefinitionMenu : TouchReactionDefinitionComponent
{
    public readonly List<TouchReactionDefinitionComponent> Definitions = new List<TouchReactionDefinitionComponent>();
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);
        
        var viewDefinition = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        
        foreach (var def in Definitions)
        {
            if(def.IsViewShow(viewDefinition) == false) continue;

            def.Make(position, piece);
            return true;
        }

        var view = viewDefinition.AddView(ViewType.Menu);
        
        view.Change(!view.IsShow);
        return true;
    }

    public TouchReactionDefinitionMenu RegisterDefinition(TouchReactionDefinitionComponent definition)
    {
        Definitions.Add(definition);
        return this;
    }
    
    public T GetDefinition<T>() where T : TouchReactionDefinitionComponent
    {
        foreach (var definition in Definitions)
        {
            if (definition is T) return definition as T;
        }
        
        return null;
    }
}