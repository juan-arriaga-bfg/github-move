using UnityEngine;

public class ChangeSimpleObstacleStateView : UIBoardView, IBoardEventListener
{
    [SerializeField] private NSText message;
    [SerializeField] private NSText amount;
    [SerializeField] private NSText progress;
    
    [SerializeField] private BoardProgressBarView progressBar;

    private TouchReactionDefinitionSimpleObstacle simpleObstacle;
    
    public override Vector3 Ofset
    {
        get { return new Vector3(0, -0.5f); }
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        amount.Text = GameDataService.Current.ObstaclesManager.SimpleObstaclePrice.Amount.ToString();
        
        var touchReaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
        
        if(touchReaction == null) return;
        
        simpleObstacle = touchReaction.GetComponent<TouchReactionDefinitionSimpleObstacle>(TouchReactionDefinitionSimpleObstacle.ComponentGuid);
        
        if(simpleObstacle == null) return;
        
        simpleObstacle.OnClick = OnClick;
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceMenu);
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceMenu);
    }
    
    private void OnClick()
    {
        Context.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);

        SetProgress();
        simpleObstacle.isOpen = true;

        Change(true);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceMenu) return;
        if(simpleObstacle.isOpen == false) return;

        simpleObstacle.isOpen = false;
        Change(false);
    }

    private void SetProgress()
    {
        if (progress != null) progress.Text = simpleObstacle.GetProgressText;
        if (progressBar != null) progressBar.SetProgress(simpleObstacle.GetProgress);
    }
}