using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIAbTestCheatSheetWindowModel : IWWindowModel 
{
    public string Title => "AB Tests";

    public List<AbTestItem> Items => GameDataService.Current?.AbTestManager?.Tests?.Values.ToList();
}
