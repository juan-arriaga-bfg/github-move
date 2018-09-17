using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class UICurrencyCheatSheetWindowModel : IWWindowModel 
{
    public string Title => "Cheat Sheet";
    
    public List<CurrencyDef> CurrenciesList()
    {
        List<FieldInfo> fields = new List<FieldInfo>();
        
        var classesList = Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsClass && e.IsDefined(typeof(IncludeToCheatSheet))).ToList();
        foreach (var classType in classesList)
        {
            var classFields = classType.GetFields();
            var result      = classFields.Where(e => e.IsDefined(typeof(IncludeToCheatSheet))).ToList();

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
