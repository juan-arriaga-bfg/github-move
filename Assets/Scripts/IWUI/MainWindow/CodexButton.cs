using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexButton : MonoBehaviour
{
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject exclamationMark;

    public void UpdateState()
    {
        ToggleShine(false);
        ToggleExclamationMark(false);
        
        var codexManager = GameDataService.Current.CodexManager;
        switch (codexManager.CodexState)
        {
            case CodexState.NewPiece:
                ToggleShine(true);
                ToggleExclamationMark(true);
                break;
            
            case CodexState.PendingReward:
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
