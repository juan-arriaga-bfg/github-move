using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class ViewAnimationUid { }

public class BoardElementView : IWBaseMonoBehaviour, IFastPoolItem
{
    private int cachedIdleAnimatorHash = Animator.StringToHash("Idle");

    private Animator animator;

    protected ViewAnimationUid animationUid = new ViewAnimationUid();

    private BetterList<RendererLayer> cachedRenderers = new BetterList<RendererLayer>();

    public bool IsFading;

    public float FadeValue;

    public BoardRenderer Context { get; set; }

    public ViewAnimationUid AnimationUid { get { return animationUid; } }

    public Animator Animator
    {
        get
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }

    public virtual void Init(BoardRenderer context)
    {
        Context = context;
    }

    protected void AddLayerToCache(Renderer rend)
    {
        var rendererLayerParams = rend.GetComponent<RendererLayerParams>();
        if (rendererLayerParams != null && rendererLayerParams.IsIgnoreRenderLayer) return;

        var rendererLayer = rend.gameObject.GetComponent<RendererLayer>();

        if (rendererLayer == null)
        {
            rendererLayer = rend.gameObject.AddComponent<RendererLayer>();
        }
            
        rendererLayer.SortingOrderOffset = rend.sortingOrder;

        cachedRenderers.Add(rendererLayer);
    }

    protected void RemoveLayerFromCache(Renderer rend)
    {
        var rendererLayerParams = rend.GetComponent<RendererLayerParams>();
        if (rendererLayerParams != null && rendererLayerParams.IsIgnoreRenderLayer) return;

        var rendererLayer = rend.gameObject.GetComponent<RendererLayer>();

        if (rendererLayer == null)
        {
            rendererLayer = rend.gameObject.AddComponent<RendererLayer>();
        }
        
        cachedRenderers.Remove(rendererLayer);
    }

    protected virtual void Start()
    {
        if (cachedRenderers.size <= 0)
        {
            CacheLayers();
        }
    }

