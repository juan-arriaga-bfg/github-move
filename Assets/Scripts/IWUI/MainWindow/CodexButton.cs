using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexButton : MonoBehaviour
{
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject exclamationMark;
    [SerializeField] private NSText label;
    
    public void UpdateState()
    {
        label.Text = LocalizationService.Get("window.main.collection", "window.main.collection");
        
        ToggleShine(false);
        ToggleExclamationMark(false);
        
        var codexManager = GameDataService.Current.CodexManager;
        switch (codexManager.CodexState)
        {
            case CodexState.PendingReward:
                ToggleShine(true);
                ToggleExclamationMark(true);
                break;
        }
    }
    
    private void ToggleShine(bool enabled)
    {
        try
        {
            shine.SetActive(enabled);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    
        
    private void ToggleExclamationMark(bool enabled)
    {
        try
        {
            exclamationMark.SetActive(enabled);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
