using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObstaclePieceView : PieceBoardElementView
{
    [SerializeField] private Transform anchorWorker;

    private WorkplaceLifeComponent life;
    
    private PieceSkin skin;
    private GameObject worker;
    private List<SpriteRenderer> workerSprites;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);
        
        life = Piece.GetComponent<WorkplaceLifeComponent>(WorkplaceLifeComponent.ComponentGuid);
        
        if(life == null) return;

        life.Timer.OnStart += UpdateAnimation;
        life.Timer.OnComplete += UpdateAnimation;
        
        UpdateAnimation();
        
        skin = gameObject.GetComponent<PieceSkin>();
        
        if(skin == null) return;
        
        skin.Init(life.Value);
        life.Timer.OnComplete += skin.UpdateView;
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        life.Timer.OnStart -= UpdateAnimation;
        life.Timer.OnComplete -= UpdateAnimation;
        
        if(skin != null) life.Timer.OnComplete -= skin.UpdateView;
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
            var gnom = worker;
            
            worker.GetComponent<Animator>().SetTrigger("Hide");

            DOTween.Sequence().AppendInterval(1f).AppendCallback(() =>
            {
                RemoveFromLayerCache(gnom);
                Context.DestroyElement(gnom);
            });
            
            bodySprites.RemoveAll(elem => workerSprites.Contains(elem));
            
            workerSprites = null;
            worker = null;
        }
        
        if(life.Timer.IsExecuteable() == false) return;
        
        worker = Context.CreateElement((int)(isExtra ? ViewType.ExtraWorker : ViewType.DefaultWorker)).gameObject;
        worker.transform.SetParent(anchorWorker, false);

        workerSprites = new List<SpriteRenderer>(worker.GetComponentsInChildren<SpriteRenderer>());
        bodySprites.AddRange(workerSprites);
        
        AddToLayerCache(worker);
        
        SyncRendererLayers(Piece.CachedPosition);
    }
}