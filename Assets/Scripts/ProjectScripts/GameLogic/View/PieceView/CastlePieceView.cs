using UnityEngine;

public class CastlePieceView : PieceBoardElementView
{
    [SerializeField] private Transform cap;
    [SerializeField] private GameObject shine;
    
    [SerializeField] private float open;
    [SerializeField] private float close;

    private StorageComponent storage;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if (storage == null) return;
        
        storage.Timer.OnStart += Change;
        storage.Timer.OnComplete += Change;
        
        Change();
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();

        if (storage == null) return;
        
        storage.Timer.OnStart -= Change;
        storage.Timer.OnComplete -= Change;
    }

    private void Change()
    {
        var isOpen = storage.Filling > 0;
        
        shine.SetActive(isOpen);
        cap.localPosition = new Vector3(cap.localPosition.x, isOpen ? open : close);
    }
}