using UnityEngine;

public class ChangeMineStateView : UIBoardView, IBoardEventListener
{
    [SerializeField] private NSText message;
    [SerializeField] private NSText progress;
    [SerializeField] private NSText price;
    
    private MineLifeComponent life;
    
    public override Vector3 Ofset
    {
        get { return new Vector3(0, 1.5f); }
    }

    public override void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }
    
    protected override ViewType Id
    {
        get { return ViewType.MineState; }
    }
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Priority = defaultPriority = 20;
        
        life = piece.GetComponent<MineLifeComponent>(MineLifeComponent.ComponentGuid);
        
        if(life == null) return;
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
    }
    
    public override void ResetViewOnDestroy()
    {
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);
        
        base.ResetViewOnDestroy();
    }

    public override void UpdateVisibility(bool isVisible)
    {
        base.UpdateVisibility(isVisible);

        if (IsShow == false) return;

        message.Text = "Clear mine " + life.Energy.ToStringIcon();
        price.Text = string.Format("Send<sprite name={0}>", life.Worker.Currency);
        progress.Text = string.Format("Attempts: {0}", life.HP - life.Current);
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