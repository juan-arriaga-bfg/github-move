using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexButton : MonoBehaviour
{
    [SerializeField] private GameObject shine;
    
    public void ToggleShine(bool enabled)
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
}
