using DG.Tweening;
using UnityEngine;

public class FireflyView : BoardElementView
{
    [SerializeField] private Transform body;
    public ParticleSystem Plume;
    
    private Vector2 bottom;
    private Vector2 right;
    
    private Vector2 to;
    private Vector2 current;
    
    private bool isFirst = true;
    private bool isClick;
    
    public virtual void Init(BoardRenderer context)
    {
        base.Init(context);

        isClick = false;
        
        var positionStart = new Vector2(-Random.Range(50, 150), Screen.height + Random.Range(50, 150));
        var positionFinish = new Vector2(Screen.width, 0);

        if (Random.Range(0, 2) == 0) positionStart.y = Random.Range(Screen.height / 2f, Screen.height);
        else positionStart.x = Random.Range(0, Screen.width / 3f);

        if (Random.Range(0, 2) == 0) positionFinish.y = Random.Range(0, Screen.height / 2f);
        else positionFinish.x = Random.Range(2 * Screen.width / 3f, Screen.width);
        
        if (isFirst)
        {
            isFirst = false;
            
            bottom = Context.Context.BoardDef.GetWorldPosition(Context.Context.BoardDef.Width + 5, 0);
            right = Context.Context.BoardDef.GetWorldPosition(Context.Context.BoardDef.Width + 5, Context.Context.BoardDef.Height + 5);
        }
        
        Vector2 from = Context.Context.BoardDef.ViewCamera.ScreenToWorldPoint(positionStart);
        Vector2 temp = Context.Context.BoardDef.ViewCamera.ScreenToWorldPoint(positionFinish);
        
        to = Cross(from, temp);
        CachedTransform.position = from;
        Move();
        
        Plume.gameObject.SetActive(false);
    }
    
    public override void OnFastInstantiate()
    {
    }
    
    public override void OnFastDestroy()
    {
        Context.Context.BoardLogic.FireflyLogic.Remove();
        CachedTransform.localScale = Vector3.one;
        
        Plume.gameObject.SetActive(false);
        
        var temp = Plume.main;
        temp.loop = true;
    }

    public void OnDragStart()
    {
        DOTween.Kill(CachedTransform);
        current = CachedTransform.position;
    }

    public void OnDragEnd()
    {
        var diff = (Vector2)CachedTransform.position - current;

        to = to + diff;
        Move();
    }

    public void OnClick()
    {
        if(isClick) return;

        isClick = true;
        
        var boardPos = Context.Context.BoardDef.GetSectorPosition(CachedTransform.position);
        boardPos.Z = Context.Context.BoardDef.PieceLayer;
        
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
    
    private Vector2 Cross(Vector2 a, Vector2 b) //точки a и b концы первого отрезка
    {
        Vector2 T;
        
        T.x = -((a.x * b.y - b.x * a.y) * (right.x - bottom.x) - (bottom.x * right.y - right.x * bottom.y) * (b.x - a.x)) / ((a.y - b.y) * (right.x - bottom.x) - (bottom.y - right.y) * (b.x - a.x));
        T.y = ((bottom.y - right.y) * (-T.x) - (bottom.x * right.y - right.x * bottom.y)) / (right.x - bottom.x);
        
        return T;
    }
}