using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIChestWindowView : UIGenericPopupWindowView 
{
    [SerializeField] private NSText chestTypeLabel;
    [SerializeField] private NSText btnOpenLabel;
    [SerializeField] private NSText btnStartLabel;
    
    [SerializeField] private NSText timerLabel;
    [SerializeField] private NSText timerLengthLabel;
    
    [SerializeField] private GameObject btnOpen;
    [SerializeField] private GameObject btnStart;
    [SerializeField] private GameObject timer;
    
    [SerializeField] private RectTransform IconBox;
    [SerializeField] private RectTransform NameBox;
    
    [SerializeField] private Image chest;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIChestWindowModel;
        
        SetMessage(windowModel.Message);

        chestTypeLabel.Text = windowModel.ChestTypeText;
        
        btnOpenLabel.Text = windowModel.ButtonText;
        btnStartLabel.Text = windowModel.ButtonText;
        timerLengthLabel.Text = windowModel.TimerLength;
        
        btnOpen.SetActive(windowModel.CurrentChestState == ChestState.Progres);
        btnStart.SetActive(windowModel.CurrentChestState == ChestState.Lock);
        
        timerLengthLabel.gameObject.SetActive(windowModel.CurrentChestState == ChestState.Lock);
        timer.SetActive(windowModel.CurrentChestState == ChestState.Progres);

        chest.sprite = windowModel.ChestImage;
        
        IconBox.anchoredPosition = new Vector2(IconBox.anchoredPosition.x, windowModel.CurrentChestState == ChestState.Lock ? 48 : 20);
        NameBox.anchoredPosition = new Vector2(NameBox.anchoredPosition.x, windowModel.CurrentChestState == ChestState.Lock ? -95 : -123);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIChestWindowModel windowModel = Model as UIChestWindowModel;
    }
}