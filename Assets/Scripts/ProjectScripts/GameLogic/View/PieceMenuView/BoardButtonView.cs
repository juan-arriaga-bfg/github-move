using System;
using UnityEngine;

public class BoardButtonView : PieceTouchRegion, ITouchRegionListener
{
    public enum Color
    {
        Blue,
        Green,
        Orange,
    }
    
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private SpriteRenderer border;
    
    private Action<BoardButtonView> onClick;

    private CameraManipulator Manipulator
    {
        get
        {
            if (BoardService.Current.GetBoardById(0) == null || BoardService.Current.GetBoardById(0).Manipulator == null) return null;
            return BoardService.Current.GetBoardById(0).Manipulator.CameraManipulator;
        }
    }

    public void Init(string key, BoardButtonView.Color color, Action<BoardButtonView> onClick)
    {
        icon.sprite = IconService.Current.GetSpriteById(key);
        this.onClick = onClick;
        
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
    
    public void OnEnable()
    {
        if (Manipulator != null)
        {
            Manipulator.RegisterTouchListener(this);
        }
    }

    private void OnDisable()
    {
        if (Manipulator != null)
        {
            Manipulator.UnRegisterTouchListener(this);
        }
    }

    public bool OnTap(Vector2 startPos, Vector2 pos, int tapCount)
    {
        if (IsTouchableAt(pos) == false) return false;
        
        if (onClick != null)
        {
            onClick(this);
        }

        return true;
    }

    public bool OnSet(Vector2 startPos, Vector2 pos, float duration)
    {
        return false;
    }

    public bool OnDown(Vector2 startPos, Vector2 pos)
    {
        return false;
    }

    public bool OnUp(Vector2 startPos, Vector2 pos)
    {
        return false;
    }
}