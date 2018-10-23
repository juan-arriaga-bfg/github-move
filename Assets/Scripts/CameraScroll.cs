using System;
using Lean.Touch;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    [SerializeField] private CameraManipulator cameraManipulator;

    [Header("Interface offset")] 
    [SerializeField] private float leftOffset;
    [SerializeField] private float topOffset;
    [SerializeField] private float rightOffset;
    [SerializeField] private float bottomOffset;
    
    private int border;
    private float speed = 60f;
    
    private Vector2 half;
    private LayerMask defaultMask;
    
    private Vector2? unscaledRectSize;
    public Vector2 UnscaledRectSize => unscaledRectSize ?? (unscaledRectSize = cameraManipulator.CurrentCameraSettings.CameraClampRegion.size) ?? Vector2.one;

    private Vector2? unscaledRectOffset;
    public Vector2 UnscaledRectOffset => unscaledRectOffset ?? (unscaledRectOffset = cameraManipulator.CurrentCameraSettings.CameraClampRegion.position) ?? Vector2.one;

    private float lastCheckedScale;
    
    public void ScaleClampRect()
    {
        var clamp = cameraManipulator.CurrentCameraSettings.CameraClampRegion;
        var resolutionFactor = 1;
        var camera = Camera.main;

        //float leftOffset = 0.03f * 2 + 0.08f;
        //float topOffset = 0.08f;
        //float rightOffset = 0.03f*2 + 0.08f;
        //float bottomOffset = 0.06f ;
        
        //var targetZoom = Mathf.Clamp(cameraManipulator.TargetCameraZoom, cameraZoom.ZoomMin, cameraZoom.ZoomMax);

        Func<float, float> convertFunc = (vect) =>
            Vector3.Distance(camera.ViewportToWorldPoint(new Vector3(vect, 0, 0)),
                camera.ViewportToWorldPoint(Vector3.zero));
        
        var leftResult = convertFunc(leftOffset);
        var topResult = convertFunc(topOffset);
        var rightResult = convertFunc(rightOffset);
        var bottomResult =  convertFunc(bottomOffset);
       //var resultScale = (1 + (targetZoom - cameraZoom.ZoomMin) / (cameraZoom.ZoomMax - cameraZoom.ZoomMin)) * resolutionFactor;
        clamp.size = new Vector2(UnscaledRectSize.x + leftResult + rightResult, UnscaledRectSize.y + bottomResult + topResult);

        clamp.position = new Vector2(
            UnscaledRectOffset.x - leftResult,
            UnscaledRectOffset.y - bottomResult
        );
            
        cameraManipulator.CurrentCameraSettings.CameraClampRegion = clamp;
    }
    
    private void Awake()
    {
        border = (int)(200 * (Screen.height / 1080f));
        half = new Vector2(Screen.width, Screen.height) * 0.5f;
        defaultMask = LeanTouch.Instance.GuiLayers;
    }

    private void LateUpdate()
    {
//        var point = Camera.main.ScreenToViewportPoint(Input.mousePosition);
//        Debug.LogError($"{point.x:F}.{point.y:F}.{point.z:F}");
        if (lastCheckedScale != cameraManipulator.TargetCameraZoom)
        {
            ScaleClampRect();
            lastCheckedScale = cameraManipulator.TargetCameraZoom;
        }
        
        if (cameraManipulator.CameraMove.IsLocked == false) return;
        
        var fingers = LeanTouch.GetFingers(true, 1);
        
        if(fingers == null) return;
        
        var finger = fingers[0];
        
        if(LeanTouch.RaycastGui(finger.ScreenPosition, defaultMask).Count > 0) return;
        
        var speedFactorX = 1f;
        var speedFactorY = 1f;
        
        var ignoreX = false;
        var ignoreY = false;
        
        if (finger.ScreenPosition.x < border)
        {
            speedFactorX = 1 - Mathf.Clamp01(finger.ScreenPosition.x / border);
        }
        else if (finger.ScreenPosition.x > Screen.width - border)
        {
            speedFactorX = 1 - Mathf.Clamp01((Screen.width - finger.ScreenPosition.x) / border);
        }
        else
        {
            ignoreX = true;
        }
        
        if (finger.ScreenPosition.y < border)
        {
            speedFactorY = 1 - Mathf.Clamp01(finger.ScreenPosition.y / border);
        }
        else if (finger.ScreenPosition.y > Screen.height - border)
        {
            speedFactorY = 1 - Mathf.Clamp01((Screen.height - finger.ScreenPosition.y) / border);
        }
        else
        {
            ignoreY = true;
        }
        
        if (ignoreX && ignoreY) return;
        
        Vector3 next = (finger.ScreenPosition - half).normalized;
        var factor = LeanTouch.GetDampenFactor(speed * Mathf.Min(speedFactorX, speedFactorY), Time.deltaTime);
        
        cameraManipulator.MoveTo(Vector3.Lerp(cameraManipulator.CameraPosition, cameraManipulator.CameraPosition + next, factor), false);
    }
}