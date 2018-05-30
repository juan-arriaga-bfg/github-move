using Lean.Touch;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    [SerializeField] private CameraManipulator cameraManipulator;
    
    private int border;
    private float speed = 40f;

    private void Awake()
    {
        border = (int)(200 * (Screen.height / 1080f));
    }

    private void LateUpdate()
    {
        if (cameraManipulator.CameraMove.IsLocked == false) return;
        
        var fingers = LeanTouch.GetFingers(true, 1);
         
        if(fingers == null) return;
        
        var finger = fingers[0];
        var currentSpeed = speed * 0.5f;
        
        var factor = LeanTouch.GetDampenFactor(speed, Time.deltaTime);
        var next = Vector3.zero;
        
        if (finger.ScreenPosition.x < border)
        {
            next += Vector3.left;
        }
        else if (finger.ScreenPosition.x > Screen.width - border)
        {
            next += Vector3.right;
        }

        if (finger.ScreenPosition.y < border)
        {
            next += Vector3.down;
        }
        else if (finger.ScreenPosition.y > Screen.height - border)
        {
            next += Vector3.up;
        }
        
        cameraManipulator.MoveTo(Vector3.Lerp(cameraManipulator.CameraPosition, cameraManipulator.CameraPosition + next, factor), false);
    }
}