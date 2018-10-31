using UnityEngine;

public class ObstaclePieceView : PieceBoardElementView
{
    [SerializeField] private Transform anchorWorker;
    
    private StorageComponent storage;
    private PieceSkin skin;
    private GameObject worker;
    
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
            RemoveFromLayerCache(worker);
            Context.DestroyElement(worker);
            worker = null;
        }
        
        if(storage.Timer.IsExecuteable() == false) return;
        
        worker = Context.CreateElement((int)(isExtra ? ViewType.ExtraWorker : ViewType.DefaultWorker)).gameObject;
        worker.transform.SetParent(anchorWorker, false);
        
        AddToLayerCache(worker);
        
        SyncRendererLayers(Piece.CachedPosition);
    }
}