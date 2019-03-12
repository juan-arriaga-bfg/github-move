using System;
using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CodexItem : IWUIWindowViewController
{
    [Header("Materials")] 
    [SerializeField] protected Material unlokedMaterial;
    [SerializeField] protected Material lockedMaterial;
    [SerializeField] protected Color unlockedColor;
    [SerializeField] protected Color lockedColor;
    
    [Header("Image params")]
    [SerializeField] private Vector3 defaultScale;
    [SerializeField] private Vector3 rewardScale;
    
    public const float MIN_ITEM_IMAGE_SIZE = 90;
    public const float MAX_ITEM_IMAGE_SIZE = 155;

    [IWUIBindingNullable("#Caption")] protected TextMeshProUGUI caption;
    [IWUIBinding("#(?)")]     protected GameObject questionMark;
    [IWUIBindingNullable("#Arrow")]   protected GameObject arrow;
    [IWUIBinding("#Shine")]   protected GameObject shine;
    [IWUIBindingNullable("#Basket")]  protected GameObject basket;
    [IWUIBindingNullable("#Hand")]    protected GameObject hand;
    [IWUIBinding("#Piece")]   protected RectTransform pieceImageRectTransform;
    [IWUIBinding("#Gift")]    protected GameObject gift;
    [IWUIBinding("#Gift")]    protected Animator giftAnimator;

    public CodexChain Context;
    
    // private CodexItemState state;

    private readonly Color COLOR_TRANSPARENT = new Color(1, 1, 1, 0);
    
    protected CodexItemDef def;
    
    private bool forceHideArrow;
    
    public RectTransform PieceImageRectTransform => pieceImageRectTransform;

    public CodexItemDef Def => def;

    private Transform icon;
    private List<Image> IconSprites = new List<Image>();

    public void ReloadWithState(CodexItemState state)
    {
        def.State = state;
        Setup(def, forceHideArrow);
    }
    
    public virtual void Setup(CodexItemDef itemDef, bool forceHideArrow)
    {
        this.forceHideArrow = forceHideArrow;
        
        def = itemDef;

        Reset();

        if(arrow != null) arrow.SetActive(def.ShowArrow && !this.forceHideArrow && !def.PieceTypeDef.Filter.Has(PieceTypeFilter.ProductionField));
        if(basket != null) basket.SetActive(def.ShowArrow && !this.forceHideArrow &&  def.PieceTypeDef.Filter.Has(PieceTypeFilter.ProductionField));
        
        string captionText = GetCaption();

        switch (def.State)
        {
            case CodexItemState.FullLock:
                SetStateFullLock();

                break;
            
            case CodexItemState.PartLock:
                SetStatePartLock();

                break;
            
            case CodexItemState.PendingReward:
                SetStatePendingReward();

                break;
            
            case CodexItemState.Unlocked:
                SetStateUnlocked();

                break;
            
            case CodexItemState.Highlighted:
                SetStateHighlighted();

                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (caption != null) caption.text = def.HideCaption ? "" : captionText;

        // SyncPivotAndSizeOfPieceImage();
        //
        // Vector2 size = pieceImage.rectTransform.sizeDelta;
        // size.x = Mathf.Min(MIN_ITEM_IMAGE_SIZE, size.x);
        // size.y = Mathf.Min(MIN_ITEM_IMAGE_SIZE, size.y);
        // pieceImage.rectTransform.sizeDelta = size;

        // Debug.Log($"[CodexItem] => Init {itemDef.PieceTypeDef.Abbreviations[0]} as {def.State}, arrow: {def.ShowArrow}");
    }

    protected virtual void SetStateHighlighted()
    {
        CreateIcon(true);
        shine.SetActive(true);

        icon.transform.localScale = rewardScale;
    }

    protected virtual void SetStateUnlocked()
    {
        CreateIcon(true);
        shine.SetActive(false);
        if (hand != null) hand.SetActive(def.PieceTypeDef.Filter.Has(PieceTypeFilter.Ingredient));
    }

    protected virtual void SetStatePendingReward()
    {
        CreateIcon(false);
        shine.SetActive(true);
        PlayGiftIdleAnimation();
    }

    protected virtual void SetStatePartLock()
    {
        questionMark.SetActive(false);
        CreateIcon(true);

        foreach (var sprite in IconSprites)
        {
            sprite.material = lockedMaterial;
            sprite.color = lockedColor;
        }
    }

    protected virtual void SetStateFullLock()
    {
        questionMark.SetActive(true);
        CreateIcon(true);

        foreach (var sprite in IconSprites)
        {
            sprite.material = lockedMaterial;
            sprite.color = lockedColor;
        }
    }

    private void Reset()
    {
        StopGiftAnimation();

        questionMark.SetActive(false);
        shine.SetActive(false);

        DOTween.Kill(icon);
        
        if (hand != null) hand.SetActive(false);
        if (basket != null) basket.SetActive(false);
    }

    private void StopGiftAnimation()
    {
        if (giftAnimator.isActiveAndEnabled && giftAnimator.runtimeAnimatorController != null)
        {
            giftAnimator.ResetTrigger("Open");
        }

        gift.SetActive(false);
    }

    private void CreateIcon(bool isShow)
    {
        if (icon != null)
        {
            foreach (var sprite in IconSprites)
            {
                sprite.material = unlokedMaterial;
                sprite.color = unlockedColor;
            }
            
            UIService.Get.PoolContainer.Return(icon.gameObject);
            IconSprites = new List<Image>();
            icon = null;
        }
        
        if (isShow == false) return;

        var id = $"{PieceType.Parse(def.PieceTypeDef.Id)}{(Context.Context != null && Context.Context.IsHero ? "Icon" : "")}";
        
        icon = UIService.Get.PoolContainer.Create<Transform>((GameObject) ContentService.Current.GetObjectByName(id));
        icon.SetParentAndReset(pieceImageRectTransform);
        icon.transform.localScale = defaultScale;
        
        IconSprites = icon.GetComponentsInChildren<Image>().ToList();
    }
    
    private string GetCaption()
    {
        if (def?.PieceDef == null)
            return "";

        if (def.PieceDef.SpawnResources != null)
        {
            var resource = def.PieceDef.SpawnResources;
            return def.PieceDef.SpawnResources.ToStringIcon(false, 24);
        }
        return def.PieceDef?.Name;
    }
    
    public void SetCaption(string text)
    {
        if (caption != null) caption.text = text;
    }

    private void PlayGiftIdleAnimation()
    {
        gift.SetActive(true);
        giftAnimator.Play("CodexGiftIdle", -1, Random.Range(0f,1f));
    }

    private void PlayGiftOpenAnimation(Action onComplete)
    {
        giftAnimator.SetTrigger("Open");
        // giftAnimator.Update(0);

        if (onComplete == null)
        {
            return;
        }

        CreateIcon(true);
        
        foreach (var sprite in IconSprites)
        {
            sprite.color = COLOR_TRANSPARENT;
            sprite.material = unlokedMaterial;
        }
        
        icon.transform.localScale = Vector3.one * 0.15f;

        var clips = giftAnimator.runtimeAnimatorController.animationClips.ToList();
        var clip = clips.FirstOrDefault(e => e.name == "CodexGiftOpen");

        float animLen = clip != null ? clip.averageDuration : 1;
        float blendTime = 1.2f;
        float tweenStartTime = Mathf.Max(0, animLen - blendTime);
        float tweenTime = 0.4f;

        Vector3 shineScale = shine.transform.localScale;

        DOTween.Sequence()
            .SetId(icon)
            .Insert(tweenStartTime, shine.transform.DOScale(Vector3.zero, tweenTime).SetEase(Ease.InOutBack).SetId(icon))
            .InsertCallback(tweenStartTime, () =>
            {
                foreach (var sprite in IconSprites)
                {
                    sprite.DOColor(unlockedColor, tweenTime).SetId(icon);
                }
            })
            .Insert(tweenStartTime, icon.transform.DOScale(defaultScale, tweenTime).SetEase(Ease.InOutBack).SetId(icon))
            .InsertCallback(tweenTime, () =>
            {
                shine.transform.localScale = shineScale;
                shine.SetActive(false);
                if (hand != null) hand.SetActive(def.PieceTypeDef.Filter.Has(PieceTypeFilter.Ingredient));
            })
            .InsertCallback(tweenTime /*+ 0.2f*/, () => // Coins flight
            {
                onComplete();
            })
            .InsertCallback(animLen, StopGiftAnimation);
    }

    public virtual void OnClick()
    {        
        if (def.State == CodexItemState.PendingReward)
        {
            ClaimReward();
        }
    }
    
    private void ClaimReward()
    {
        if (def.State != CodexItemState.PendingReward) return;

        NSAudioService.Current.Play(SoundId.GiftOpen);

        var reward = def.PendingReward;

        def.PendingReward = null;
        def.State = CodexItemState.Unlocked;
        
        GameDataService.Current.CodexManager.ClaimRewardForPiece(def.PieceTypeDef.Id);
        
        // todo: use something like: targetEntity.WindowController.Window.Layers[0].ViewCamera.WorldToScreenPoint(taskIcon.transform.position)
        var flyPosition = GetComponentInParent<Canvas>().worldCamera.WorldToScreenPoint(pieceImageRectTransform.position);
        var transactions = CurrencyHelper.PurchaseAsync(reward, null, flyPosition);
        
        Analytics.SendPurchase("screen_collection", "item1", null, new List<CurrencyPair>(reward), false, false);
        
        PlayGiftOpenAnimation(() =>
        {
            foreach (var transaction in transactions)
            {
                transaction.Complete();
            }
            
            if (reward != null && reward.Count != 0) return;
            Debug.LogError($"[CodexItem] => ClaimReward: No unlock bonus specified for [{def.PieceDef.Id}] {def.PieceTypeDef.Abbreviations[0]}");
        });
    }

    private void OnEnable()
    {
        if (def == null)
        {
            return;
        }

        if (Def.State == CodexItemState.PendingReward)
        {
            PlayGiftIdleAnimation();
        }
    }
}