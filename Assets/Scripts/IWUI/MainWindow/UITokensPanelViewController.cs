﻿using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UITokensPanelViewController : UIGenericResourcePanelViewController
{
    [SerializeField] private RectTransform progress;
    [SerializeField] private Transform dot;
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject exclamationMark;
    
    private AmountRange progressBorder = new AmountRange(12, 220);

    private List<Toggle> dots;
    
    public override int CurrentValueAnimated
    {
        set
        {
            currentValueAnimated = value;
            SetLabelText(currentValueAnimated);
            UpdateProgress(currentValueAnimated);
        }
    }

    public override void OnViewShow(IWUIWindowView context)
    {
        var amount = GameDataService.Current.EventGameManager.CurrentEventGame.Steps.Count;

        for (var i = 1; i < amount; i++)
        {
            Instantiate(dot, dot.parent);
        }

        dots = dot.parent.GetComponentsInChildren<Toggle>().ToList();
        
        UpdateMark();
        
        base.OnViewShow(context);
    }

    public override void UpdateView()
    {
        if (storageItem == null) return;

        currentValue = storageItem.Amount;
        SetLabelText(storageItem.Amount);
        UpdateProgress(currentValue);
        
        if (icon != null) icon.sprite = IconService.Instance.Manager.GetSpriteById(string.Format(IconPattern, storageItem.Currency));
    }
    
    public override void UpdateLabel(int value)
    {
        if (amountLabel == null) return;
        
        DOTween.Kill(amountLabel);
        
        var sequence = DOTween.Sequence().SetId(amountLabel);
        sequence.Insert(0f, DOTween.To(() => CurrentValueAnimated, (v) => { CurrentValueAnimated = v; }, value, 0.5f ));
        sequence.Insert(0f, icon.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f)).SetEase(Ease.InSine);
        sequence.Insert(0.3f, icon.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f)).SetEase(Ease.OutSine);
    }
    
    private void UpdateProgress(float value)
    {
        var eventGame = GameDataService.Current.EventGameManager.CurrentEventGame;
        var price = eventGame.Price;
        
        progress.sizeDelta = new Vector2(Mathf.Lerp(progressBorder.Min, progressBorder.Max, value/price), progress.sizeDelta.y);
        
        if (eventGame.IsCompleted || CurrencyHelper.IsCanPurchase(itemUid, price) == false) return;

        CurrencyHelper.Purchase(Currency.EventStep.Name, 1, itemUid, eventGame.IsLastStep ? 0 : price);
        UpdateDots();
        UpdateMark();
    }

    private void UpdateDots()
    {
        var current = GameDataService.Current.EventGameManager.CurrentEventGame.Step;

        for (var i = 0; i < dots.Count; i++)
        {
            dots[i].isOn = i < current;
        }
    }

    public void UpdateMark()
    {
        var eventGame = GameDataService.Current.EventGameManager.CurrentEventGame;
        var step = eventGame.Step;

        for (var i = 0; i < step; i++)
        {
            var def = eventGame.Steps[i];

            if (def.IsNormalIgnoredOrClaimed && (eventGame.IsPremium == false || def.IsPremiumIgnoredOrClaimed)) continue;
            
            shine.SetActive(true);
            exclamationMark.SetActive(true);
            return;
        }
        
        shine.SetActive(false);
        exclamationMark.SetActive(false);
    }
    
    private void SetLabelText(int value)
    {
        if (amountLabel == null) return;
        
        var eventGame = GameDataService.Current.EventGameManager.CurrentEventGame;
        
        value = Mathf.Max(0, value);
        amountLabel.Text = $"<mspace=2.7em>{value}/{eventGame.Price}</mspace>";
    }
    
    public void OnClick()
    {
        UIService.Get.ShowWindow(UIWindowType.EventWindow);
    }
}