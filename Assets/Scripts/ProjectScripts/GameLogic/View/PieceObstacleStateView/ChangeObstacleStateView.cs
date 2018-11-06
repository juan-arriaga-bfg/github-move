using UnityEngine;

public class ChangeObstacleStateView : UIBoardView, IBoardEventListener
{
    [SerializeField] private NSText message;
    [SerializeField] private NSText price;
    
    [SerializeField] private BoardProgressbar bar;
    
    private StorageLifeComponent life;
    
    public override Vector3 Ofset => new Vector3(0, 1.5f);
    
    protected override ViewType Id => ViewType.ObstacleState;

    private bool isAddListener;

    public override void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);

        life = piece.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);
        
        if(life == null) return;

        bar.IsVisible = life.HP != -1;
        bar.Init(life.HP);

        if (life.IsUseCooldown == false) return;
        
        life.Timer.OnExecute += UpdateButtonText;
        life.Timer.OnComplete += UpdateButtonText;
    }
    
    public override void ResetViewOnDestroy()
    {
        bar.Clear();
        
        if (life.IsUseCooldown)
        {
            life.Timer.OnExecute -= UpdateButtonText;
            life.Timer.OnComplete -= UpdateButtonText;
        }
        
        base.ResetViewOnDestroy();
    }
    
    public override void OnDrag(bool isEnd)
    {
        base.OnDrag(isEnd);
        
        if (life == null) return;

        life.Timer.IsPaused = !isEnd;
    }
    
    public override void UpdateVisibility(bool isVisible)
    {
        if (isVisible)
        {
            Context.Context.HintCooldown.Pause(this);
        }
        else
        {
            Context.Context.HintCooldown.Resume(this);
        }

        base.UpdateVisibility(isVisible);
        
        UpdateListener(isVisible);
        
        if (IsShow == false) return;
        
        message.Text = life.Message;
        UpdateButtonText();
        
        bar.UpdateValue(life.GetProgress, life.GetProgressNext);
    }
    
    private void UpdateButtonText()
    {
        price.Text = life.Price;
    }
    
    private void UpdateListener(bool value)
    {
        if(isAddListener == value) return;
        
        isAddListener = value;
        
        if(isAddListener) Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
        else Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);
    }
    
    public void Clear()
    {
        life.Damage();
        Change(false);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceUI || context is BoardPosition && ((BoardPosition) context).Equals(Context.CachedPosition)) return;
		
        Change(false);
    }
}