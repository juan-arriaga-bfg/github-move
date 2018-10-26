using UnityEngine;

public class PieceStorageView : PieceBoardElementView
{
    [SerializeField] private GameObject anchorObject;
    
    private StorageComponent storage;
    private PieceSkin skin;
    private WorkerView worker;

    private WorkerType lastWorkerType = WorkerType.Empty;

    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if(storage == null) return;

        storage.Timer.OnStart += UpdateAnimation;
        storage.Timer.OnComplete += UpdateAnimation;
        
        UpdateAnimation();
        
        skin = gameObject.GetComponent<PieceSkin>();
        
        if(skin == null) return;

        var life = Piece.GetComponent<LifeComponent>(LifeComponent.ComponentGuid);
        
        if(life == null) return;
        
        skin.Init(life.Value);
        storage.Timer.OnComplete += skin.UpdateView;
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        storage.Timer.OnStart -= UpdateAnimation;
        storage.Timer.OnComplete -= UpdateAnimation;
        
        if(skin != null) storage.Timer.OnComplete -= skin.UpdateView;
    }

    private void UpdateAnimation()
    {
        var currentWorkerType = Piece.Context.WorkerLogic.GetWorkerType(Piece.CachedPosition);
        if (lastWorkerType == currentWorkerType) return;
        Debug.LogError($"{Piece.CachedPosition} workType change from {lastWorkerType} to {currentWorkerType}");
        lastWorkerType = currentWorkerType; 
        
        if (worker != null)
        {
            worker.WorkAnimation.ResetTrigger(storage.Timer.IsExecuteable() ? "IsStop" : "IsPlay");
            Context.DestroyElement(worker);
        }

        if (currentWorkerType == WorkerType.Empty)
        {
            worker = null;
            return;
        }
        else
        {
            var resourceName = currentWorkerType == WorkerType.Default ? R.DefaultWorker : R.ExtraWorker;
            worker = Context.CreateBoardElementAt<WorkerView>(resourceName, new BoardPosition(Piece.CachedPosition.X, Piece.CachedPosition.Y, Piece.CachedPosition.Z + 1));
            worker.CachedTransform.SetParent(anchorObject.transform);
            worker.CachedTransform.localPosition = Vector3.zero;
            worker.WorkAnimation.SetTrigger(storage.Timer.IsExecuteable() ? "IsPlay" : "IsStop");    
        }
        
    }
}