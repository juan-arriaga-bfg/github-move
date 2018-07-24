using Lean.Touch;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    [SerializeField] private CameraManipulator cameraManipulator;
    
    private int border;
    private float speed = 60f;
    
    private Vector2 half;
    private LayerMask defaultMask;
    
    private void Awake()
    {
        border = (int)(200 * (Screen.height / 1080f));
        half = new Vector2(Screen.width, Screen.height) * 0.5f;
        defaultMask = LeanTouch.Instance.GuiLayers;
    }

    private void LateUpdate()
    {
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