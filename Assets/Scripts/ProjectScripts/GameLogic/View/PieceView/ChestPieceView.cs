using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChestPieceView : PieceBoardElementView
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Material backlightMaterial;
    [SerializeField] private float backlightScale = 1.1f;

    [SerializeField] private Material hightlightMaterial;
    
    [SerializeField] private Transform cap;
    [SerializeField] private GameObject shine;
    
    [SerializeField] private float open;
    [SerializeField] private float close;

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
        
        var hint = Context.Context.GetComponent<HintCooldownComponent>(HintCooldownComponent.ComponentGuid);
        
        if(hint == null) return;
        
        hint.Step(HintType.CloseChest);
    }

    public override void ResetViewOnDestroy()
    {
        if (backlight != null)
        {
            Destroy(backlight);
            
        }
        if(hightlight != null)
            Destroy(hightlight);
            
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
        if(chestComponent == null || chestComponent.Chest == null) return;

        var isStorage = chestComponent.Chest.CheckStorage();
        SetBackLight(isStorage);
        SetHighlight(isStorage);
        
        var isOpen = chestComponent.Chest.State == ChestState.Open;
        
        if(shine != null) shine.SetActive(isOpen);
        
        if(cap != null) cap.localPosition = new Vector3(cap.localPosition.x, isOpen ? open : close);
    }

    public void SetBackLight(bool active = true)
    {
        if (backlight != null || CreateBackLight())
        {
            backlight.gameObject.SetActive(active);
            BacklightAnimation(active);
        }
    }
    
    private void SetHighlight(bool active = true)
    {
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

        var backlightLocal = Instantiate(sprite, sprite.transform);
        backlightLocal.transform.localPosition = Vector3.zero;
        backlightLocal.name = "_backlight";
        if(backlightMaterial != null)
            backlightLocal.material = backlightMaterial;

        backlightLocal.sortingOrder = 0;

        this.backlight = backlightLocal;
        return true;
    }

    private bool CreateHighlight()
    {
        if (sprite == null)
            return false;

        var highlightLocal = Instantiate(sprite, sprite.transform);
        highlightLocal.transform.localPosition = Vector3.zero;
        highlightLocal.name = "_highlight";
        if(hightlightMaterial != null)
            highlightLocal.material = hightlightMaterial;

        highlightLocal.sortingOrder = sprite.sortingOrder + 1;
        this.hightlight = highlightLocal;
        return true;
    }
}