    public virtual void ClearCacheLayers()
    {
        cachedRenderers.Clear();
        
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var rend in renderers)
        {
            var rendererLayerParams = rend.GetComponent<RendererLayerParams>();
            if (rendererLayerParams != null && rendererLayerParams.IsIgnoreRenderLayer) continue;

            var rendererLayer = rend.gameObject.GetComponent<RendererLayer>();

            if (rendererLayer == null)
            {
                rendererLayer = rend.gameObject.AddComponent<RendererLayer>();
            }
            
            rendererLayer.SortingOrderOffset = rend.sortingOrder;

            cachedRenderers.Add(rendererLayer);
        }
    }

    public virtual void CacheLayers()
    {
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var rend in renderers)
        {
            var rendererLayerParams = rend.GetComponent<RendererLayerParams>();
            if (rendererLayerParams != null && rendererLayerParams.IsIgnoreRenderLayer) continue;

            var rendererLayer = rend.gameObject.AddComponent<RendererLayer>();
            rendererLayer.SortingOrderOffset = rend.sortingOrder;

            cachedRenderers.Add(rendererLayer);
        }
    }

    public virtual void SyncRendererLayers(BoardPosition boardPosition)
    {
        if (cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        for (int i =0; i < cachedRenderers.size; i++)
        {
            var rend = cachedRenderers[i];
   
            rend.CachedRenderer.sortingOrder = boardPosition.X * Context.Context.BoardDef.Width - boardPosition.Y + boardPosition.Z * 100 + rend.SortingOrderOffset;
        }

        CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y, -boardPosition.Z * 0.1f);
    }

    public virtual void OnFastInstantiate()
    {
        ResetView();
    }

    public virtual void OnFastDestroy()
    {
        CachedTransform.SetParent(null);
        
        ResetViewOnDestroy();
    }

    private Vector3 targetPosition;

    private Vector3 acceleratedPosition;

    public const float DefaultSpeed = 600f;

    public const float DefaultAcceleration = 1500f;

    private float speed = DefaultSpeed;

    private float startSpeed = DefaultSpeed;

    private float acceleration = DefaultAcceleration;

    private float delta = 0.0001f;

    private bool isProcessMove = false;

    private Vector3 velocity;

    private BoardPosition boardPoint;

    private BoardPosition targetDirection;

    public virtual void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        this.acceleratedPosition = targetPosition;
        this.isProcessMove = true;
    }

    public virtual void MoveToWithAcceleration(BoardPosition boardPoint, BoardPosition targetDirection, Vector3 targetPosition, Vector3 acceleratedPosition, float startSpeed = DefaultSpeed, float acceleration = DefaultAcceleration)
    {
        this.startSpeed = startSpeed;
        this.boardPoint = boardPoint;
        this.targetDirection = targetDirection;

        SetTargetPosition(targetPosition);
        SetSpeed(startSpeed);

        this.acceleratedPosition = acceleratedPosition;
        this.acceleration = acceleration;
        this.fallingTime = 0f;

        DOTween.Kill(AnimationUid, true);
    }

    public virtual void SetAcceleration(float acceleration)
    {
        this.acceleration = acceleration;
    }

    public virtual void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public float Speed
    {
        get { return speed; }
    }

    public Vector3 Velocity
    {
        get { return velocity; }
    }

    public virtual bool IsReachedPosition
    {
        get { return isProcessMove == false; }
    }

    public virtual void StopMove()
    {
        isProcessMove = false;
    }


    private float fallingTime = 0f;

    public virtual float FallingTime
    {
        get { return fallingTime; }
    }

    public virtual void Update()
    {
        if (isProcessMove == false) return;

        float distance = (CachedTransform.localPosition - targetPosition).sqrMagnitude;
        if (distance > delta)
        {
            var prevPosition = CachedTransform.localPosition;
            CachedTransform.localPosition = Vector3.MoveTowards(CachedTransform.localPosition, targetPosition, Time.deltaTime * speed);

            if (CachedTransform.localPosition.y < acceleratedPosition.y)
            {
                fallingTime += Time.deltaTime;
                speed = speed + acceleration * Time.deltaTime;

                // fade in
                if (IsFading == false && Mathf.Abs(FadeValue - 1f) > 0.01f)
                {
                    FadeAlpha(1f, 0.3f, (_) => { });
                }
            }


            distance = (CachedTransform.localPosition - targetPosition).sqrMagnitude;
            if (distance <= delta)
            {
                isProcessMove = false;
                CachedTransform.localPosition = targetPosition;
                SetSpeed(DefaultSpeed);
            }
            else
            {
                var nextPiecePoint = new BoardPosition
                (
                    boardPoint.X + targetDirection.X,
                    boardPoint.Y + targetDirection.Y,
                    boardPoint.Z + targetDirection.Z
                );

                // clamp speed to prevent intersections
                var nextPieceView = Context.GetElementAt(nextPiecePoint);
                if (nextPieceView != null)
                {
                    if ((nextPieceView.CachedTransform.localPosition - CachedTransform.localPosition).sqrMagnitude < Context.Context.BoardDef.CellHeightInUnit() * Context.Context.BoardDef.CellHeightInUnit() * 0.9f)
                    {
                        CachedTransform.localPosition = prevPosition;
                        this.speed = nextPieceView.Speed * 0.9f;
                    }
                }
            }
        }
    }

    public virtual void ResetViewOnDestroy()
    {
        this.isProcessMove = false;
        this.targetPosition = CachedTransform.localPosition;

        DOTween.Kill(animationUid);

        if (Animator != null && Animator.HasState(0, cachedIdleAnimatorHash))
        {
            Animator.CrossFade(cachedIdleAnimatorHash, 0f);
            Animator.Update(0f);
        }
    }

    public virtual void ResetView()
    {
        this.isProcessMove = false;
        this.targetPosition = CachedTransform.localPosition;

        CachedTransform.localPosition = Vector3.zero;
        CachedTransform.localScale = Vector3.one;
        CachedTransform.localRotation = Quaternion.identity;
    }


    public virtual void FadeAlpha(float alpha, float time, Action<BoardElementView> onComplete)
    {
        if (onComplete != null)
        {
            onComplete(this);
        }
    }

    public virtual void SetColor(Color color)
    {

    }

    public virtual BoardElementView DestroyOnBoard(float delay = -1f)
    {
        if (delay > 0f)
        {
            DOTween.Sequence()
                .SetId(animationUid)
                .AppendInterval(delay)
                .OnComplete(() =>
               {
                   if (Context != null)
                   {
                       Context.DestroyElement(this);
                   }
               });
        }
        else
        {
            if (Context != null)
            {
                Context.DestroyElement(this);
            }
        }

        return this;
    }
}

[RequireComponent(typeof(Renderer))]
public class RendererLayer : MonoBehaviour
{
    [SerializeField] private int sortingOrderOffset;

    private Renderer cachedRenderer;

    public int SortingOrderOffset
    {
        get
        {
            return sortingOrderOffset;
        }
        set
        {
            sortingOrderOffset = value;
        }
    }

    public Renderer CachedRenderer
    {
        get
        {
            if (cachedRenderer == null)
            {
                cachedRenderer = GetComponent<Renderer>();
            }
            return cachedRenderer;
        }
    }
    // public virtual void SetLayerAndOrder(int layerId, int order)
    // {
    //     CachedRenderer.sortingOrder = order * 100 + sortingOrderOffset + layerId * 10000;
    // }


}
