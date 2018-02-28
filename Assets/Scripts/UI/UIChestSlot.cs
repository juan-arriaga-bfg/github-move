﻿using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIChestSlot : MonoBehaviour
{
    [SerializeField] private NSText timerLabel;
    [SerializeField] private NSText slotLabel;
    [SerializeField] private NSText btnLabel;
    
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject currensy;
    
    [SerializeField] private Image icon;
    [SerializeField] private Image iconOpenTop;
    [SerializeField] private Image iconOpenDown;
    
    [SerializeField] private Image btnImage;
    [SerializeField] private Image slotImage;

    private ChestState state;
    private ChestDef chest;
    
    public void Initialize(ChestState state, ChestDef chest = null)
    {
        this.state = state;
        this.chest = chest;
        slotLabel.Text = string.Format("<color=#FFFEAF>{0}</color>", "CHEST SLOT");
        
        if (chest == null)
        {
            slotImage.color = new Color(74/255f, 74/255f, 74/255f, 1f);
            button.SetActive(false);
            icon.gameObject.SetActive(false);
            timer.SetActive(false);
            iconOpenTop.gameObject.SetActive(false);
            iconOpenDown.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
            return;
        }
        
        timer.SetActive(state == ChestState.Progres);
        currensy.SetActive(state == ChestState.Progres);
        iconOpenTop.gameObject.SetActive(state == ChestState.Open);
        iconOpenDown.gameObject.SetActive(state == ChestState.Open);
        icon.gameObject.SetActive(state != ChestState.Open);

        switch (state)
        {
            case ChestState.Lock:
                btnImage.sprite = IconService.Current.GetSpriteById("btn_orange_norm");
                btnLabel.Text = string.Format("<color=#FFFEAF>UNLOCK IN</color> <size=60>{0}m</size>", new TimeSpan(0, 0, chest.Time).TotalMinutes);
                break;
            case ChestState.Progres:
                btnImage.sprite = IconService.Current.GetSpriteById("btn_blue_norm");
                btnLabel.Text = string.Format("<size=60>{0}</size> <color=#FFFEAF>OPEN</color>", chest.Price.Amount);
                break;
            case ChestState.Open:
                btnImage.sprite = IconService.Current.GetSpriteById("btn_green_norm");
                btnLabel.Text = "<color=#FFFEAF>TAP TO OPEN</color>";
                break;
        }

        var id = "chest_3";
        
        switch ((ChestType) Enum.Parse(typeof(ChestType), chest.Uid))
        {
            case ChestType.Common:
                id = "chest_1";
                break;
            case ChestType.Rare:
                id = "chest_2";
                break;
            case ChestType.Epic:
                id = "chest_4";
                break;
        }
        
        icon.sprite = IconService.Current.GetSpriteById(id);
        iconOpenTop.sprite = IconService.Current.GetSpriteById(id + "_3");
        iconOpenDown.sprite = IconService.Current.GetSpriteById(id + "_1");
    }

    public void OnClick()
    {
        if(state == ChestState.None) return;
        
        if (state == ChestState.Open)
        {
            UIMessageWindowController.CreateNotImplementedMessage();
            return;
        }
        
        var model = UIService.Get.GetCachedModel<UIChestWindowModel>(UIWindowType.ChestWindow);

        model.Chest = chest;
        model.CurrentChestState = state;
        
        UIService.Get.ShowWindow(UIWindowType.ChestWindow);
    }
}