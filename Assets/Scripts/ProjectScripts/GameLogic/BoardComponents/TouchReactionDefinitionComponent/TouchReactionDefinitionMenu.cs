using System.Collections.Generic;

public class TouchReactionDefinitionMenu : TouchReactionDefinitionComponent
{
    public int MainReactionIndex = -1;
    public readonly List<TouchReactionDefinitionComponent> Definitions = new List<TouchReactionDefinitionComponent>();
    
    private ViewDefinitionComponent viewDef;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, piece);
        
        if (viewDef == null)
        {
            viewDef = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
            
            if (viewDef == null) return false;
        }
        
        foreach (var def in Definitions)
        {
            if(def.IsViewShow(viewDef) == false) continue;

            def.Make(position, piece);
            return true;
        }

        if (MainReactionIndex != -1)
        {
            Definitions[MainReactionIndex].Make(position, piece);
            return true;
        }
        
        var view = viewDef.AddView(ViewType.Menu);
        
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