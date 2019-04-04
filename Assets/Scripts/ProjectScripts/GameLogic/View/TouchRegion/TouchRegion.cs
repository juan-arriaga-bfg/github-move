using System.Collections.Generic;
using UnityEngine;

public class TouchRegion : IWBaseMonoBehaviour, ITouchableListener
{
    [SerializeField] private List<RectTransform> touchRegions = new List<RectTransform>();
    
    protected BoardElementView context;

    private CameraManipulator cameraManipulator;

    private Rect cachedRect;
    
    protected virtual void Awake()
    {
        context = GetComponent<BoardElementView>();
    }

    public virtual bool IsTouchableAt(Vector2 pos, int state)
    {
        for (int i = 0; i < touchRegions.Count; i++)
        {
            if (state == 0)
            {
                var worldRect = GetWorldRect(touchRegions[i]);
                cachedRect = worldRect;
            }

            if (cachedRect == null)
            {
                continue;
            }
            
            if (Contains(new Vector2(pos.x, pos.y), cachedRect))
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
    
    public bool Contains(Vector2 point, Rect rect)
    {
        return point.x >= rect.xMin && point.x < rect.xMax && point.y >= rect.yMin && point.y < rect.yMax;
    }

    protected virtual void OnEnable()
    {
        if (cameraManipulator == null)
        {
            cameraManipulator = BoardService.Current.FirstBoard.Manipulator.CameraManipulator;
        }
        
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

    public void AddTouchRegion(RectTransform region)
    {
        touchRegions.Add(region);
    }

    public void ClearTouchRegion()
    {
        touchRegions = new List<RectTransform>();
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
