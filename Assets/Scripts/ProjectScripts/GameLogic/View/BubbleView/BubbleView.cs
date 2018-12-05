using System;
using UnityEngine;

public class BubbleView : UIBoardView, IBoardEventListener
{
    [SerializeField] private NSText message;
    [SerializeField] private NSText button;
    
    protected override ViewType Id => ViewType.Bubble;

    public override Vector3 Ofset => new Vector3(0, 2f);

    private Action<Piece> onClick;
    
    private bool isPausedHint;
    private bool isIgnoreHide;
    private bool isAddListener;

    public override void Init(Piece piece)
    {
        base.Init(piece);
		
        Priority = defaultPriority = 0;
    }

    public void SetData(string message, string button, Action<Piece> onClick, bool isPausedHint = true, bool isIgnoreHide = true)
    {
        this.message.Text = message;
        this.button.Text = button;
        this.onClick = onClick;

        this.isPausedHint = isPausedHint;
        this.isIgnoreHide = isIgnoreHide;
        
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);
        
        if (isIgnoreHide) return;
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
    }

    public void CleanOnClick()
    {
        onClick = null;
    }
    
    public override void UpdateVisibility(bool isVisible)
    {
        base.UpdateVisibility(isVisible);

        if (isPausedHint)
        {
            if (isVisible)
            {
                Context.Context.HintCooldown.Pause(this);
            }
            else
            {
                Context.Context.HintCooldown.Resume(this);
            }
        }
        
        if(isIgnoreHide == false) UpdateListener(isVisible);
    }
    
    private void UpdateListener(bool value)
    {
        if(isAddListener == value) return;
        
        isAddListener = value;
        
        if(isAddListener) Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
        else Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceUI || context is BoardPosition && ((BoardPosition) context).Equals(Context.CachedPosition)) return;
		
        Change(false);
    }

    public void OnClick()
    {
        Context.Context.TutorialLogic.Pause(true);
        onClick?.Invoke(Context);
    }
    
    public override void SyncRendererLayers(BoardPosition boardPosition)
    {
        base.SyncRendererLayers(boardPosition);
        
        if(canvas == null) return;
        
        canvas.overrideSorting = true;
        canvas.sortingOrder = GetLayerIndexBy(new BoardPosition(boardPosition.X, boardPosition.Y, BoardLayer.UIUP1.Layer));
    }
}