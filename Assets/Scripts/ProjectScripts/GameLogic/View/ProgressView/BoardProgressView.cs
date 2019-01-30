using UnityEngine;

public class BoardProgressView : UIBoardView
{
    [SerializeField] private BoardProgressbar bar; 
    
    public override Vector3 Offset => new Vector3(0, 0.3f);

    protected override ViewType Id => ViewType.Progress;
    
    private WorkplaceLifeComponent life;
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Priority = defaultPriority = -2;
        
        life = piece.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);

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
}