using UnityEngine;

public class CastlePieceView : PieceBoardElementView
{
    [SerializeField] private GameObject chest;
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
        chest.SetActive(storage.Filling > 0);
    }

    public Vector3 GetSpawnPosition()
    {
        return chest.transform.position;
    }
}