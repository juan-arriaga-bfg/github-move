using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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

    private bool isReward;

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
        isReward = false;
        
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
    }
    
    public void OnRewardClick()
    {
        isReward = true;
        Controller.CloseCurrentWindow();
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        if(isReward == false) return;

        var windowModel = Model as UICodexWindowModel;
        
        CurrencyHellper.Purchase(
            Currency.Coins.Name,
            GameDataService.Current.CodexManager.GetCodexContent().PendingRewardAmount,
            isSuccess =>
            {
                Save();
                windowModel.OnClaim?.Invoke();
            },
            new Vector2(Screen.width/2f, Screen.height/2f));
    }

    private void Save()
    {
        var codexManager = GameDataService.Current.CodexManager;
        
        var items = codexManager.Items;
        
        foreach (var item in items)
        {
            item.Value.PendingReward.Clear();
        }
        
        codexManager.ClearCodexContentCache();
        codexManager.CodexState = CodexState.Normal;
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

        // No reward, just open the first one tab
        if (model.CodexContent.PendingRewardAmount <= 0)
        {
            tabGroup.ActivateTab(0);
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
        
        for (var i = 0; i < itemDefs.Count; i++)
        {
            var codexItemDef = itemDefs[i];
            
            CodexItem item = UIService.Get.PoolContainer.Create<CodexItem>((GameObject) ContentService.Current.GetObjectByName("CodexItem"));

            bool forceHideArrow = (i + 1) % rowLength == 0;
            item.OnViewInit(null);
            item.Setup(codexItemDef, forceHideArrow);
            
            chain.AddItem(item);
        }
    }

    // DEBUG ONLY!
    public void OnUnlockAllClick()
    {
        foreach (var tab in codexTabs)
        {
            CodexItem[] items = GetComponentsInChildren<CodexItem>(tab.gameObject);
            foreach (var item in items)
            {
                item.ReloadWithState(CodexItemState.Unlocked);
            }
        }
    }
}
