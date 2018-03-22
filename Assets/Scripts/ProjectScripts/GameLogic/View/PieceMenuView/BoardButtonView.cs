using UnityEngine;

public class BoardButtonView : PieceTouchRegion
{
    [SerializeField] private SpriteRenderer icon;

    private string key;
    
    public void Init(string key)
    {
        this.key = key;
        
        icon.sprite = IconService.Current.GetSpriteById(key);
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
}