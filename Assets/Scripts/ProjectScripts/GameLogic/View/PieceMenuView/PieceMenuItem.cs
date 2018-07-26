using UnityEngine;
using UnityEngine.UI;

public class PieceMenuItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    
    private Piece piece;
    private TouchReactionDefinitionComponent definition;
    
    public void Decoration(TouchReactionDefinitionComponent definition, Piece piece)
    {
        this.piece = piece;
        this.definition = definition;

        icon.sprite = IconService.Current.GetSpriteById(definition.Icon);
    }

    public void OnClick()
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
        definition.Make(piece.CachedPosition, piece);
    }
}