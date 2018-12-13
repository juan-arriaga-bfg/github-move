using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.Rendering;

public class ViewAnimationUid { }

public class BoardElementView : IWBaseMonoBehaviour, IFastPoolItem
{
    [SerializeField] protected BoardPosition lastBoardPosition;

    private int cachedIdleAnimatorHash = Animator.StringToHash("Idle");

    private Animator animator;

    protected ViewAnimationUid animationUid = new ViewAnimationUid();

    protected BetterList<RendererLayer> cachedRenderers = new BetterList<RendererLayer>();

    [SerializeField] protected SortingGroup cachedSortingGroup;

    protected OverrideSortingGroup[] cachedOverrideSortingGroup;

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

    protected void AddToLayerCache(GameObject target)
    {
        var renderers = target.GetComponentsInChildren<Renderer>();
		
        foreach (var items in renderers)
        {
            AddLayerToCache(items);
        }
    }

    protected void RemoveFromLayerCache(GameObject target)
    {
        var renderers = target.GetComponentsInChildren<Renderer>();
		
        foreach (var items in renderers)
        {
            RemoveLayerFromCache(items);
            var rendererLayer = items.gameObject.GetComponent<RendererLayer>();
            items.sortingOrder = rendererLayer.SortingOrderOffset;
        }
    }
    
    public virtual void ClearCurrentMaterialAsDefault()
    {
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        foreach (var rend in cachedRenderers)
        {
            if (rend == null) continue;
            if (rend.CachedRenderer == null) continue;
            if (rend.CachedRenderer.sharedMaterial == null) continue;

            rend.SetLastDefaultMaterial(null);
        }
    }

    public virtual void SaveCurrentMaterialAsDefault()
    {
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        foreach (var rend in cachedRenderers)
        {
            if (rend == null) continue;
            if (rend.CachedRenderer == null) continue;
            if (rend.CachedRenderer.sharedMaterial == null) continue;

            rend.SetLastDefaultMaterial(rend.CachedRenderer.sharedMaterial);
        }
    }
    
    public virtual void SetFade(float alpha)
    {
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        foreach (var rend in cachedRenderers)
        {
            if (rend == null) continue;
            if (rend.CachedRenderer == null) continue;
            if (rend.CachedRenderer.sharedMaterial == null) continue;

            var material = rend.MaterialCopy;//rend.SetCustomMaterial(Context.MaterialsCache.GetMaterial(BoardElementMaterialType.PiecesFadeMaterial));
            if (material.HasProperty("_AlphaCoef") == false)
            {
                material = rend.SetCustomMaterial(Context.MaterialsCache.GetMaterial(BoardElementMaterialType.PiecesDefaultMaterial));
            }
            
            material.SetFloat("_AlphaCoef", alpha);
            
            if (alpha >= 1f)
            {
                rend.ResetDefaultMaterial();
            }
        }
    }
    
    public virtual void SetGrayscale()
    {
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        foreach (var rend in cachedRenderers)
        {
            if (rend == null) continue;
            if (rend.CachedRenderer == null) continue;
            if (rend.CachedRenderer.sharedMaterial == null) continue;

            rend.SetCustomMaterial(Context.MaterialsCache.GetMaterial(BoardElementMaterialType.PiecesGrayscaleMaterial));
        }
    }
    
    public virtual Material GetExistingCustomMaterial(string customMaterialId, object groupTag)
    {
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        var customMaterial = Context.MaterialsCache.GetMaterialForGroup(customMaterialId, groupTag, false);

        return customMaterial;
    }
    
    
    public virtual Material SetCustomMaterial(string customMaterialId, bool state, object groupTag)
    {
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        var customMaterial = Context.MaterialsCache.GetMaterialForGroup(customMaterialId, groupTag);

        foreach (var rend in cachedRenderers)
        {
            if (rend == null) continue;
            if (rend.CachedRenderer == null) continue;
            if (rend.CachedRenderer.sharedMaterial == null) continue;

            if (state)
            {
                rend.SetCustomMaterial(customMaterial);
            }
            else
            {
                rend.ResetDefaultMaterial();
            }
        }

        return customMaterial;
    }
    
