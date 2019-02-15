using System.Collections.Generic;

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
            "Simple A", "Simple B", "Simple C", "Simple D", "Simple E", "Simple F", "Simple G", "Simple H", "Simple I", "Simple J",
            "Ingredient A", "Ingredient B", "Ingredient C", "Ingredient D", "Ingredient E", "Ingredient F", "Ingredient G",
            "Chests", "Currencies", "Obstacles", "Mines", "Boosters", "Characters",
        };
        
        Fill(UpdateTabsEntities(filters), tabsPanel);
        
        tabsPanel.Select(0);
        
        var tabsScrollRect = tabsPanel.GetScrollRect();
        if (tabsScrollRect != null)
        {
            tabsScrollRect.horizontalNormalizedPosition = 0f;
        }
    }
    
    private List<IUIContainerElementEntity> UpdateTabsEntities(List<string> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (int i = 0; i < entities.Count; i++)
        {
            var def = entities[i];

            var entity = new UISimpleTabContainerElementEntity
            {
                LabelText = def,
                CheckmarkText = def,
                OnSelectEvent = (view) =>
                {
                    var model = Model as UIPiecesCheatSheetWindowModel;
                    var uid = (view.Entity as UISimpleTabContainerElementEntity).LabelText;
                    
                    Fill(UpdateItemsEntities(model.GetPieceIds(uid)), itemsPanel);
                    
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
        
        return views;
    }

    private List<IUIContainerElementEntity> UpdateItemsEntities(List<int> entities)
    {
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
        
        return views;
    }
}
