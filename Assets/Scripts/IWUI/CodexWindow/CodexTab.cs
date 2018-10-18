using System.Collections;
using Boo.Lang;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexTab : Tab
{
    [SerializeField] private Transform chainsHost;
    [SerializeField] private TextMeshProUGUI captionActive;
    [SerializeField] private TextMeshProUGUI captionDisabled;    
    [SerializeField] private GameObject exclamationMarkActive;
    [SerializeField] private GameObject exclamationMarkDisabled;
    [SerializeField] private ScrollRect scroll;
    
    private readonly List<CodexChain> codexChains = new List<CodexChain>();

    public void Init(CodexTabDef def)
    {
        captionActive.text = def.Name;
        captionDisabled.text = def.Name;

        if (exclamationMarkActive != null)
        {
            exclamationMarkActive.SetActive(def.PendingReward);
        }
        
        exclamationMarkDisabled.SetActive(def.PendingReward);

        ScrollToTop();
    }
    
    public void AddChain(CodexChain codexChain)
    {
        // if (codexChains.Count == 0)
        // {
        //     var tempItem = chainsHost.GetChild(0);
        //     Destroy(tempItem.gameObject);
        // }
        
        codexChains.Add(codexChain);
        codexChain.transform.SetParent(chainsHost, false);
    }

    public void ScrollToTop()
    {
        scroll.normalizedPosition = new Vector2(0.5f, 1);
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
        scroll.enabled = false;
        
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
        yield return new WaitForEndOfFrame();
        
        // Respect space between top size of the viewport and chain
        const float PADDING = 7f;

        RectTransform chainRect = target.GetComponent<RectTransform>();
        float chainY   = chainRect.localPosition.y;
        float chainH   = chainRect.sizeDelta.y;
        float chainTop = chainY + chainH / 2 + PADDING;

        float contentH = scroll.content.sizeDelta.y;

        float scrollToY           = -chainTop;
        float scrollToYNormalized = 1 - scrollToY / contentH;

        scrollToYNormalized = Mathf.Clamp(scrollToYNormalized, 0, 1);

        // Vector2 targetValue = new Vector2(0.5f, scrollToYNormalized);

        // DOTween.To(() => scroll.normalizedPosition, (pos) => { scroll.normalizedPosition = pos; }, targetValue, 1f)
        //        .SetEase(Ease.InOutBack);

        // Debug.LogWarning($"[CodexTab] => ScrollTo\nchainY: {chainY}\nchainH: {chainH}\nchainTop: {chainTop}\ncontentH: {contentH}\nscrollToY:{scrollToY}\nscrollToYNormalized:{scrollToYNormalized}\n");

        DOTween.Kill(scroll.content);
        
        scroll.content.DOAnchorPosY(scrollToY, 1.0f)
              .SetEase(Ease.InOutBack)
              .SetId(scroll.content)
              .OnComplete(() => { scroll.enabled = true; });
    }
}