using System;
using System.Collections.Generic;
using System.Linq;

public class UIPiecesCheatSheetWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("#Container")] private UIContainerViewController itemsPanel;
    
    [IWUIBinding("#TabsContainer")] private UIContainerViewController tabsPanel;
    

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UIPiecesCheatSheetWindowModel model = Model as UIPiecesCheatSheetWindowModel;

        SetTitle(model.Title);
        
        CreateItems(model);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIPiecesCheatSheetWindowModel model = Model as UIPiecesCheatSheetWindowModel;
    }
    
    private void CreateItems(UIPiecesCheatSheetWindowModel model)
    {
        var filters = Enum.GetValues(typeof(PieceTypeFilter)).Cast<PieceTypeFilter>().Select(x => Enum.GetName(typeof(PieceTypeFilter), x)).ToList();
        FillTabs(filters, tabsPanel);
        var tabsScrollRect = tabsPanel.GetScrollRect();
        if (tabsScrollRect != null)
        {
            tabsScrollRect.horizontalNormalizedPosition = 0f;
        }
    }
    
    public void FillTabs(List<string> entities, UIContainerViewController container)
    {
        if (entities == null || entities.Count <= 0)
        {
            container.Clear();
            return;
        }

        // update items
        var views = new List<IUIContainerElementEntity>(entities.Count);
        for (int i = 0; i < entities.Count; i++)
        {
            var def = entities[i];

            var entity = new UITabContainerElementEntity
            {
                Uid = def,
                TabLabel = def,
                OnSelectEvent = (view) =>
                {
                    UIPiecesCheatSheetWindowModel model = Model as UIPiecesCheatSheetWindowModel;
                    Fill(model.GetPieceIdsBy((PieceTypeFilter)Enum.Parse(typeof(PieceTypeFilter), view.Entity.Uid)), itemsPanel);
                },
                OnDeselectEvent = null
            };
            views.Add(entity);
        }
        container.Create(views);
        container.Select(0);
    }
    
    public void Fill(List<int> entities, UIContainerViewController container)
    {
        if (entities == null || entities.Count <= 0)
        {
            container.Clear();
            return;
        }
        
        var exclude = new HashSet<int>
        {
            0, 
            -1,
            1
        };
        
        // update items
        var views = new List<IUIContainerElementEntity>(entities.Count);
        for (int i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            // exclude items
            if (exclude.Contains(def)) continue;

            var entity = new UIPiecesCheatSheetElementEntity
            {
                PieceId = def,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            views.Add(entity);
        }
        container.Create(views);
        container.Select(0);
    }
}
