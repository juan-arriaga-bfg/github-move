using System.Collections.Generic;
using System.Text;

public class UICurrencyCheatSheetWindowView : UIGenericPopupWindowView
{
    [IWUIBinding("#Content")] private UIContainerViewController content;
    
    [IWUIBinding("#ButtonAdd")] private UIButtonViewController btnAdd;
    [IWUIBinding("#ButtonRemove")] private UIButtonViewController btnRemove;
    
    [IWUIBinding("#ButtonAddLabel")] protected NSText buttonAddLabel;
    [IWUIBinding("#ButtonRemoveLabel")] protected NSText buttonRemoveLabel;
    
    private List<CurrencyDef> mainCurrency = new List<CurrencyDef>
    {
        Currency.Coins,
        Currency.Crystals,
        Currency.Energy,
    };
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICurrencyCheatSheetWindowModel model = Model as UICurrencyCheatSheetWindowModel;

        SetTitle(model.Title);
        
        buttonAddLabel.Text = $"{GetBtnLabel()}99999";
        buttonRemoveLabel.Text = $"{GetBtnLabel()}0";
        
        Fill(UpdateEntities(model.CurrenciesList()), content);
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        InitButtonBase(btnAdd, () => OnSet(999999));
        InitButtonBase(btnRemove, () => OnSet(0));
    }

    private string GetBtnLabel()
    {
        var str = new StringBuilder();

        for (var i = 0; i < mainCurrency.Count; i++)
        {
            var def = mainCurrency[i];
            str.Append($"<sprite name=\"{def.Icon}\">{(i == mainCurrency.Count - 1 ? " = " : ", ")}");
        }

        return str.ToString();
    }
    
    private List<IUIContainerElementEntity> UpdateEntities(List<CurrencyDef> entities)
    {
        var views = new List<IUIContainerElementEntity>(entities.Count);
        
        for (var i = 0; i < entities.Count; i++)
        {
            var def = entities[i];
            
            var entity = new UISimpleScrollElementEntity
            {
                ContentId = def.Icon,
                LabelText = def.Name,
                OnSelectEvent = null,
                OnDeselectEvent = null
            };
            
            views.Add(entity);
        }
        
        return views;
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        ProfileService.Instance.Manager.UploadCurrentProfile();
    }

    private void OnSet(int value)
    {
        foreach (UICurrencyCheatSheetElementViewController item in content.Tabs)
        {
            var currency = (item.Entity as UISimpleScrollElementEntity).LabelText;
            
            if (mainCurrency.Find(def => def.Name == currency) == null) continue;
            
            item.SetValue(value);
        }
    }
}
