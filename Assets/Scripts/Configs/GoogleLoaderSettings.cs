﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GoogleLink
{
    public string Key;
    public string Link;
}

public class GoogleLoaderSettings : IWEditorSettings<GoogleLoaderSettings>
{
    [SerializeField] List<GoogleLink> configLinks;

    public List<GoogleLink> ConfigLinks
    {
        get
        {
            return configLinks;
        }
        set
        {
            configLinks = value;
        }
    }
    
    public override string GetResourceName()
    {
        return "googleLoader.settings";
    }
}