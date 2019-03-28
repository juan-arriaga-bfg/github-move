using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = IW.Logger;

public class UISystemDevParamViewController : UIContainerElementViewController
{
    [IWUIBinding("#Id")] protected NSText id;
    [IWUIBindingNullable("#Label")] protected NSText label;
    [IWUIBinding("#Input")] protected TMP_InputField inputField;
    [IWUIBindingNullable("#Icon")] protected Image icon;
    
    private float oldValue;

    public override void Init()
    {
        base.Init();

        var contentEntity = entity as UISystemDevParamsElementEntity;
        
        icon.gameObject.SetActive(false);
        
        label.Text = $"{contentEntity.Name}";
        
        id.Text = "";
        
        oldValue = contentEntity.DefaultValue;
        
        SetValue(oldValue);
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();
        
        if(entity == null) return;
        
        var contentEntity = entity as UISystemDevParamsElementEntity;
        
        var newValue = float.Parse(inputField.text);
        var deffValue = newValue - oldValue;

        if (Mathf.Abs(deffValue) > float.Epsilon)
        {
            if (contentEntity.OnChanged != null)
            {
                contentEntity.OnChanged(newValue);
            }
            
            Debug.Log($"[UISystemDevParamViewController] => OnEndEdit: {contentEntity.Name}: {oldValue} -> {newValue}");
        }
    }

    public void SetValue(float value)
    {
        var str = value.ToString();
        
        inputField.text = str;
        OnEndEdit(str);
    }
    
    public void OnEndEdit(string value)
    {
        var newValue = float.Parse(value);

        if (Mathf.Abs(newValue - oldValue) > float.Epsilon)
        {
            inputField.text = newValue.ToString();
        }
        else
        {
            inputField.text = oldValue.ToString();
        }
        
        // inputField.textComponent.color = oldValue != newValue ? Color.yellow : Color.white;
    }
}