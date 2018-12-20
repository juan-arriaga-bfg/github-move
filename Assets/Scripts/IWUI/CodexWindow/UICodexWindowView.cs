using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UICodexWindowView : UIGenericPopupWindowView
{
    // [IWUIBinding("#PanelTabs")] private TabGroup tabGroup;
    [IWUIBinding("#TabPrefab")] private GameObject tabPrefab; 
    [IWUIBinding("#Toggles")] private UIContainerViewController contentToggles;
    
    // Debug
    [IWUIBinding("#ButtonUnlockAll")] private UIButtonViewController btnUnlockAll; 

    // If you change this, also change Grid Layout component settings in Chain prefab
    private const int ITEMS_IN_ROW_COUNT = 6;

    private int lastCodexContentId = -1;

    private List<CodexTab> codexTabs = new List<CodexTab>();

    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
        
        var windowModel = Model as UICodexWindowModel;

        CodexContent codexContent = GameDataService.Current.CodexManager.GetCodexContent();
        
        CreateTabs(codexContent.TabDefs);    
        
        Fill(UpdateEntitiesToggles(codexContent.TabHeaders), contentToggles);
        
        contentToggles.transform.SetAsLastSibling();

        // Ensure that all layouts is up to date before going to background
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetCanvas().GetComponent<RectTransform>());
        
        lastCodexContentId = codexContent.InstanceId;
    }
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
#if DEBUG && UNITY_EDITOR
        btnUnlockAll.gameObject.SetActive(true);
        InitBtnBase(btnUnlockAll, OnUnlockAllClick);
#else
        btnUnlockAll.gameObject.SetActive(false);
#endif
        
        var model = Model as UICodexWindowModel;

        ReInit(model);
        SetTitle(model.Title);
        
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
        
        // var codexManager = GameDataService.Current.CodexManager;
        // codexManager.ClearCodexContentCache();
        
        model.OnClose?.Invoke();
    }
    
    private void ReInit(UICodexWindowModel model)
    {
        UpdateExclamationMarks();

        if (model.CodexContent.InstanceId == lastCodexContentId)
        {
            return;
        }

        foreach (var tab in codexTabs)
        {
            tab.ReturnContentToPool();
        }
       
        for (var i = 0; i < codexTabs.Count; i++)
        {
            var tab = codexTabs[i];
            CreateChains(tab, model.CodexContent.TabDefs[i]);
        }
        
        //int activeTabIndex = CalculateActiveTabIndexFromDefs(model);
        
        //CreateChains(codexTabs[activeTabIndex], model.CodexContent.TabDefs[activeTabIndex]);

        //StartCoroutine(CreateChainsCoroutine(activeTabIndex, model));
        
        lastCodexContentId = model.CodexContent.InstanceId;
    }

    private void UpdateExclamationMarks()
    {
        UICodexWindowModel model = Model as UICodexWindowModel;
        
        if (model?.CodexContent?.TabDefs == null)
        {
            return;
        }
        
        for (var i = 0; i < model.CodexContent.TabDefs.Count; i++)
        {
            var tabDef = model.CodexContent.TabDefs[i];
            ((UISimpleTabContainerElementViewController) contentToggles.Tabs[i]).ToggleExclamationMark(tabDef.PendingReward);
        }
    }

    // private IEnumerator CreateChainsCoroutine(int ignoreIndex, UICodexWindowModel model)
    // {
    //     for (var i = 0; i < codexTabs.Count; i++)
    //     {
    //         if (i == ignoreIndex)
    //         {
    //             continue;
    //         }
    //         
    //         var tab = codexTabs[i];
    //         CreateChains(tab, model.CodexContent.TabDefs[i]);
    //
    //         yield return new WaitForEndOfFrame();
    //     }
    // }
    
    private void CreateTabs(List<CodexTabDef> tabDefs)
    {
        codexTabs = new List<CodexTab>();

        for (var i = 0; i < tabDefs.Count; i++)
        {
            var codexTabDef = tabDefs[i];
            
            var tabGo = Instantiate(tabPrefab);
            tabGo.transform.SetParent(tabPrefab.transform.parent, false);
            tabGo.name = $"TabContent_{i}";
            tabGo.SetActive(true);
            
            CodexTab tab = tabGo.GetComponent<CodexTab>();
            tab.OnViewInit(null);
            tab.Init(codexTabDef);

            CreateChains(tab, codexTabDef);

            codexTabs.Add(tab);
        }
        
        tabPrefab.SetActive(false);
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
            contentToggles.Select(model.ActiveTabIndex);
            return;
        }

        // Scan all tabs to find (!) to focus
        for (var tabIndex = 0; tabIndex < model.CodexContent.TabDefs.Count; tabIndex++)
        {
            var tabDef = model.CodexContent.TabDefs[tabIndex];
            for (var chainIndex = 0; chainIndex < tabDef.ChainDefs.Count; chainIndex++)
            {
                var chainDef = tabDef.ChainDefs[chainIndex];
                for (var itemIndex = 0; itemIndex < chainDef.ItemDefs.Count; itemIndex++)
                {
                    var itemDef = chainDef.ItemDefs[itemIndex];
                    if (itemDef.PendingReward != null)
                    {
                        contentToggles.Select(tabIndex);

                        // Scroll
                        var target = chainDef.ItemDefs[0].PieceTypeDef.Id;

                        if (tabDef.ChainDefs.Count == 2)
                        {
                            if (chainIndex == 1)
                            {
                                codexTabs[tabIndex].ScrollToBottom();
                            }
                            else
                            {
                                codexTabs[tabIndex].ScrollToTop();
                            }
                        }
                        else
                        {
                            codexTabs[tabIndex].ScrollTo(target);
                        }

                        return;
                    }
                }
            }
        }
        
        // fallback
        contentToggles.Select(0);
    }

    private int CalculateActiveTabIndexFromDefs(UICodexWindowModel model)
    {
        for (var i = 0; i < model.CodexContent.TabDefs.Count; i++)
        {
            var tabDef = model.CodexContent.TabDefs[i];
            if (tabDef.PendingReward)
            {
                return i;
            }
        }

        return 0;
    }

    private List<IUIContainerElementEntity> UpdateEntitiesToggles(List<string> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UISimpleTabContainerElementEntity
            {
                LabelText = def,
                CheckmarkText = def,
                OnSelectEvent = view =>
                {
                    OnSelectToggle(view.Index);
                },
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }

    private void OnSelectToggle(int index)
    {
        for (var i = 0; i < codexTabs.Count; i++)
        {
            var tab = codexTabs[i];
            tab.gameObject.SetActive(i == index);
        }
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
    
    private void OnEnable()
    {
        GameDataService.Current.CodexManager.OnPieceRewardClaimed += OnPieceRewardClaimed;
    }

    private void OnDisable()
    {
        GameDataService.Current.CodexManager.OnPieceRewardClaimed -= OnPieceRewardClaimed;
    }
    
    private void OnPieceRewardClaimed(int id)
    {
        UpdateExclamationMarks();
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
