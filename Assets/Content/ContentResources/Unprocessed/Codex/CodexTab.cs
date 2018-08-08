using Boo.Lang;
using TMPro;
using UnityEngine;

public class CodexTab : Tab
{
    [SerializeField] private Transform chainsHost;
    [SerializeField] private TextMeshProUGUI captionActive;
    [SerializeField] private TextMeshProUGUI captionDisabled;    
    [SerializeField] private GameObject exclamationMarkActive;
    [SerializeField] private GameObject exclamationMarkDisabled;
    
    private readonly List<CodexChain> codexChains = new List<CodexChain>();

    public void Init(CodexTabDef def)
    {
        captionActive.text = def.Name;
        captionDisabled.text = def.Name;
        
        exclamationMarkActive.SetActive(def.PendingReward);
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
}