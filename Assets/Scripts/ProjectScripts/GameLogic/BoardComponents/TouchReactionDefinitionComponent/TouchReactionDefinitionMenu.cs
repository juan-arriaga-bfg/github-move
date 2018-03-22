using System.Collections.Generic;
using UnityEngine;

public class TouchReactionDefinitionMenu : TouchReactionDefinitionComponent, IBoardEventListener
{
    public readonly Dictionary<string, TouchReactionDefinitionComponent> Definitions = new Dictionary<string, TouchReactionDefinitionComponent>();

    private bool isOpen;
    
    private BoardEventsComponent eventBus;
    private Piece cachedPiece;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        eventBus.RemoveListener(this, GameEventsCodes.OpenPieceMenu);
        eventBus.RemoveListener(this, GameEventsCodes.ClosePieceMenu);
        eventBus = null;
        cachedPiece = null;
    }

    public override bool Make(BoardPosition position, Piece piece)
    {
        if (eventBus == null)
        {
            eventBus = piece.Context.BoardEvents;
            eventBus.AddListener(this, GameEventsCodes.ClosePieceMenu);
            eventBus.AddListener(this, GameEventsCodes.OpenPieceMenu);
        }
        
        if (isOpen) return false;

        cachedPiece = piece;
        eventBus.RaiseEvent(GameEventsCodes.OpenPieceMenu, piece);
        isOpen = true;
        return true;
    }

    public TouchReactionDefinitionMenu RegisterDefinition(TouchReactionDefinitionComponent definition, string image)
    {
        if (Definitions.ContainsKey(image)) return this;
        
        Definitions.Add(image, definition);
        return this;
    }

    public void OnBoardEvent(int code, object context)
    {
        if(code == GameEventsCodes.ClosePieceMenu && isOpen == false
            || code == GameEventsCodes.OpenPieceMenu && isOpen) return;

        if (code == GameEventsCodes.OpenPieceMenu)
        {
            OpenMenu(context as Piece);
            return;
        }
        
        CloseMenu(context as string);
    }
    
    private void OpenMenu(Piece piece)
    {
        if(piece.CachedPosition.Equals(cachedPiece.CachedPosition)) return;
            
        isOpen = false;
    }

    private void CloseMenu(string key)
    {
        isOpen = false;
        
        TouchReactionDefinitionComponent definition;
        
        if (Definitions.TryGetValue(key, out definition) == false) return;
        
        definition.Make(cachedPiece.CachedPosition, cachedPiece);
    }
}
