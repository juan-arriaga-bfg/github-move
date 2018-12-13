using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CodexItem : IWUIWindowViewController
{
    [Header("Materials")] 
    [SerializeField] private Material unlokedMaterial;
    [SerializeField] private Material lockedMaterial;
    [SerializeField] private Color unlockedColor;
    [SerializeField] private Color lockedColor;
    
    [Header("Image params")]
    [SerializeField] private Vector3 defaultScale;
    [SerializeField] private Vector3 rewardScale;
    
    public const float MIN_ITEM_IMAGE_SIZE = 80;
    public const float MAX_ITEM_IMAGE_SIZE = 155;

    [IWUIBinding("#Caption")] private TextMeshProUGUI caption;
    [IWUIBinding("#(?)")]     private GameObject questionMark;
    [IWUIBinding("#Arrow")]   private GameObject arrow;
    [IWUIBinding("#Shine")]   private GameObject shine;
    [IWUIBinding("#Basket")]  private GameObject basket;
    [IWUIBinding("#Hand")]    private GameObject hand;
    [IWUIBinding("#Piece")]   private Image pieceImage;
    [IWUIBinding("#Piece")]   private RectTransform pieceImageRectTransform;
    [IWUIBinding("#Gift")]    private GameObject gift;
    [IWUIBinding("#Gift")]    private Animator giftAnimator;

    // private CodexItemState state;

    private readonly Color COLOR_TRANSPARENT = new Color(1, 1, 1, 0);
    
    private CodexItemDef def;

    private bool forceHideArrow;

    public Image PieceImage => pieceImage;

    public RectTransform PieceImageRectTransform => pieceImageRectTransform;

    public CodexItemDef Def => def;

    public void ReloadWithState(CodexItemState state)
    {
        def.State = state;
        Setup(def, forceHideArrow);
    }
    
    public void Setup(CodexItemDef itemDef, bool forceHideArrow)
    {
        this.forceHideArrow = forceHideArrow;
        
        def = itemDef;

        Reset();

        arrow .SetActive(def.ShowArrow && !this.forceHideArrow && !def.PieceTypeDef.Filter.Has(PieceTypeFilter.ProductionField));
        basket.SetActive(def.ShowArrow && !this.forceHideArrow &&  def.PieceTypeDef.Filter.Has(PieceTypeFilter.ProductionField));
        
        Sprite sprite = null;
        string captionText = GetCaption();

        switch (def.State)
        {
            case CodexItemState.FullLock:
                questionMark.SetActive(true);
                pieceImage.gameObject.SetActive(false);
                               
                break;
            
            case CodexItemState.PartLock:
                questionMark.SetActive(true);
                sprite = GetPieceSprite();
                
                pieceImage.material = lockedMaterial;
                pieceImage.color = lockedColor;
                
                break;
            
            case CodexItemState.PendingReward:
                sprite = GetPieceSprite();
                shine.SetActive(true);
                pieceImage.gameObject.SetActive(false);
                PlayGiftIdleAnimation();
                
                break;
            
            case CodexItemState.Unlocked:
                sprite = GetPieceSprite();
                pieceImage.gameObject.SetActive(true);
                shine.SetActive(false);
                hand.SetActive(def.PieceTypeDef.Filter.Has(PieceTypeFilter.Ingredient));

                break;
            
            case CodexItemState.Highlighted:
                sprite = GetPieceSprite();
                shine.SetActive(true);

                pieceImage.transform.localScale = rewardScale;
                
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        pieceImage.sprite = sprite;
        caption.text = captionText;

        SyncPivotAndSizeOfPieceImage();

        Vector2 size = pieceImage.rectTransform.sizeDelta;
        size.x = Mathf.Min(MIN_ITEM_IMAGE_SIZE, size.x);
        size.y = Mathf.Min(MIN_ITEM_IMAGE_SIZE, size.y);
        pieceImage.rectTransform.sizeDelta = size;
        
        // Debug.Log($"[CodexItem] => Init {itemDef.PieceTypeDef.Abbreviations[0]} as {state}");
    }

    private void Reset()
    {
        StopGiftAnimation();

        questionMark.SetActive(false);
        shine.SetActive(false);

        DOTween.Kill(pieceImage);
        pieceImage.gameObject.SetActive(true);
        pieceImage.material = unlokedMaterial;
        pieceImage.color = unlockedColor;

        pieceImage.transform.localScale = defaultScale;
        
        hand.SetActive(false);
        basket.SetActive(false);
    }

    private void StopGiftAnimation()
    {
        if (giftAnimator.isActiveAndEnabled && giftAnimator.runtimeAnimatorController != null)
        {
            giftAnimator.ResetTrigger("Open");
        }

        gift.SetActive(false);
    }

    private Sprite GetPieceSprite()
    {
        return IconService.Current.GetSpriteById(PieceType.Parse(def.PieceTypeDef.Id));
    }
    
    private string GetCaption()
    {
        return def?.PieceDef?.Name;
    }

    private void PlayGiftIdleAnimation()
    {
        gift.SetActive(true);
        giftAnimator.Play("CodexGiftIdle", -1, Random.Range(0f,1f));
    }

    private void PlayGiftOpenAnimation(Action onComplete)
    {
        giftAnimator.SetTrigger("Open");

        if (onComplete == null)
        {
            return;
        }

        pieceImage.gameObject.SetActive(true);
        pieceImage.color = COLOR_TRANSPARENT;
        pieceImage.transform.localScale = Vector3.one * 0.15f;
        
        float animLen = giftAnimator.GetCurrentAnimatorClipInfo(0).Length;
        float blendTime = 0.6f;
        float tweenStartTime = Mathf.Max(0, animLen - blendTime);
        float tweenTime = Mathf.Max(0, animLen - tweenStartTime);
        
        DOTween.Sequence()
               .Insert(tweenStartTime, pieceImage.DOColor(unlockedColor, tweenTime).SetId(pieceImage))
               .Insert(tweenStartTime, pieceImage.transform.DOScale(defaultScale, tweenTime - 0.15f).SetEase(Ease.InOutBack) .SetId(pieceImage))
               .InsertCallback(tweenStartTime, ()=>
                {
                    onComplete();
                    
                    shine.SetActive(false);
                    hand.SetActive(def.PieceTypeDef.Filter.Has(PieceTypeFilter.Ingredient));
                    
                    StopGiftAnimation();
                });
    }

    public void OnClick()
    {
        switch (def.State)
        {
            case CodexItemState.FullLock:
                break;
            
            case CodexItemState.PartLock:
                break;
            
            case CodexItemState.PendingReward:
                ClaimReward();
                break;
            
            case CodexItemState.Unlocked:
                break;
            
            case CodexItemState.Highlighted:
                break;
        }
    }
    
    private void ClaimReward()
    {
        if (def.State != CodexItemState.PendingReward)
        {
            return;
        }

        var reward = new List<CurrencyPair> {new CurrencyPair {Amount = 15, Currency = "Coins"}};//def.PendingReward;

        def.PendingReward = null;
        def.State = CodexItemState.Unlocked;
        
        GameDataService.Current.CodexManager.ClaimRewardForPiece(def.PieceTypeDef.Id);

        PlayGiftOpenAnimation(() =>
        {
            if (reward == null || reward.Count == 0)
            {
                Debug.LogError($"[CodexItem] => ClaimReward: No unlock bonus specified for [{def.PieceDef.Id}] {def.PieceTypeDef.Abbreviations[0]}");
                return;
            }

            // todo: use something like: targetEntity.WindowController.Window.Layers[0].ViewCamera.WorldToScreenPoint(taskIcon.transform.position)
            var flyPosition = GetComponentInParent<Canvas>().worldCamera.WorldToScreenPoint(pieceImage.transform.position);
            CurrencyHellper.Purchase(reward, null, flyPosition);
        });
    }
    
    private void SyncPivotAndSizeOfPieceImage()
    {
        if (pieceImage.sprite == null)
        {
            return;
        }
        
        pieceImage.SetNativeSize();
        
        // Sync sprite and rect transform pivots
        RectTransform rectTransform = pieceImage.GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta;
        size *= pieceImage.pixelsPerUnit;
        Vector2 pixelPivot = pieceImage.sprite.pivot;
        Vector2 percentPivot = new Vector2(pixelPivot.x / size.x, pixelPivot.y / size.y);
        rectTransform.pivot = percentPivot;
    }

    private void OnEnable()
    {
        if (def == null)
        {
            return;
        }
        
        ReloadWithState(def.State);
    }
}