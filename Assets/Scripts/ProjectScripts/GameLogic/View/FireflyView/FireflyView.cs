using DG.Tweening;
using UnityEngine;

public class FireflyView : BoardElementView
{
    [SerializeField] private Transform body;
    
    public ParticleSystem Plume;
    
    private Vector2 to;
    private Vector2 current;
    
    private bool isClick;
    private HintArrowView arrow;
    
    public void Init(BoardRenderer context, Vector2 start, Vector2 finish)
    {
        base.Init(context);

        isClick = false;
        
        to = Context.Context.BoardLogic.FireflyLogic.Cross(start, finish);
        CachedTransform.position = start;
        Move();
        
        Plume.gameObject.SetActive(false);
        
        SyncRendererLayers(new BoardPosition(context.Context.BoardDef.Width, 0, BoardLayer.UI.Layer));
    }
    
    public override void OnFastInstantiate()
    {
    }
    
    public override void OnFastDestroy()
    {
        Context.Context.BoardLogic.FireflyLogic.Remove(this);
        CachedTransform.localScale = Vector3.one;
        
        Plume.gameObject.SetActive(false);
        
        var temp = Plume.main;
        temp.loop = true;
    }

    public void AddArrow()
    {
        arrow = HintArrowView.Show(CachedTransform, 0, 1f, false, true);
        arrow.CachedTransform.SetParent(CachedTransform, true);
    }

    public void RemoveArrow()
    {
        if(arrow == null) return;
        
        arrow.CachedTransform.SetParent(null);
        arrow.Remove(0);
    }

    public void OnDragStart()
    {
        DOTween.Kill(CachedTransform);
        current = CachedTransform.position;
    }

    public void OnDragEnd()
    {
        var diff = (Vector2)CachedTransform.position - current;
        
        to = Context.Context.BoardLogic.FireflyLogic.Cross(CachedTransform.position, to + diff);
        Move();
    }

    public void OnClick()
    {
        if(isClick) return;

        isClick = true;
        
        var boardPos = Context.Context.BoardDef.GetSectorPosition(CachedTransform.position);
        boardPos.Z = BoardLayer.Piece.Layer;
        
        var free = Context.Context.BoardLogic.EmptyCellsFinder.FindNearWithPointInCenter(boardPos, 1, 100);
        
        DOTween.Kill(CachedTransform);
        
        if (free.Count == 0)
        {
            DOTween.Sequence()
                .Append(body.DOLocalMoveY(-0.07f, 0.06f))
                .Append(body.DOLocalMoveY(0.07f, 0.06f))
                .SetLoops(6)
                .OnComplete(() =>
                {
                    isClick = false;
                    Move();
                });
            
            return;
        }

        CurrencyHellper.Purchase(Currency.Firefly.Name, 1);
        RemoveArrow();
        
        Plume.gameObject.SetActive(true);
        Context.Context.ActionExecutor.AddAction(new FireflyPieceSpawnAction
        {
            PieceId = GameDataService.Current.LevelsManager.GetSequence(Currency.Level.Name).GetNext().Piece,
            At = free[0],
            View = this
        });
    }

    private void Move()
    {
        var lenght = Vector2.Distance(CachedTransform.position, to);
        
        CachedTransform.DOMove(to, lenght / (GameDataService.Current.ConstantsManager.SpeedFirefly / 10f))
            .SetEase(Ease.Linear)
            .SetId(CachedTransform)
            .OnComplete(() => { Context.DestroyElement(gameObject); });
    }
}