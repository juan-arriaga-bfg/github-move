using UnityEngine;

public class SimpleUpgradeView : UIBoardView
{
    protected override ViewType Id
    {
        get { return ViewType.SimpleUpgrade; }
    }
    
    public override Vector3 Ofset
    {
        get { return new Vector3(0, 1.5f); }
    }

    public override void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        Priority = defaultPriority = 2;
    }
    
    public void OnClick()
    {
        var definition = Context.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);

        if (definition == null) return;
        
        definition.Touch(Context.CachedPosition);
    }
}