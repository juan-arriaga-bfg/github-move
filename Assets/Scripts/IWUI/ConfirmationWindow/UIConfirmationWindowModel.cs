using System;
using UnityEngine;

public class UIConfirmationWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.confirmation.title", "window.confirmation.title");
    public string Message => LocalizationService.Get("window.confirmation.message", "window.confirmation.message");

    public string ButtonText;
    public string ProductAmountText;
    public string ProductNameText;
    
    public string Icon;
    
    public Action<Transform> OnAccept;
    public Action OnCancel;
}