using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UICurrencyCheatSheetWindowView : UIGenericPopupWindowView
{
    [SerializeField] private UICurrencyCheatSheetWindowItem itemPrefab;

    private List<UICurrencyCheatSheetWindowItem> items;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICurrencyCheatSheetWindowModel model = Model as UICurrencyCheatSheetWindowModel;

        SetTitle(model.Title);
        CreateItems(model);
    }

    private void CreateItems(UICurrencyCheatSheetWindowModel model)
    {
        itemPrefab.gameObject.SetActive(true);
        
        items = new List<UICurrencyCheatSheetWindowItem>();
        var currencies = model.CurrenciesList();

        foreach (var currencyDef in currencies)
        {
            var item = Instantiate(itemPrefab);
            item.transform.SetParent(itemPrefab.transform.parent, false);
            item.Init(currencyDef); 
            
            items.Add(item);
        }

        itemPrefab.gameObject.SetActive(false);
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        UICurrencyCheatSheetWindowModel windowModel = Model as UICurrencyCheatSheetWindowModel;

        bool isAnyChange = false;
        
        foreach (var item in items)
        {
            isAnyChange = isAnyChange || item.IsChanged();
            Destroy(item.gameObject);
        }

        if (isAnyChange)
        {
            ReloadScene();
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
            
        var ecsSystems = new List<IECSSystem>(ECSService.Current.SystemProcessor.RegisteredSystems);
            
        foreach (var system in ecsSystems)
        {
            ECSService.Current.SystemProcessor.UnRegisterSystem(system);
        }
    }

    public override void InitView(IWWindowModel model, IWWindowController controller)
    {
        base.InitView(model, controller);
    }
}
