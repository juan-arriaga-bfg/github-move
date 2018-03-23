using System;
using UnityEngine;

public class BoardButtonView : PieceTouchRegion
{
    public enum Color
    {
        Blue,
        Green,
        Orange,
    }
    
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private SpriteRenderer border;

    private string key;
    
    public void Init(string key, BoardButtonView.Color color)
    {
        this.key = key;
        
        icon.sprite = IconService.Current.GetSpriteById(key);

        switch (color)
        {
            case Color.Blue:
                border.sprite = IconService.Current.GetSpriteById("card_border_Lvl_Hero");
                break;
            case Color.Green:
                border.sprite = IconService.Current.GetSpriteById("card_border_Lvl_Build");
                break;
            case Color.Orange:
                border.sprite = IconService.Current.GetSpriteById("card_border_Lvl_other");
                break;
        }
    }

    public override bool IsTouchableAt(Vector2 pos)
    {
        var isTouchableAt = base.IsTouchableAt(pos);

        if (isTouchableAt)
        {
            BoardService.Current.GetBoardById(0).BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, key);
        }

        return isTouchableAt;
    }

    public static BoardButtonView.Color GetColor(TouchReactionDefinitionComponent definition)
    {
        if (definition is TouchReactionDefinitionUpgrade) return Color.Blue;
        
        if (definition is TouchReactionDefinitionOpenTavernWindow) return Color.Orange;
        
        return Color.Green;
    }
}