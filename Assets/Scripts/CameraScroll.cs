using Lean.Touch;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    [SerializeField] private CameraManipulator cameraManipulator;
    
    public int Border = 200;
    
    private void LateUpdate()
    {
        if (cameraManipulator.CameraMove.IsLocked == false) return;
        
        var fingers = LeanTouch.GetFingers(true, 1);
         
        if(fingers == null) return;
        
        var finger = fingers[0];
        
        // Get t value
        var factor = LeanTouch.GetDampenFactor(20, Time.deltaTime);
        
        if (finger.ScreenPosition.x < Border)
        {
            cameraManipulator.MoveTo(Vector3.Lerp(cameraManipulator.CameraPosition, cameraManipulator.CameraPosition + Vector3.left, factor), false);
        }
        else if (finger.ScreenPosition.x > Screen.width - Border)
        {
            cameraManipulator.MoveTo(Vector3.Lerp(cameraManipulator.CameraPosition, cameraManipulator.CameraPosition + Vector3.right, factor), false);
        }

        if (finger.ScreenPosition.y < Border)
        {
            cameraManipulator.MoveTo(Vector3.Lerp(cameraManipulator.CameraPosition, cameraManipulator.CameraPosition + Vector3.down, factor), false);
        }
        else if (finger.ScreenPosition.y > Screen.height - Border)
        {
            cameraManipulator.MoveTo(Vector3.Lerp(cameraManipulator.CameraPosition, cameraManipulator.CameraPosition + Vector3.up, factor), false);
        }
    }
}