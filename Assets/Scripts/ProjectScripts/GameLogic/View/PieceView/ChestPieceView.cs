using DG.Tweening;
using UnityEngine;

public class ChestPieceView : PieceBoardElementView
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Material backlightMaterial;
    [SerializeField] private float backlightScale = 1.1f;

    [SerializeField] private Material hightlightMaterial;
    
    private SpriteRenderer backlight;
    private SpriteRenderer hightlight;
    
    private ChestPieceComponent chestComponent;

    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        chestComponent = piece.GetComponent<ChestPieceComponent>(ChestPieceComponent.ComponentGuid);
        chestComponent.Timer.OnStop += UpdateView;
        chestComponent.Timer.OnComplete += UpdateView;
        
        UpdateView();
        Context.Context.HintCooldown.Step(HintType.CloseChest);
    }

    public override void ResetViewOnDestroy()
    {
        if (backlight != null)
        {
            RemoveLayerFromCache(backlight);
            Destroy(backlight.gameObject);
        }


        if (hightlight != null)
        {
            RemoveLayerFromCache(hightlight);
            Destroy(hightlight.gameObject);
        }
          
        chestComponent.Timer.OnStop -= UpdateView;
        chestComponent.Timer.OnComplete -= UpdateView;
        base.ResetViewOnDestroy();
    }

    public override void OnDragStart(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragStart(boardPos, worldPos);
        chestComponent.TimerViewChange(false);
    }

    public override void OnDragEnd(BoardPosition boardPos, Vector2 worldPos)
    {
        base.OnDragEnd(boardPos, worldPos);
        chestComponent.TimerViewChange(true);
    }
    
    public override void UpdateView()
    {   
        if(chestComponent?.Chest == null) return;
        
        var isStorage = chestComponent.Chest.CheckStorage();
        
        SetHighlight(isStorage);
        SetBackLight(isStorage);
    }

    public void SetBackLight(bool active = true)
    {
        if (backlight == null && !active)
            return;
        
        if (backlight != null || CreateBackLight())
        {
            backlight.gameObject.SetActive(active);
            BacklightAnimation(active);
        }
    }
    
    private void SetHighlight(bool active = true)
    {
        if (backlight == null && !active)
            return;
        
        if (hightlight != null || CreateHighlight())
        {
            hightlight.gameObject.SetActive(active);
            HightlightAnimation(active);
        }
    }

    private const float duration = 0.5f;

    private void HightlightAnimation(bool active)
    {
        DOTween.Kill(hightlight);
        if (!active)
            return;
        
        var color = Color.red;
        
        color.a = 0;
        hightlight.color = color;

        hightlight.DOColor(new Color(color.r, color.b, color.g, 0.4f), duration).SetLoops(-1, LoopType.Yoyo).SetDelay(duration).SetId(hightlight);
    }
    
    private void BacklightAnimation(bool active)
    {
        DOTween.Kill(backlight);
        if (!active)
            return;
        
        var color = Color.red;
        
        color.a = 0.8f;
        backlight.color = color;

        backlight.DOColor(new Color(color.r, color.b, color.g, 0.2f), duration).SetLoops(-1, LoopType.Yoyo).SetId(backlight);
        
        backlight.gameObject.transform.DOScale(
            new Vector3(backlightScale, backlightScale, backlight.transform.localScale.z),
            0.6f);
    }
    
    private bool CreateBackLight()
    {
        if (sprite == null)
            return false;
        
        var backlightObject = Instantiate(sprite.gameObject, transform);
        backlightObject.transform.position = sprite.gameObject.transform.position;
        var backlightLocal = backlightObject.GetComponent<SpriteRenderer>();
        
        backlightLocal.name = "_backlight";
        if(backlightMaterial != null)
            backlightLocal.material = backlightMaterial;

        backlightLocal.sortingOrder = sprite.sortingOrder - 1;

        this.backlight = backlightLocal; 
        
        AddLayerToCache(backlightLocal);
        var baclightRendererLayer = backlightObject.GetComponent<RendererLayer>();
        baclightRendererLayer.SortingOrderOffset = 0;
        
        return true;
    }

    private bool CreateHighlight()
    {
        if (sprite == null)
            return false;
        
        var highlightObject = Instantiate(sprite.gameObject, transform);
        highlightObject.transform.position = sprite.gameObject.transform.position;
        var highlightLocal = highlightObject.GetComponent<SpriteRenderer>();
        highlightLocal.name = "_highlight";
        if(hightlightMaterial != null)
            highlightLocal.material = hightlightMaterial;

        highlightLocal.sortingOrder = sprite.sortingOrder + 1;
        
        this.hightlight = highlightLocal;

        AddLayerToCache(highlightLocal);
        
        var highlightRendererLayer = highlightObject.GetComponent<RendererLayer>();
        highlightRendererLayer.SortingOrderOffset = 2;
        
        return true;
    }
}