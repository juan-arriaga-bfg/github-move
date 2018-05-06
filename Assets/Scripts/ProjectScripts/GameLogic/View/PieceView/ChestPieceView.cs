using UnityEngine;

public class ChestPieceView : PieceBoardElementView
{
    [SerializeField] private Transform cap;
    
    [SerializeField] private GameObject shine;
    
    [SerializeField] private float open;
    [SerializeField] private float close;
    
    private ChestPieceComponent chestComponent;

    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        chestComponent = piece.GetComponent<ChestPieceComponent>(ChestPieceComponent.ComponentGuid);
        chestComponent.Timer.OnStop += UpdateView;
        chestComponent.Timer.OnComplete += UpdateView;
        
        UpdateView();
        
        var hint = Context.Context.GetComponent<HintCooldownComponent>(HintCooldownComponent.ComponentGuid);
        
        if(hint == null) return;
        
        hint.Step(HintType.CloseChest);
    }

    public override void ResetViewOnDestroy()
    {
        chestComponent.Timer.OnStop -= UpdateView;
        chestComponent.Timer.OnComplete -= UpdateView;
        base.ResetViewOnDestroy();
    }

    public override void UpdateView()
    {
        if(chestComponent == null || chestComponent.Chest == null) return;
        
        var isOpen = chestComponent.Chest.State == ChestState.Open;
        
        shine.SetActive(isOpen);
        
        cap.localPosition = new Vector3(cap.localPosition.x, isOpen ? open : close);
    }
}