using UnityEngine;

public class ObstaclePieceView : PieceBoardElementView
{
    [SerializeField] private Animator worker;
    
    private StorageComponent storage;
    private PieceSkin skin;

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
        worker.ResetTrigger(storage.Timer.IsExecuteable() ? "IsStop" : "IsPlay");
        worker.SetTrigger(storage.Timer.IsExecuteable() ? "IsPlay" : "IsStop");
    }
}