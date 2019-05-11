using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class AirShipView : BoardElementView
{
    [SerializeField] private Transform body;
    [SerializeField] private List<Transform> pieceAnchors;
    
    public bool isShow;
    private bool isClick;
    private HintArrowView arrow;

    public int Id { get; private set; }

    public void Init(BoardRenderer context, AirShipDef def)
    {
        base.Init(context);

        Id = def.Id;
        
        isShow = false;
        isClick = false;

        UpdatePayload(def);

        FixZ();
    }

    private void FixZ()
    {
        SyncRendererLayers(GetCellForSyncLayers());
        for (var i = 0; i < pieceAnchors.Count; i++)
        {
            var anchor = pieceAnchors[i];
            anchor.gameObject.GetComponent<SortingGroup>().sortingOrder = pieceAnchors.Count - i;
        }
    }

    private void ClearPayload()
    {
        foreach (var anchor in pieceAnchors)
        {
            if (anchor.childCount == 0) continue;
            
            var child = anchor.GetChild(0).GetComponent<PieceBoardElementView>();

            if (child != null)
            {
                Context.Context.BoardLogic.DragAndDrop.DestroyFakePiece(child);
            }
        }
    }
    
    public void UpdatePayload(AirShipDef def)
    {
        ClearPayload();
        
        var list = def.SortedPayload;
        
        for (var i = 0; i < pieceAnchors.Count && i < list.Count; i++)
        {
            var anchor = pieceAnchors[i];
            var id = list[i];
            
            var pieceView = Context.Context.BoardLogic.DragAndDrop.CreateFakePieceOutsideOfBoard(id);
            
            pieceView.CachedTransform.SetParent(anchor, false);
            pieceView.CachedTransform.localScale = Vector3.one;
            pieceView.CachedTransform.localPosition = Vector3.zero;
            pieceView.SyncRendererLayers(GetCellForSyncLayers());
        }
    }

    private BoardPosition GetCellForSyncLayers()
    {
        return new BoardPosition(Context.Context.BoardDef.Width, 0, BoardLayer.UI.Layer);
    }

    public override void OnFastInstantiate()
    {
    }
    
    public override void OnFastDestroy()
    {
        StopAnimation();

        CachedTransform.localScale = Vector3.one;

        ClearPayload();

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
        isClick = false;
    }

    public void OnDragEnd()
    {
        if (!FixPosIfNearTheWorldEnd())
        {
            AnimateIdle(); 
        }
    }

    private bool FixPosIfNearTheWorldEnd()
    {
        const float SAFE_ZONE = 4.5f;
        
        var manipulator = Context.Context.Manipulator.CameraManipulator;
        var bounds = manipulator.CurrentCameraSettings.CameraClampRegion;
        var pos = CachedTransform.position;

        bounds.x += SAFE_ZONE;
        bounds.y += SAFE_ZONE;
        bounds.width  -= SAFE_ZONE * 2;
        bounds.height -= SAFE_ZONE * 2;

        if (bounds.Contains(pos))
        {
            return false;
        }
        
        Vector2 delta = new Vector2();
        if (pos.x < bounds.x)
        {
            delta.x = bounds.x - pos.x;
        }
        
        if (pos.y < bounds.y)
        {
            delta.y = bounds.y - pos.y;
        }
        
        if (pos.x > bounds.x + bounds.width )
        {
            delta.x = (bounds.x + bounds.width) - pos.x;
        }
        
        if (pos.y > bounds.y + bounds.height )
        {
            delta.y = (bounds.y + bounds.height) - pos.y;
        }

        DOTween.Kill(CachedTransform);

        CachedTransform.DOMove(delta, 0.5f)
                       .SetRelative(true)
                       .SetId(CachedTransform)
                       .OnComplete(AnimateIdle);

        return true;
    }
    
    public void PlaceTo(Vector2 position)
    {
        CachedTransform.position = position;
        body.localScale = Vector3.zero;
    }
    
    public void OnClick()
    {
        if (isClick) return;

        isClick = true;
        
        RemoveArrowImmediate();

        if (Context.Context.BoardLogic.AirShipLogic.DropPayload(Id, out var partialDrop)) return;
        
        if (partialDrop)
        {
            isClick = false;
            return;
        }
        
        // Nothing dropped
        AnimateError();
    }

    public void AnimateIdle()
    {
        StopAnimation();

        isShow = true;
        body.localScale = Vector3.one;
        
        const float DIST = 0.25f;
        var TIME = Random.Range(1.8f, 2.2f);

        var distFromZero = body.localPosition.y;
        var normalizedDistFromZero = distFromZero / DIST; 
       
        // At the first, we should return body to initial position to avoid jitter
        DOTween.Sequence()
               .SetId(body)
               .Append(body.DOLocalMove(Vector3.zero, TIME * normalizedDistFromZero)
                           .SetId(body)
                           .SetEase(Ease.InOutSine))
               .OnComplete(() =>
                {
                    // And then floating animation
                    DOTween.Sequence()
                           .SetId(body)
                
                           .Append(body.DOMove(Vector3.up * DIST, TIME)
                                       .SetRelative(true)
                                       .SetId(body)
                                       .SetEase(Ease.InOutSine))
                
                           .Append(body.DOLocalMove(Vector3.down * DIST, TIME)
                                       .SetRelative(true)
                                       .SetId(body)
                                       .SetEase(Ease.InOutSine))
                
                           .SetLoops(-1);
                });
    }

    public void AnimateSpawn()
    {
        var manipulator = Context.Context.Manipulator.CameraManipulator;
            
        if (manipulator.CameraMove.IsLocked == false)
        {
            float DURATION = 0.4f;
            manipulator.MoveTo(CachedTransform.position, true, DURATION, Ease.OutCubic);
        }
        
        body.localScale = Vector3.zero;

        DOTween.Sequence()
               .SetId(body)
               .Append(body.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack))
               .OnComplete(AnimateIdle);

        var position = GetCellForSyncLayers();
        var animView = Context.CreateBoardElementAt<AnimationView>(R.SpawnAirShipAnimation, position);
        
        animView.SyncRendererLayers(position);
        animView.transform.position = CachedTransform.position;
        animView.Play(null);
    }
    
    public void AnimateDeath()
    {
        StopAnimation();

        DOTween.Sequence()
            .SetId(body)
            .Append(body.DOScale(Vector3.zero, 0.6f).SetEase(Ease.InBack))
            .OnComplete(() => Context.DestroyElement(this));
        
        var position = GetCellForSyncLayers();
        var animView = Context.CreateBoardElementAt<AnimationView>(R.DestroyAirShipAnimation, position);
        
        animView.SyncRendererLayers(position);
        animView.transform.position = CachedTransform.position;
        animView.Play(null);
    }

    private void AnimateError()
    {
        StopAnimation();
        UIErrorWindowController.AddNoFreeSpaceError();
        
        DOTween.Sequence()
            .SetId(body)
            .Append(body.DOLocalMoveX(-0.05f, 0.06f))
            .Append(body.DOLocalMoveX(0.05f, 0.06f))
            .SetLoops(5)
            .OnComplete(() =>
            {
                isClick = false;
                AnimateIdle();
            });
    }
    
    public void StopAnimation()
    {
        DOTween.Kill(CachedTransform);
        DOTween.Kill(body); 
    }
    
    private void OnDestroy()
    {
        StopAnimation();
    }
}