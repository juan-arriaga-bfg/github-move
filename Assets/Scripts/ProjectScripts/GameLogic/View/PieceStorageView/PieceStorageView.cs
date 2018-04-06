using UnityEngine;

public class PieceStorageView : UIBoardView
{
    [SerializeField] private SpriteRenderer icon;
    
    private StorageComponent storage;

    public override Vector3 Ofset
    {
        get { return new Vector3(0, (multiSize == 0 ?  -0.3f : 0.7f)); }
    }

    protected override void SetOfset()
    {
        if (multiSize == 0)
        {
            base.SetOfset();
            return;
        }
        
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if (storage == null) return;
        
        icon.sprite = IconService.Current.GetSpriteById(PieceType.Parse(storage.SpawnPiece));
        storage.Timer.OnComplete += UpdateView;
        storage.Timer.OnStart += UpdateView;
        UpdateView();
    }
    
    private void UpdateView()
    {
        if (storage == null) return;
        Change(storage.Filling / (float) storage.Capacity > 0.2);
    }
}