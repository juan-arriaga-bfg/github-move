using System.Collections.Generic;
using UnityEngine;

public class PieceTouchRegion : IWBaseMonoBehaviour, ITouchableListener
{
    [SerializeField] private List<RectTransform> touchRegions = new List<RectTransform>();
    
    private PieceBoardElementView context;

    private CameraManipulator cameraManipulator;
    
    protected virtual void Awake()
    {
        context = GetComponent<PieceBoardElementView>();

        cameraManipulator = GameObject.FindObjectOfType<CameraManipulator>();
    }

    public bool IsTouchableAt(Vector2 pos)
    {
        for (int i = 0; i < touchRegions.Count; i++)
        {
            var worldRect = GetWorldRect(touchRegions[i]);
            if (worldRect.Contains(new Vector2(pos.x, pos.y)))
            {
                return true;
            }
        }
        return false;
    }

    public object GetContext()
    {
        return context;
    }

    protected virtual void OnEnable()
    {
        if (cameraManipulator == null) return;
        
        cameraManipulator.RegisterTouchableListener(this);
    }
    
    protected virtual void OnDisable()
    {
        if (cameraManipulator == null) return;
        
        cameraManipulator.UnRegisterTouchableListener(this);
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        var worldRect = new Rect
        (
            rectTransform.position.x - rectTransform.rect.width * 0.5f, 
            rectTransform.position.y - rectTransform.rect.height * 0.5f, 
            rectTransform.rect.width, 
            rectTransform.rect.height
        );

        return worldRect;
    }
    
    private void OnDrawGizmos()
    {
        for (int i = 0; i < touchRegions.Count; i++)
        {
            var worldRect = GetWorldRect(touchRegions[i]);
            Gizmos.color = new Color(Color.red.r,Color.red.g, Color.red.b, 0.3f);
            Gizmos.DrawCube(worldRect.center, worldRect.size);
        }        
    }
}
