#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using Wilberforce;

public class ColorBlindController : MonoBehaviour
{
    // Normal Vision
    // Protanopia
    // Deuteranopia
    // Tritanopia
    private static int mode;

    private static bool enabled;
    
    private static readonly object ID = new object();
    
    private static void Toggle(bool enabled, int mode, bool manual = true)
    {
        ColorBlindController.mode = mode;
        ColorBlindController.enabled = enabled;
        
        var cams = FindObjectsOfType<Camera>();
        List<Colorblind> colorBlinds = new List<Colorblind>();
        Camera selectedCam = cams[0];

        foreach (var cam in cams)
        {
            var cmp = cam.gameObject.GetComponent<Colorblind>();
            if (cmp != null)
            {
                colorBlinds.Add(cmp);
            }

            if (cam.depth > selectedCam.depth)
            {
                selectedCam = cam;
            }
        }

        if (manual)
        {
            DOTween.Kill(ID);
        }

        if (!enabled)
        {
            foreach (var colorblind in colorBlinds)
            {
                Destroy(colorblind);
            }
        }

        if (enabled)
        {
            foreach (var colorblind in colorBlinds)
            {
                if (colorblind.gameObject != selectedCam.gameObject)
                {
                    Destroy(colorblind);
                }
            }

            Colorblind curBlind = selectedCam.gameObject.GetComponent<Colorblind>();
            if (curBlind == null)
            {
                curBlind = selectedCam.gameObject.AddComponent<Colorblind>();
            }

            if (curBlind.Type != mode)
            {
                curBlind.Type = mode;
            }

            if (manual)
            {
                DOTween.Sequence()
                       .AppendInterval(0.1f)
                       .AppendCallback(() =>
                        {
                            Toggle(ColorBlindController.enabled, ColorBlindController.mode, false);
                        })
                       .SetLoops(-1);
            }
        }
    }

    [MenuItem("Tools/Color Blind/Normal Vision")]
    public static void SetModeNormal()
    {
        Toggle(false, 0);
    }
    
    [MenuItem("Tools/Color Blind/Protanopia")]
    public static void SetModeProtanopia()
    {
        Toggle(true, 1);
    }    
    
    [MenuItem("Tools/Color Blind/Deuteranopia")]
    public static void SetModeDeuteranopia()
    {
        Toggle(true, 2);
    }    
    
    [MenuItem("Tools/Color Blind/Tritanopia")]
    public static void SetModeTritanopia()
    {
        Toggle(true, 3);
    }
    
    // Check state to toggle menu items
    [MenuItem("Tools/Color Blind/Normal Vision", true)]
    public static bool CheckNormalVision()
    {
        return Application.isPlaying;
    }
    
    [MenuItem("Tools/Color Blind/Protanopia", true)]
    public static bool CheckProtanopia()
    {
        return Application.isPlaying;
    }
    
    [MenuItem("Tools/Color Blind/Deuteranopia", true)]
    public static bool CheckDeuteranopia()
    {
        return Application.isPlaying;
    }
    
    [MenuItem("Tools/Color Blind/Tritanopia", true)]
    public static bool CheckTritanopia()
    {
        return Application.isPlaying;
    }
}

#endif
