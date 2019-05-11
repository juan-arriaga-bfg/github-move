using BfgAnalytics;
using DG.Tweening;
using UnityEngine;

public enum FireflyType
{
    Production,
    Event,
}

public class FireflyView : BoardElementView
{
    [SerializeField] private Transform body;
    [SerializeField] private FireflyType fireflyType;
    [SerializeField] private float speedFirefly = 5f;
    
    public ParticleSystem Plume;
    
    private Vector2 to;
    private Vector2 current;
    
    private bool isClick;
    private HintArrowView arrow;

    public FireflyType FireflyType => fireflyType;
    
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
        DOTween.Kill(CachedTransform);
        
        Context.Context.BoardLogic.FireflyLogic.Remove(this);
        CachedTransform.localScale = Vector3.one;
        
        Plume.gameObject.SetActive(false);
        
        var temp = Plume.main;
        temp.loop = true;

        RemoveArrowImmediate();
    }

    public void AddArrow(float showDelay = 0, bool isLoop = true)
    {
        if (arrow != null) return;
        
        arrow = HintArrowView.Show(CachedTransform, 0, 1f, false, isLoop, showDelay);
        arrow.CachedTransform.SetParent(CachedTransform, true);
        arrow.AddOnRemoveAction(() =>
        {
            arrow = null;
        });
    }

    public void RemoveArrow(float delay = 0)
    {
        if(arrow == null) return;

        arrow.Remove(delay);
    }
    
    public void RemoveArrowImmediate()
    {
        if (arrow == null) return;
        
        arrow.gameObject.SetActive(false);
        arrow.CachedTransform.SetParent(null);
        RemoveArrow();
    }

    public void OnDragStart()
    {
        StopFly();
        
        NSAudioService.Current.Play(SoundId.FireflyTap);
    }

    public void StopFly()
    {
        DOTween.Kill(CachedTransform);
        current = CachedTransform.position;
    }

    public void StartFly()
    {
        var diff = (Vector2)CachedTransform.position - current;
        
        to = Context.Context.BoardLogic.FireflyLogic.Cross(CachedTransform.position, to + diff);
        Move();
    }

    public void OnDragEnd()
    {
        StartFly();
    }

    public void OnClick()
    {
        if(isClick) return;

        isClick = true;
        
        var boardPos = Context.Context.BoardDef.GetSectorPosition(CachedTransform.position);
        boardPos.Z = BoardLayer.Piece.Layer;
        
        var free = Context.Context.BoardLogic.EmptyCellsFinder.FindNearWithPointInCenter(boardPos, 1, 100);
        
        DOTween.Kill(CachedTransform);
        
        NSAudioService.Current.Play(SoundId.FireflyTap);
        
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

        CurrencyHelper.Purchase(Currency.Firefly.Name, 1);
        RemoveArrowImmediate();

        var rewardId = fireflyType == FireflyType.Production
            ? GameDataService.Current.LevelsManager.GetSequence(Currency.Level.Name).GetNext().Piece
            : PieceType.Token1.Id;
        
        Plume.gameObject.SetActive(true);
        Context.Context.ActionExecutor.AddAction(new FireflyPieceSpawnAction
        {
            PieceId = rewardId,
            At = free[0],
            View = this,
            FireflyType = fireflyType
        });

        if (fireflyType != FireflyType.Event)
        {
            Analytics.SendFireflyCollect();
        }
    }

    private void Move()
    {
        var lenght = Vector2.Distance(CachedTransform.position, to);
        
        CachedTransform.DOMove(to, lenght / (speedFirefly / 10f))
            .SetEase(Ease.Linear)
            .SetId(CachedTransform)
            .OnComplete(() => { Context.DestroyElement(gameObject); });
    }
}