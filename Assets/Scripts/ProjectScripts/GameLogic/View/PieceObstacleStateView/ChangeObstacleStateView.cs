using UnityEngine;

public class ChangeObstacleStateView : UIBoardView, IBoardEventListener
{
    [SerializeField] private NSText message;
    [SerializeField] private NSText price;
    
    [SerializeField] private BoardProgressbar bar;
    
    private StorageLifeComponent life;
    
    public override Vector3 Ofset => new Vector3(0, 1.5f);
    
    protected override ViewType Id => ViewType.ObstacleState;
    
    public override void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        life = piece.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);
        
        if(life == null) return;
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
        
        bar.Init(life.HP);

        if (life.IsUseCooldown)
        {
            life.Timer.OnExecute += UpdateButtonText;
            life.Timer.OnComplete += UpdateButtonText;
        }
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

    public override void UpdateVisibility(bool isVisible)
    {
        base.UpdateVisibility(isVisible);

        if (IsShow == false) return;
        
        message.Text = life.Message;
        UpdateButtonText();
        
        bar.UpdateValue(life.GetProgress, life.GetProgressNext);
    }

    private void UpdateButtonText()
    {
        price.Text = life.Price;
    }
    
    public void Clear()
    {
        if (life.Damage()) Change(false);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceUI || context is BoardPosition && ((BoardPosition) context).Equals(Context.CachedPosition)) return;
		
        Change(false);
    }
}