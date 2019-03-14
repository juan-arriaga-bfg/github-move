using System.Collections;
using Boo.Lang;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CodexTab : IWUIWindowViewController
{
    [IWUIBinding("#SelectHero")] private GameObject selectHero;
    [IWUIBinding("#SelectHero")] public CodexSelectItem SelectItem;
    
    [IWUIBinding("#Chains")] private VerticalLayoutGroup verticalLayout;
    [IWUIBinding("#Chains")] private Transform chainsHost;
    [IWUIBinding("#Chains")] private ContentSizeFitter contentSizeFitter;
    [IWUIBinding("#ScrollView")] private ScrollRect scroll;
    
    [IWUIBinding("#Viewport")] private RectTransform viewport;

    private readonly List<CodexChain> codexChains = new List<CodexChain>();

    public bool IsHero { get; private set; }
    
    private CodexTabDef defData;

    public List<CodexChain> GetChains()
    {
        return codexChains;
    }
    
    public void Init(CodexTabDef def)
    {
        defData = def;
        IsHero = defData.ChainDefs[0].IsHero;
        
        ScrollToTop();
        
        selectHero.SetActive(IsHero);
        viewport.offsetMax = -new Vector2(0, IsHero ? 200 : 5);
    }

    public void AddChain(CodexChain codexChain)
    {
        codexChains.Add(codexChain);
        codexChain.transform.SetParent(chainsHost, false);
        codexChain.transform.SetAsLastSibling();
    }
    
    public void ReturnContentToPool()
    {
        if (codexChains == null)
        {
            return;
        }

        foreach (var chain in codexChains)
        {
            chain.ReturnContentToPool();
            UIService.Get.PoolContainer.Return(chain.gameObject);
        }
        
        codexChains.Clear();
    }

    public void ScrollToTop()
    {
        scroll.normalizedPosition = new Vector2(0.5f, 1);
        
        if(IsHero == false || SelectItem == null) return;

        var def = defData.ChainDefs[0].ItemDefs[0];
        
        SelectItem.SetItem(def.State > CodexItemState.PendingReward ? def.PieceDef : null, def.State);
    }
    
    public void ScrollToBottom()
    {
        scroll.normalizedPosition = new Vector2(0.5f, 0);
    }

    public void ScrollTo(int chainId)
    {
        CodexChain target = null;
        foreach (var chain in codexChains)
        {
            if (chain.ChainId == chainId)
            {
                target = chain;
                break;
            }
        }

        if (target == null)
        {
            Debug.LogError($"[CodexTab] => ScrollTo({chainId}): chain not found!");
            return;
        }

        RunScrollTween(target);
    }

    private void RunScrollTween(CodexChain target)
    {
        scroll.enabled = IsHero;
        
        DOTween.Sequence()
               .SetId(this)
               .AppendInterval(0.3f)
               .AppendCallback(() =>
                {
                    StartCoroutine(WaitForLayoutAndScroll(target));
                })
                ;
    }

    private IEnumerator WaitForLayoutAndScroll(CodexChain target)
    {       
       // yield return new WaitForEndOfFrame();
        
        // Respect space between top size of the viewport and chain
        const float PADDING = 30f;

        RectTransform chainRect = target.GetComponent<RectTransform>();
        float chainY   = chainRect.localPosition.y;
        float chainH   = chainRect.sizeDelta.y;
        float chainTop = chainY + chainH / 2 + PADDING;

        // float contentH = scroll.content.sizeDelta.y;

        float scrollToY = -chainTop;
        // float scrollToYNormalized = 1 - scrollToY / contentH;

        // scrollToYNormalized = Mathf.Clamp(scrollToYNormalized, 0, 1);

        // Vector2 targetValue = new Vector2(0.5f, scrollToYNormalized);

        // DOTween.To(() => scroll.normalizedPosition, (pos) => { scroll.normalizedPosition = pos; }, targetValue, 1f)
        //        .SetEase(Ease.InOutBack);

        // Debug.LogWarning($"[CodexTab] => ScrollTo\nchainY: {chainY}\nchainH: {chainH}\nchainTop: {chainTop}\ncontentH: {contentH}\nscrollToY:{scrollToY}\nscrollToYNormalized:{scrollToYNormalized}\n");

        DOTween.Kill(scroll.content);

        // Do not overscroll for last element
        
        if (scrollToY + chainH + PADDING >= scroll.content.rect.height)
        {
            yield break;
            // scrollToY = PADDING + 2;
            // scroll.content.anchoredPosition = new Vector2(scroll.content.anchoredPosition.x, scrollToY);
        }
        // var ease = scroll.content.rect.height - scrollToY > chainH / 2 ? Ease.InOutBack : Ease.Linear;
        // ease = Ease.Linear;
        
        scroll.content.DOAnchorPosY(scrollToY, 1.0f)
              .SetEase(Ease.InOutBack)
              .SetId(scroll.content)
              .OnComplete(() => { scroll.enabled = true; });
    }

    private void OnEnable()
    {
        // Hack to fix Layout problems
        StartCoroutine(FixVerticalLayoutBug());
    }

    private IEnumerator FixVerticalLayoutBug()
    {
        if (verticalLayout == null)
        {
            yield break;
        }
        
        yield return new WaitForEndOfFrame();
        
        //Canvas.ForceUpdateCanvases();
        //
        verticalLayout.enabled = false;
        verticalLayout.enabled = true;
        //
        // contentSizeFitter.enabled = false;
        // contentSizeFitter.enabled = true;
    }
}                            