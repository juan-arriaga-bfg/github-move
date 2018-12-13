using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICodexWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#PanelTabs")] private TabGroup tabGroup;
    [IWUIBinding("#Tab")] private GameObject tabPrefab; 
    
    // Debug
    [IWUIBinding("#ButtonUnlockAll")] private UIButtonViewController btnUnlockAll; 

    // If you change this, also change Grid Layout component settings in Chain prefab
    private const int ITEMS_IN_ROW_COUNT = 6;

    private int lastCodexContentId = -1;

    private List<CodexTab> codexTabs = new List<CodexTab>();

    public override void OnViewShow()
    {
        base.OnViewShow();
        
#if DEBUG && UNITY_EDITOR
        btnUnlockAll.gameObject.SetActive(true);
        btnUnlockAll.Init()
              .ToState(GenericButtonState.Active)
              .OnClick(OnUnlockAllClick);
#else
        btnUnlockAll.gameObject.SetActive(false);
#endif
        
        UICodexWindowModel model = Model as UICodexWindowModel;

        Init(model);
        
        // Call update after one frame to make sure that layouts are up to date
        StartCoroutine(UpdateLayout());
    }

    private IEnumerator UpdateLayout()
    {
        yield return new WaitForEndOfFrame();
        
        UICodexWindowModel model = Model as UICodexWindowModel;
        UpdateTabs(model);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UICodexWindowModel model = Model as UICodexWindowModel;   
        
        var codexManager = GameDataService.Current.CodexManager;
        codexManager.ClearCodexContentCache();
        
        model.OnClose?.Invoke();
    }
    
    private void Init(UICodexWindowModel model)
    {
        if (model.CodexContent.InstanceId == lastCodexContentId)
        {
            return;
        }

        lastCodexContentId = model.CodexContent.InstanceId;
        
        SetTitle(model.Title);
        
        // todo: refresh instead of recreate

        tabPrefab.SetActive(false);

        for (var i = 0; i < codexTabs.Count; i++)
        {
            var tabGo = codexTabs[i].gameObject;
            Destroy(tabGo);
        }
        
        tabGroup.RemoveAllTabs();

        CreateTabs(model.CodexContent.TabDefs);       
    }

    private void CreateTabs(List<CodexTabDef> tabDefs)
    {
        codexTabs = new List<CodexTab>();

        for (var i = 0; i < tabDefs.Count; i++)
        {
            var codexTabDef = tabDefs[i];
            
            var tabGo  = Instantiate(tabPrefab);
            tabGo.SetActive(true);
            
            CodexTab tab = tabGo.GetComponent<CodexTab>();
            tab.Init(codexTabDef);
            
            tabGroup.AddTab(tab, i);

            CreateChains(tab, codexTabDef);

            codexTabs.Add(tab);
        }
    }
    
    private void UpdateTabs(UICodexWindowModel model)
    {
        // All tabs shold be scrolled to top
        for (var i = 0; i < codexTabs.Count; i++)
        {
            var tab = codexTabs[i];
            tab.ScrollToTop();
        }

        // some tab forced
        if (model.ActiveTabIndex != -1)
        {
            tabGroup.ActivateTab(model.ActiveTabIndex);
            return;
        }

        // Scan all tabs to find (!) to focus
        for (var i = 0; i < model.CodexContent.TabDefs.Count; i++)
        {
            var tabDef = model.CodexContent.TabDefs[i];
            foreach (var chainDef in tabDef.ChainDefs)
            {
                foreach (var itemDef in chainDef.ItemDefs)
                {
                    if (itemDef.PendingReward != null)
                    {
                        tabGroup.ActivateTab(i);

                        // Scroll
                        var target = chainDef.ItemDefs[0].PieceTypeDef.Id;
                        codexTabs[i].ScrollTo(target);
                        return;
                    }
                }
            }
        }
        
        // fallback
        tabGroup.ActivateTab(0);
    }

    private static void CreateChains(CodexTab tab, CodexTabDef tabDef)
    {
        var chainDefs = tabDef.ChainDefs;
        
        for (var i = 0; i < chainDefs.Count; i++)
        {
            var codexChainDef = chainDefs[i];

            CodexChain chain = UIService.Get.PoolContainer.Create<CodexChain>((GameObject) ContentService.Current.GetObjectByName("CodexChain"));
            chain.Init(codexChainDef);
            
            tab.AddChain(chain);

            CreateItems(chain, codexChainDef, ITEMS_IN_ROW_COUNT);
        }
    }

    public static void CreateItems(CodexChain chain, CodexChainDef chainDef, int rowLength)
    {
        var itemDefs = chainDef.ItemDefs;
        
        List<CodexItem> itemsToAdd = new List<CodexItem>();
        
        for (var i = 0; i < itemDefs.Count; i++)
        {
            var codexItemDef = itemDefs[i];
            
            CodexItem item = UIService.Get.PoolContainer.Create<CodexItem>((GameObject) ContentService.Current.GetObjectByName("CodexItem"));

            bool forceHideArrow = (i + 1) % rowLength == 0;
            item.OnViewInit(null);
            item.Setup(codexItemDef, forceHideArrow);
            
            itemsToAdd.Add(item);
        }
        
        chain.AddItems(itemsToAdd);
    }

    // DEBUG ONLY!
    public void OnUnlockAllClick()
    {
        var state = Input.GetKey(KeyCode.LeftShift) ? CodexItemState.PendingReward : CodexItemState.Unlocked;
        
        foreach (var tab in codexTabs)
        {
            CodexItem[] items = GetComponentsInChildren<CodexItem>(tab.gameObject);
            foreach (var item in items)
            {
                item.ReloadWithState(state);
            }

            CodexChain[] chains = GetComponentsInChildren<CodexChain>(tab.gameObject);
            foreach (var chain in chains)
            {
                chain.FixItemsSize();
            }
        }
    }
}