    public virtual Material SetCustomMaterial(string customMaterialId, bool state)
    {
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        var customMaterial = Context.MaterialsCache.GetMaterial(customMaterialId);

        foreach (var rend in cachedRenderers)
        {
            if (rend == null) continue;
            if (rend.CachedRenderer == null) continue;
            if (rend.CachedRenderer.sharedMaterial == null) continue;

            if (state)
            {
                rend.SetCustomMaterial(customMaterial);
            }
            else
            {
                rend.ResetDefaultMaterial();
            }
        }

        return customMaterial;
    }
    
    public virtual void SetHighlight(bool state, List<GameObject> ignoredObjects = null)
    {
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        foreach (var rend in cachedRenderers)
        {
            if (rend == null) continue;
            if (rend.CachedRenderer == null) continue;
            if (rend.CachedRenderer.sharedMaterial == null) continue;
            if (ignoredObjects != null && ignoredObjects.Contains(rend.gameObject)) continue;

            if (state)
            {
                rend.SetCustomMaterial(Context.MaterialsCache.GetMaterial(BoardElementMaterialType.PiecesHighlightMaterial));
            }
            else
            {
                rend.ResetDefaultMaterial();
            }
        }
    }
    
    public virtual void ResetDefaultMaterial()
    {
        if (cachedRenderers == null || cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        foreach (var rend in cachedRenderers)
        {
            if (rend == null) continue;
            if (rend.CachedRenderer == null) continue;
            if (rend.CachedRenderer.sharedMaterial == null) continue;

            rend.ResetDefaultMaterial();
        }
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

        cachedOverrideSortingGroup = null;
        
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
        
        if (cachedOverrideSortingGroup == null)
        {
            cachedOverrideSortingGroup = GetComponentsInChildren<OverrideSortingGroup>(true);
        }
    }

    public virtual void CacheLayers()
    {
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

        if (cachedOverrideSortingGroup == null)
        {
            cachedOverrideSortingGroup = GetComponentsInChildren<OverrideSortingGroup>(true);
        }
    }
    
    public virtual int GetLayerIndexBy(BoardPosition boardPosition)
    {
        var layer = BoardLayer.GetDefaultLayerIndexBy(boardPosition, Context.Context.BoardDef.Width, Context.Context.BoardDef.Height);
        
        return layer;
    }

    public virtual void SyncRendererLayers(BoardPosition boardPosition)
    {
        this.lastBoardPosition = boardPosition;

        if (BoardLayer.IsValidLayer(boardPosition.Z)){}

        if (cachedRenderers.size <= 0)
        {
            CacheLayers();
        }

        if (cachedSortingGroup == null)
        {
            cachedSortingGroup = GetComponent<SortingGroup>();
            if (cachedSortingGroup == null)
            {
                cachedSortingGroup = gameObject.AddComponent<SortingGroup>();
            }
        }

        cachedSortingGroup.sortingOrder = GetLayerIndexBy(boardPosition);

        CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y, -boardPosition.Z * 0.1f);

        if (cachedOverrideSortingGroup != null)
        {
            for (int i = 0; i < cachedOverrideSortingGroup.Length; i++)
            {
                var cachedOverrideSorting = cachedOverrideSortingGroup[i];
                cachedOverrideSorting.SyncRendererLayers(this, boardPosition);
            }
        }
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

        ClearCurrentMaterialAsDefault();
        ResetDefaultMaterial();
        
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

    private Material cachedDefaultMaterial;
    
    private Material cachedLastDefaultMaterial;

    public void CacheDefaultMaterial()
    {
        if (cachedDefaultMaterial == null)
        {
            cachedDefaultMaterial = CachedRenderer.sharedMaterial;
        }
    }

    public void ResetDefaultMaterial()
    {
        if (cachedLastDefaultMaterial != null)
        {
            CachedRenderer.material = cachedLastDefaultMaterial;
            return;
        }
        
        if (cachedDefaultMaterial != null)
        {
            CachedRenderer.material = cachedDefaultMaterial;
        }
    }
    public void SetDefaultMaterial(Material defaultMaterial)
    {
        this.cachedDefaultMaterial = defaultMaterial;
    }

    public void SetLastDefaultMaterial(Material lastDefaultMaterial)
    {
        this.cachedLastDefaultMaterial = lastDefaultMaterial;
    }
    
    public Material SetCustomMaterial(Material customMaterial)
    {
        CacheDefaultMaterial();
        CachedRenderer.material = customMaterial;

        return customMaterial;
    }

    public Material MaterialCopy
    {
        get
        {
            CacheDefaultMaterial();
            return CachedRenderer.material;
        }
    }

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
}
