using UnityEngine;

public class ObstaclePieceView : PieceBoardElementView
{
    [SerializeField] private GameObject anchorWorker;
    
    private StorageComponent storage;
    private PieceSkin skin;
    private WorkerView worker;
    
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
        var isExtra = !Piece.Context.WorkerLogic.Check(Piece.CachedPosition);
        ChangeAnimationState(isExtra);
    }

    private void ChangeAnimationState(bool isExtra)
    {
        if (worker != null)
        {
            worker.WorkAnimation.ResetTrigger("IsPlay");
            Context.DestroyElement(worker);
        }
        
        if(storage.Timer.IsExecuteable() == false) return;
        
        var resourceName = isExtra ? R.ExtraWorker : R.DefaultWorker;
        worker = Context.CreateBoardElementAt<WorkerView>(resourceName, new BoardPosition(Piece.CachedPosition.X, Piece.CachedPosition.Y, Piece.CachedPosition.Z + 1));
        worker.CachedTransform.SetParent(anchorWorker.transform);
        worker.CachedTransform.localPosition = Vector3.zero;
        worker.WorkAnimation.SetTrigger("IsPlay");
    }
}