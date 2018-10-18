using System;
using System.Collections.Generic;

public class TouchReactionDefinitionMenu : TouchReactionDefinitionComponent
{
    public Action<bool> OnMake;
    public int MainReactionIndex = -1;
    public readonly List<TouchReactionDefinitionComponent> Definitions = new List<TouchReactionDefinitionComponent>();
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, piece);

        if (piece.ViewDefinition == null)
        {
            OnMake?.Invoke(false);
            return false;
        }
        
        foreach (var def in Definitions)
        {
            if(def.IsViewShow(piece.ViewDefinition) == false) continue;

            OnMake?.Invoke(true);
            def.Make(position, piece);
            return true;
        }

        if (MainReactionIndex != -1)
        {
            OnMake?.Invoke(true);
            Definitions[MainReactionIndex].Make(position, piece);
            return true;
        }
        
        var view = piece.ViewDefinition.AddView(ViewType.Menu);
        
        view.Change(!view.IsShow);
        OnMake?.Invoke(true);
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