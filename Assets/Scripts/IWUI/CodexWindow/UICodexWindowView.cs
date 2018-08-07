using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UICodexWindowView : UIGenericPopupWindowView
{
    [SerializeField] private TabGroup tabGroup;

    [SerializeField] private GameObject tabPrefab; 
    [SerializeField] private GameObject chainPrefab; 
    [SerializeField] private GameObject itemPrefab;
    
    [Header("Buttons")]
    [SerializeField] private GameObject btnClose;
    [SerializeField] private GameObject btnReward;
    [SerializeField] private TextMeshProUGUI btnRewardText;

    private bool isInited;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICodexWindowModel model = Model as UICodexWindowModel;

        Init(model);
      
        tabGroup.ActivateTab(model.ActiveTabIndex);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UICodexWindowModel model = Model as UICodexWindowModel;     
    }
    
    private void Init(UICodexWindowModel model)
    {
        if (isInited)
        {
            return;
        }
       
        tabPrefab.SetActive(false);
        chainPrefab.SetActive(false);
        itemPrefab.SetActive(false);
        
        CreateTabs(model.CodexContent.TabDefs);
        ToggleButtons(model);
    }

    private void ToggleButtons(UICodexWindowModel model)
    {
        int reward = model.PendingRewardAmount;
        bool isRewardAvailable = reward > 0;
        
        btnClose.SetActive(!isRewardAvailable);
        btnReward.SetActive(isRewardAvailable);

        btnRewardText.text = $"Claim Reward <sprite name=\"Coins\"> {reward}";
    }

    private void CreateTabs(List<CodexTabDef> tabDefs)
    {
        for (var i = 0; i < tabDefs.Count; i++)
        {
            var codexTabDef = tabDefs[i];
            
            var tabGo  = Instantiate(tabPrefab);
            tabGo.SetActive(true);
            
            CodexTab tab = tabGo.GetComponent<CodexTab>();
            tab.Init(codexTabDef);
            
            tabGroup.AddTab(tab, i);

            CreateChains(tab, codexTabDef);
        }
    }

    private void CreateChains(CodexTab tab, CodexTabDef tabDef)
    {
        var chainDefs = tabDef.ChainDefs;
        
        for (var i = 0; i < chainDefs.Count; i++)
        {
            var codexChainDef = chainDefs[i];
            
            var chainGo = Instantiate(chainPrefab);
            chainGo.SetActive(true);
            
            CodexChain chain = chainGo.GetComponent<CodexChain>();
            chain.Init(codexChainDef);
            
            tab.AddChain(chain);

            CreateItems(chain, codexChainDef);
        }
    }

    private void CreateItems(CodexChain chain, CodexChainDef chainDef)
    {
        var itemDefs = chainDef.ItemDefs;
        
        for (var i = 0; i < itemDefs.Count; i++)
        {
            var codexItemDef = itemDefs[i];
            
            var itemGo = Instantiate(itemPrefab);
            itemGo.SetActive(true);
            
            CodexItem item = itemGo.GetComponent<CodexItem>();
            item.Init(codexItemDef);
            
            chain.AddItem(item);
        }
    }
}
