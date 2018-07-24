using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererLayerParams : MonoBehaviour
{
    [SerializeField] private bool isIgnoreRenderLayer;

    public bool IsIgnoreRenderLayer 
    {
        get
        {
            return isIgnoreRenderLayer;
        }
        set
        {
            isIgnoreRenderLayer = value;
        }
    }
}
