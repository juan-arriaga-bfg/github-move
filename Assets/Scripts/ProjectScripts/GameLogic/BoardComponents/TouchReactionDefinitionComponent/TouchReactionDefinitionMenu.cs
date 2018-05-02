using System;
using System.Collections.Generic;

public class TouchReactionDefinitionMenu : TouchReactionDefinitionComponent
{
    public readonly List<TouchReactionDefinitionComponent> Definitions = new List<TouchReactionDefinitionComponent>();

    public Action OnClick;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public override bool Make(BoardPosition position, Piece piece)
    {
        if (StorageClick(position, piece)) return true;
        
        if (OnClick == null) return false;
        
        OnClick();
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

    private bool StorageClick(BoardPosition position, Piece piece)
    {
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);

        if (storage == null) return false;

        var definition = GetDefinition<TouchReactionDefinitionSpawnInStorage>();
        
        if (definition == null || storage.IsShow == false) return false;

        definition.Make(position, piece);
        
        return true;
    }
}