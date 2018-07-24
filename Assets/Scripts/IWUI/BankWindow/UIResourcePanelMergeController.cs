﻿using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIResourcePanelMergeController : UIGenericResourcePanelViewController
{
    [SerializeField] private Image top;
    [SerializeField] private Image bottom;
    
    [SerializeField] private GameObject shine;
    
    [SerializeField] private RectTransform progress;
    [SerializeField] private RectTransform topTransform;
    [SerializeField] private RectTransform chestGo;

    private Chest chest;

    private bool isAnimate;
    
    public override int CurrentValueAnimated
    {
        set
        {
            currentValueAnimated = value;

            if (amountLabel == null) return;
            
            amountLabel.Text = string.Format("{0}/{1}", currentValueAnimated, chest.MergePoints);
            progress.sizeDelta = new Vector2(Mathf.Clamp(215 * currentValueAnimated / (float) chest.MergePoints, 0, 215), progress.sizeDelta.y);

            var isComplete = currentValueAnimated >= chest.MergePoints;
            
            shine.SetActive(isComplete);
            topTransform.anchoredPosition = new Vector2(topTransform.anchoredPosition.x, isComplete ? 35 : 20);
            ChestSetActive(isComplete);
        }
    }

    public override void UpdateView()
    {
        return;
        if (storageItem == null) return;

        currentValue = storageItem.Amount;

        InitChestView();
            
        amountLabel.Text = string.Format("{0}/{1}", currentValueAnimated, chest.MergePoints);
        progress.sizeDelta = new Vector2(Mathf.Clamp(215 * currentValueAnimated / (float) chest.MergePoints, 0, 215), progress.sizeDelta.y);
        shine.SetActive(currentValueAnimated >= chest.MergePoints);
        
        if (icon != null) icon.sprite = IconService.Instance.Manager.GetSpriteById(string.Format(IconPattern, storageItem.Currency));
    }

    private void ChestSetActive(bool isComplete)
    {
        return;
        if (isComplete && isAnimate == false)
        {
            isAnimate = true;
            DOTween.Kill(chestGo);

            chestGo.DOShakePosition(2f).SetId(chestGo).SetLoops(int.MaxValue);
            return;
        }

        if (isAnimate)
        {
            isAnimate = false;
            DOTween.Kill(chestGo);
        }
    }

    public void OnClick()
    {
        return;
    }

    private void InitChestView()
    {
        return;

        var skin = "";
        
        top.sprite = IconService.Current.GetSpriteById(skin + "_2");
        bottom.sprite = IconService.Current.GetSpriteById(skin + "_1");
    }
}