using UnityEngine;

public class ObstacleBubbleView : UIBoardView, IBoardEventListener
{
    [SerializeField] private NSText message;
    [SerializeField] private NSText price;
    
    [SerializeField] private BoardProgressbar bar;
    
    private WorkplaceLifeComponent life;
    
    public override Vector3 Offset => new Vector3(0, 1.5f);
    
    protected override ViewType Id => ViewType.ObstacleBubble;

    private bool isAddListener;

    public override void SetOffset()
    {
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Offset;
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Priority = defaultPriority = 1;
        
        life = piece.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);
        
        if(life == null) return;
        
        bar.IsVisible = life.HP != -1;
        bar.Init(life.HP);

        if (life.IsUseCooldown == false) return;
        
        life.TimerCooldown.OnExecute += UpdateButtonText;
        life.TimerCooldown.OnComplete += UpdateButtonText;
    }

    public override void ResetViewOnDestroy()
    {
        bar.Clear();
        
        if (life.IsUseCooldown)
        {
            life.TimerCooldown.OnExecute -= UpdateButtonText;
            life.TimerCooldown.OnComplete -= UpdateButtonText;
        }
        
        base.ResetViewOnDestroy();
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
        if(IsShow == false || life.Rewards.IsScatter) return;
        
        Context.Context.TutorialLogic.Pause(true);
        
        if (life.Damage())
        {
            var typeDef = PieceType.GetDefById(Context.PieceType);
		
            if (typeDef.Filter.HasFlag(PieceTypeFilter.Mine))
            {
                NSAudioService.Current.Play(SoundId.WorkerMine);
            }
            else if(typeDef.Filter.HasFlag(PieceTypeFilter.Obstacle))
            {
                NSAudioService.Current.Play(SoundId.WorkerChop);
            }
            else if(typeDef.Filter.HasFlag(PieceTypeFilter.ProductionField))
            {
                NSAudioService.Current.Play(SoundId.WorkerHarvest);
            }
        }
            
        Change(false);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceUI || context is BoardPosition position && position.Equals(Context.CachedPosition)) return;
		
        Change(false);
    }
    
    public override void SyncRendererLayers(BoardPosition boardPosition)
    {
        base.SyncRendererLayers(boardPosition);
        
        if(canvas == null) return;
        
        canvas.overrideSorting = true;
        canvas.sortingOrder = GetLayerIndexBy(new BoardPosition(boardPosition.X, boardPosition.Y, BoardLayer.UIUP1.Layer));
    }
}