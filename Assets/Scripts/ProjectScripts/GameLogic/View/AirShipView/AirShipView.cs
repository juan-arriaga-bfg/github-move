using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class AirShipView : BoardElementView
{
    private enum AirShipState
    {
        Unknown,
        Idle,
        Drop,
        Death
    }
    
    [SerializeField] private Transform body;
    [SerializeField] private List<Transform> pieceAnchors;
    
    private bool isClick;
    private HintArrowView arrow;

    private Dictionary<int, int> pieces;

    private List<int> normalizedPieces;
    
    public void Init(BoardRenderer context, Dictionary<int, int> pieces)
    {
        base.Init(context);

        this.pieces = pieces;
        
        isClick = false;

        CreatePieces();

        FixZ();
    }

    private void FixZ()
    {
        SyncRendererLayers(new BoardPosition(Context.Context.BoardDef.Width, 0, BoardLayer.UI.Layer));
        for (var i = 0; i < pieceAnchors.Count; i++)
        {
            var anchor = pieceAnchors[i];
            anchor.gameObject.GetComponent<SortingGroup>().sortingOrder = pieceAnchors.Count - i;
        }
    }

    private void CreatePieces()
    {
        NormilizePieces();
        
        int currentSlot = -1;

        foreach (var pair in pieces)
        {
            var pieceId = pair.Key;
            var count = pair.Value;
            
            for (var i = 0; i < count; i++)
            {
                currentSlot++;
                if (currentSlot > pieceAnchors.Count - 1)
                {
                    return;
                }
                
                var anchor = pieceAnchors[currentSlot];
                var id = pieceId;
                PieceBoardElementView pieceView = Context.Context.BoardLogic.DragAndDrop.CreateFakePieceOutsideOfBoard(id);
                pieceView.transform.SetParent(anchor, false);
                pieceView.transform.localScale = Vector3.one;
                pieceView.transform.localPosition = Vector3.zero;
                pieceView.SyncRendererLayers(new BoardPosition(Context.Context.BoardDef.Width, 0, BoardLayer.UI.Layer));
            }
        }
    }

    private void NormilizePieces()
    {
        normalizedPieces = new List<int>();
        
        foreach (var pair in pieces)
        {
            var pieceId = pair.Key;
            var count = pair.Value;
            
            for (var i = 0; i < count; i++)
            {
                var id = pieceId;
                normalizedPieces.Add(id);
            }
        }
    }

    public override void OnFastInstantiate()
    {
    }
    
    public override void OnFastDestroy()
    {
        StopAnimation();
        
        DOTween.Kill(CachedTransform);
        
        Context.Context.BoardLogic.AirShipLogic.Remove(this);
        CachedTransform.localScale = Vector3.one;


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
        StopAnimation();
    }

    public void OnDragEnd()
    {
        AnimateIdle();
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

        CurrencyHelper.Purchase(Currency.Firefly.Name, 1);
        RemoveArrowImmediate();
        
        Context.Context.ActionExecutor.AddAction(new AirShipPieceSpawnAction
        {
            PieceId = GameDataService.Current.LevelsManager.GetSequence(Currency.Level.Name).GetNext().Piece,
            At = free[0],
            View = this
        });
    }

    private void Move()
    {
        // CachedTransform.DOMove(to, lenght / (GameDataService.Current.ConstantsManager.SpeedFirefly / 10f))
        //     .SetEase(Ease.Linear)
        //     .SetId(CachedTransform)
        //     .OnComplete(() => { Context.DestroyElement(gameObject); });
    }

    public void PlaceTo(Vector2 position)
    {
        CachedTransform.position = position;
    }

    public void AnimateIdle()
    {
        StopAnimation();

        const float DIST = 0.25f;
        float TIME = UnityEngine.Random.Range(1.8f, 2.2f);
        
        DOTween.Sequence()
               .SetId(CachedTransform)
                
               .Append(CachedTransform.DOMove(Vector3.up * DIST, TIME)
                                      .SetRelative(true)
                                      .SetId(CachedTransform)
                                      .SetEase(Ease.InOutSine))
                
               .Append(CachedTransform.DOLocalMove(Vector3.down * DIST, TIME)
                                      .SetRelative(true)
                                      .SetId(CachedTransform)
                                      .SetEase(Ease.InOutSine))
                
               .SetLoops(-1);
    }

    public void AnimateSpawn()
    {
        
    }

    public void AnimateDeath()
    {
        
    }

    public void StopAnimation()
    {
        DOTween.Kill(CachedTransform); 
    }

    private void OnDestroy()
    {
        StopAnimation();
    }
}