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
        
        CreateItems();
        
        ToggleVisabilityShowedWindows(false);
        
        var removerComponent = BoardService.Current.FirstBoard.BoardLogic.DragAndDrop;
        removerComponent.OnBeginDragAndDropEvent += OnBeginDragAndDropEvent;
        removerComponent.OnEndDragAndDropEvent += OnEndDragAndDropEvent;
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIPiecesCheatSheetWindowModel model = Model as UIPiecesCheatSheetWindowModel;
        
        ToggleVisabilityShowedWindows(true);
        
        var removerComponent = BoardService.Current.FirstBoard.BoardLogic.DragAndDrop;
        removerComponent.OnBeginDragAndDropEvent -= OnBeginDragAndDropEvent;
        removerComponent.OnEndDragAndDropEvent -= OnEndDragAndDropEvent;
    }
    
    private void OnBeginDragAndDropEvent()
    {
        ToggleVisabilityWindow(false, this);
    }
    
    private void OnEndDragAndDropEvent()
    {
        ToggleVisabilityWindow(true, this);
    }
    
    private void CreateItems()
    {
        var filters = new List<string>
        {
            "Simple A", "Simple B", "Simple C", "Simple D",
            "Ingredient A", "Ingredient B", "Ingredient C", "Ingredient D", "Ingredient E",
            "Chests", "Currencies", "Obstacles", "Boosters", "Characters",
        };
        
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
                    Fill(model.GetPieceIds(view.Entity.Uid), itemsPanel);
                    
                    var tabsScrollRect = itemsPanel.GetScrollRect();
                    if (tabsScrollRect != null)
                    {
                        tabsScrollRect.horizontalNormalizedPosition = 0f;
                    }
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

        // update items
        var views = new List<IUIContainerElementEntity>(entities.Count);
        for (int i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
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
