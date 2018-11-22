using System.Collections.Generic;

public class UIPiecesCheatSheetWindowView : UIGenericPopupWindowView 
{
    [IWUIBinding("/#RootCanvas/Body/Border/Content/PanelContent/ScrollView/Viewport/#Container")] private UITabPanelViewController itemsPanel;

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
        Fill(model.PiecesList());
    }
    
    public void Fill(List<int> entities)
    {
        if (entities == null || entities.Count <= 0)
        {
            itemsPanel.Clear();
            return;
        }
        
        var exclude = new HashSet<int>
        {
            0, 
            -1,
            1
        };
        
        // update items
        var tabViews = new List<IUITabElementEntity>(entities.Count);
        for (int i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            // exclude items
            if (exclude.Contains(def)) continue;

            var tabEntity = new UIPiecesCheatSheetElementEntity
            {
                PieceId = def,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            tabViews.Add(tabEntity);
        }
        itemsPanel.Create(tabViews);
        itemsPanel.Select(0);
    }
}
