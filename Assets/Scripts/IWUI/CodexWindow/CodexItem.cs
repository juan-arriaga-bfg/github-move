using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Vector2 maxSize;

    [IWUIBinding("#Caption")] private TextMeshProUGUI caption;
    [IWUIBinding("#(?)")]     private GameObject questionMark;
    [IWUIBinding("#Arrow")]   private GameObject arrow;
    [IWUIBinding("#Shine")]   private GameObject shine;
    [IWUIBinding("#Basket")]  private GameObject basket;
    [IWUIBinding("#Hand")]    private GameObject hand;
    [IWUIBinding("#Piece")]   private Image pieceImage;
    [IWUIBinding("#Gift")]    private GameObject gift;
    [IWUIBinding("#Gift")]    private Animator giftAnimator;

    private CodexItemState state;

    private CodexItemDef def;

    private bool forceHideArrow;

    public void ReloadWithState(CodexItemState state)
    {
        def.State = state;
        Setup(def, forceHideArrow);
    }
    
    public void Setup(CodexItemDef itemDef, bool forceHideArrow)
    {
        this.forceHideArrow = forceHideArrow;
        
        def = itemDef;
        state = itemDef.State;

        Reset();

        arrow .SetActive(def.ShowArrow && !this.forceHideArrow && !def.PieceTypeDef.Filter.Has(PieceTypeFilter.ProductionField));
        basket.SetActive(def.ShowArrow && !this.forceHideArrow &&  def.PieceTypeDef.Filter.Has(PieceTypeFilter.ProductionField));
        
        Sprite sprite = null;
        string captionText = GetCaption();

        switch (state)
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
        size.x = Mathf.Min(maxSize.x, size.x);
        size.y = Mathf.Min(maxSize.y, size.y);
        pieceImage.rectTransform.sizeDelta = size;
        
        // Debug.Log($"[CodexItem] => Init {itemDef.PieceTypeDef.Abbreviations[0]} as {state}");
    }

    private void ToggleViewFromPendingRewardToUnlockedState(bool animated)
    {
        pieceImage.gameObject.SetActive(true);
        shine.SetActive(false);
        hand.SetActive(def.PieceTypeDef.Filter.Has(PieceTypeFilter.Ingredient));
    }

    private void Reset()
    {
        StopGiftAnimation();

        questionMark.SetActive(false);
        shine.SetActive(false);

        pieceImage.gameObject.SetActive(true);
        pieceImage.material = unlokedMaterial;
        pieceImage.color = unlockedColor;

        pieceImage.transform.localScale = defaultScale;
        
        hand.SetActive(false);
        basket.SetActive(false);
    }

    private void StopGiftAnimation()
    {
        giftAnimator.ResetTrigger("Idle");
        giftAnimator.ResetTrigger("Open");
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
        giftAnimator.SetTrigger("Idle");
    }

    private void PlayGiftOpenAnimation(Action onComplete)
    {
        giftAnimator.SetTrigger("Open");

        if (onComplete == null)
        {
            return;
        }

        float animLen = giftAnimator.GetCurrentAnimatorClipInfo(0).Length;
        float blendTime = 0.2f;
        
        DOTween.Sequence()
               .InsertCallback(animLen - blendTime, ()=>
                {
                    onComplete();
                    ToggleViewFromPendingRewardToUnlockedState(true);
                    
                    StopGiftAnimation();
                });
    }

    public void OnClick()
    {
        switch (state)
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
        if (state != CodexItemState.PendingReward)
        {
            return;
        }

        GameDataService.Current.CodexManager.ClaimRewardForPiece(def.PieceTypeDef.Id);
        
        var reward = def.PendingReward;

        def.PendingReward = null;
        def.State = CodexItemState.Unlocked;

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
}