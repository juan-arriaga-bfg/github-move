using Boo.Lang;
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
        
        scroll.normalizedPosition = new Vector2(0.5f, 0.5f);
    }
}