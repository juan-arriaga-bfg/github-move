using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public const float MIN_ITEM_IMAGE_SIZE = 90;
    public const float MAX_ITEM_IMAGE_SIZE = 155;

    [IWUIBindingNullable("#Caption")] private TextMeshProUGUI caption;
    [IWUIBinding("#(?)")]     private GameObject questionMark;
    [IWUIBindingNullable("#Arrow")]   private GameObject arrow;
    [IWUIBinding("#Shine")]   private GameObject shine;
    [IWUIBindingNullable("#Basket")]  private GameObject basket;
    [IWUIBindingNullable("#Hand")]    private GameObject hand;
    [IWUIBinding("#Piece")]   private RectTransform pieceImageRectTransform;
    [IWUIBinding("#Gift")]    private GameObject gift;
    [IWUIBinding("#Gift")]    private Animator giftAnimator;

    public CodexChain Context;
    
    // private CodexItemState state;

    private readonly Color COLOR_TRANSPARENT = new Color(1, 1, 1, 0);
    
    private CodexItemDef def;
    
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
    
    public void Setup(CodexItemDef itemDef, bool forceHideArrow)
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
                questionMark.SetActive(true);
                CreateIcon(false);
                               
                break;
            
            case CodexItemState.PartLock:
                questionMark.SetActive(true);
                CreateIcon(Context.Context == null || Context.Context.IsHero == false);
                
                foreach (var sprite in IconSprites)
                {
                    sprite.material = lockedMaterial;
                    sprite.color = lockedColor;
                }
                
                break;
            
            case CodexItemState.PendingReward:
                CreateIcon(false);
                shine.SetActive(true);
                PlayGiftIdleAnimation();
                
                break;
            
            case CodexItemState.Unlocked:
                CreateIcon(true);
                shine.SetActive(false);
                if (hand != null) hand.SetActive(def.PieceTypeDef.Filter.Has(PieceTypeFilter.Ingredient));

                break;
            
            case CodexItemState.Highlighted:
                CreateIcon(true);
                shine.SetActive(true);

                icon.transform.localScale = rewardScale;
                
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
        if (def == null)
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

    public void OnClick()
    {
        if(Context == null || Context.Context == null) return;
        
        switch (def.State)
        {
            case CodexItemState.FullLock:
                if(Context.Context.IsHero) Context.Context.SelectItem.SetItem(null);
                break;
            
            case CodexItemState.PartLock:
                if(Context.Context.IsHero) Context.Context.SelectItem.SetItem(null);
                break;
            
            case CodexItemState.PendingReward:
                if(Context.Context.IsHero) Context.Context.SelectItem.SetItem(def.PieceDef);
                ClaimReward();
                break;
            
            case CodexItemState.Unlocked:
                if(Context.Context.IsHero) Context.Context.SelectItem.SetItem(def.PieceDef);
                break;
            
            case CodexItemState.Highlighted:
                if(Context.Context.IsHero) Context.Context.SelectItem.SetItem(def.PieceDef);
                break;
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