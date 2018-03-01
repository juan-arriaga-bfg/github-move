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
        var state = windowModel.Chest.GetState();
        
        SetMessage(windowModel.Message);

        chestTypeLabel.Text = windowModel.ChestTypeText;
        
        btnOpenLabel.Text = windowModel.ButtonText;
        btnStartLabel.Text = windowModel.ButtonText;
        timerLengthLabel.Text = windowModel.TimerLength;
        
        btnOpen.SetActive(state == ChestState.InProgres);
        btnStart.SetActive(state == ChestState.Lock);
        
        timerLengthLabel.gameObject.SetActive(state == ChestState.Lock);
        timer.SetActive(state == ChestState.InProgres);

        chest.sprite = windowModel.ChestImage;
        
        IconBox.anchoredPosition = new Vector2(IconBox.anchoredPosition.x, state == ChestState.Lock ? 48 : 20);
        NameBox.anchoredPosition = new Vector2(NameBox.anchoredPosition.x, state == ChestState.Lock ? -95 : -123);
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UIChestWindowModel windowModel = Model as UIChestWindowModel;
    }
}