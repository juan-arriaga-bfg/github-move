using System;
using System.Collections.Generic;

public class TouchReactionDefinitionMenu : TouchReactionDefinitionComponent
{
    public readonly Dictionary<string, TouchReactionDefinitionComponent> Definitions = new Dictionary<string, TouchReactionDefinitionComponent>();

    public Action OnClick;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public override bool Make(BoardPosition position, Piece piece)
    {
        if (OnClick == null) return false;
        
        OnClick();
        return true;
    }

    public TouchReactionDefinitionMenu RegisterDefinition(TouchReactionDefinitionComponent definition, string image)
    {
        if (Definitions.ContainsKey(image)) return this;
        
        Definitions.Add(image, definition);
        return this;
    }
    
    public BoardButtonView.Color GetColor(TouchReactionDefinitionComponent definition)
    {
        if (definition is TouchReactionDefinitionUpgrade) return BoardButtonView.Color.Blue;
        
        if (definition is TouchReactionDefinitionOpenHeroesWindow) return BoardButtonView.Color.Orange;
        
        return BoardButtonView.Color.Green;
    }
}
