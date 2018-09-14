using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class UICurrencyCheatSheetWindowView : IWUIWindowView 
{
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICurrencyCheatSheetWindowModel windowModel = Model as UICurrencyCheatSheetWindowModel;
        
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UICurrencyCheatSheetWindowModel windowModel = Model as UICurrencyCheatSheetWindowModel;
        
    }

    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
    }

    private List<CurrencyDef> GetCurrencies()
    {
        List<FieldInfo> fields = new List<FieldInfo>();
        
        var classesList = Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsClass && e.IsDefined(typeof(IncludeToCheatSheet))).ToList();
        foreach (var classType in classesList)
        {
            var classFields = classType.GetFields();
            var result = classFields.Where(e => e.IsDefined(typeof(IncludeToCheatSheet))).ToList();

            if (result.Count > 0)
            {
                fields.AddRange(result);
            }
        } 

        List<CurrencyDef> ret = new List<CurrencyDef>();
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(CurrencyDef))
            {
                CurrencyDef def = (CurrencyDef) field.GetValue(null);
                ret.Add(def);
            }
        }

        return ret;
    }
}
