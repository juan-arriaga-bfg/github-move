using UnityEngine;

public class UISaveElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#LevelLabel")] private NSText levelLabel;
    [IWUIBinding("#CurrenciesLabel")] private NSText currenciesLabel;
    [IWUIBinding("#ButtonLabel")] private NSText btnLabel;
    
    [IWUIBinding("#Local")] private GameObject localObj;
    [IWUIBinding("#Server")] private GameObject serverObj;
    
    [IWUIBinding("#ButtonAccept")] private UIButtonViewController button;

    public override void Init()
    {
        base.Init();

        var contentEntity = entity as UISaveElementEntity;
        
        localObj.SetActive(contentEntity.IsLocal);
        serverObj.SetActive(!contentEntity.IsLocal);

        levelLabel.Text = contentEntity.Level;
        btnLabel.Text = contentEntity.Button;
        currenciesLabel.Text = contentEntity.Currencies;
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
		
        button
            .ToState(GenericButtonState.Active)
            .OnClick(OnClick);
    }

    private void OnClick()
    {
        var contentEntity = entity as UISaveElementEntity;
        
        contentEntity.OnAccept?.Invoke();
    }
}