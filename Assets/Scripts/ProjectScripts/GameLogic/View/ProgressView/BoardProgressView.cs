using UnityEngine;

public class BoardProgressView : UIBoardView
{
    [SerializeField] private BoardProgressbar bar; 
    
    public override Vector3 Ofset => new Vector3(0, 0.3f);

    protected override ViewType Id => ViewType.Progress;
    
    private StorageLifeComponent life;
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Priority = defaultPriority = -2;
        
        life = piece.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);

        if (life == null) return;
        
        bar.Init(life.HP);
    }
    
    public override void ResetViewOnDestroy()
    {
        bar.Clear();
        base.ResetViewOnDestroy();
    }

    public override void UpdateVisibility(bool isVisible)
    {
        base.UpdateVisibility(isVisible);

        if (IsShow == false) return;
        
        bar.UpdateValue(life.GetProgress, life.GetProgressNext);
    }
    
    public override void OnDrag(bool isEnd)
    {
        base.OnDrag(isEnd);
        
        if (life == null) return;

        life.Timer.IsPaused = !isEnd;
    }
}